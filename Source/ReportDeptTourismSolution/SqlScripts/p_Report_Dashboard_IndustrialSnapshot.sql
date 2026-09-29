-- Deploy together with the dashboard application contract. One requested report type per call.
ALTER PROCEDURE dbo.p_Report_Dashboard_IndustrialSnapshot
    @ForMonth date,
    @TypeReport int,
    @WardId int = NULL,
    @EconomicSectorId int = NULL,
    @IndustryId int = NULL,
    @EnterpriseId int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @TypeReport IS NULL OR @TypeReport NOT IN (1, 2, 3) THROW 50001, 'Unsupported report type', 1;

    DECLARE @Start date = DATEFROMPARTS(YEAR(@ForMonth) - 1, 1, 1);
    DECLARE @End date = DATEFROMPARTS(YEAR(@ForMonth) + 1, 1, 1);
    DECLARE @PrimaryCode varchar(20) = CASE @TypeReport WHEN 1 THEN '0101' WHEN 2 THEN '01' ELSE 'FOB' END;
    DECLARE @SecondaryCode varchar(20) = CASE @TypeReport WHEN 1 THEN '06' WHEN 2 THEN '40' ELSE 'XK_TT' END;
    DECLARE @TertiaryCode varchar(20) = CASE @TypeReport WHEN 1 THEN '07' WHEN 2 THEN '02' ELSE 'UT_XK' END;

    ;WITH Months AS
    (
        SELECT @Start AS ForMonth
        UNION ALL
        SELECT DATEADD(month, 1, ForMonth) FROM Months
        WHERE DATEADD(month, 1, ForMonth) < @End
    ), Cohort AS
    (
        SELECT e.EnterpriseId, e.BusinessName, e.WardId,
               COALESCE(w.WardName, e.WardName) AS WardName,
               b.EconomicSectorId, s.Name AS EconomicSectorName, b.IndustryIds
        FROM [baocao.sct.cenit.vn.cate].dbo.Cate_Enterprises AS e
        CROSS APPLY
        (
            SELECT TOP (1) candidate.EconomicSectorId, candidate.IndustryIds
            FROM [baocao.sct.cenit.vn.cate].dbo.Cate_BusinessEnterprise AS candidate
            WHERE candidate.EnterpriseId = e.EnterpriseId
              AND NULLIF(LTRIM(RTRIM(candidate.IndustryIds)), '') IS NOT NULL
              AND (@EconomicSectorId IS NULL OR candidate.EconomicSectorId = @EconomicSectorId)
              AND (@IndustryId IS NULL OR ',' + REPLACE(candidate.IndustryIds, ' ', '') + ','
                  LIKE '%,' + CONVERT(varchar(11), @IndustryId) + ',%')
            ORDER BY candidate.IndustryIds, candidate.EconomicSectorId
        ) AS b
        LEFT JOIN [baocao.sct.cenit.vn.cate].dbo.Cate_Wards AS w ON w.WardId = e.WardId
        LEFT JOIN [baocao.sct.cenit.vn.cate].dbo.Cate_EconomicSector AS s
            ON s.EconomicSectorId = b.EconomicSectorId
        WHERE e.IsActive = 1 AND e.IsDeleted = 0
          AND ',' + REPLACE(ISNULL(e.TypeBusiness, ''), ' ', '') + ','
              LIKE '%,' + CONVERT(varchar(11), @TypeReport) + ',%'
          AND (@WardId IS NULL OR e.WardId = @WardId)
          AND (@EnterpriseId IS NULL OR e.EnterpriseId = @EnterpriseId)
    ), Imports AS
    (
        SELECT r.EnterpriseId,
               DATEFROMPARTS(YEAR(r.ForMonth), MONTH(r.ForMonth), 1) AS ForMonth,
               r.Code, r.PerformInPeriod
        FROM dbo.Report_DataImports AS r
        WHERE r.IsDeleted = 0 AND r.TypeReport = @TypeReport
          AND r.ForMonth >= @Start AND r.ForMonth < @End
    ), ImportPresence AS
    (
        SELECT EnterpriseId, ForMonth FROM Imports GROUP BY EnterpriseId, ForMonth
    ), MetricGroups AS
    (
        SELECT EnterpriseId, ForMonth, Code, COUNT(*) AS SourceRows,
               MAX(CAST(PerformInPeriod AS decimal(28, 4))) AS SingleValue
        FROM Imports
        GROUP BY EnterpriseId, ForMonth, Code
    ), MetricValues AS
    (
        SELECT EnterpriseId, ForMonth,
               MAX(CASE WHEN Code = @PrimaryCode AND SourceRows = 1 THEN SingleValue END) AS PrimaryValue,
               MAX(CASE WHEN Code = @SecondaryCode AND SourceRows = 1 THEN SingleValue END) AS SecondaryValue,
               MAX(CASE WHEN Code = @TertiaryCode AND SourceRows = 1 THEN SingleValue END) AS TertiaryValue,
               CAST(MAX(CASE WHEN SourceRows > 1 THEN 1 ELSE 0 END) AS bit) AS MetricConflict
        FROM MetricGroups GROUP BY EnterpriseId, ForMonth
    ), Files AS
    (
        SELECT EnterpriseId, DATEFROMPARTS(YEAR(ForMonth), MONTH(ForMonth), 1) AS ForMonth
        FROM dbo.Report_ReportFiles
        WHERE IsDeleted = 0 AND ForMonth >= @Start AND ForMonth < @End
        GROUP BY EnterpriseId, DATEFROMPARTS(YEAR(ForMonth), MONTH(ForMonth), 1)
    )
    SELECT m.ForMonth, @TypeReport AS TypeReport, c.EnterpriseId, c.BusinessName,
           c.WardId, c.WardName, c.EconomicSectorId, c.EconomicSectorName, c.IndustryIds,
           CAST(CASE WHEN p.EnterpriseId IS NULL THEN 0 ELSE 1 END AS bit) AS DataImported,
           CAST(CASE WHEN f.EnterpriseId IS NULL THEN 0 ELSE 1 END AS bit) AS FileSubmittedAnyType,
           v.PrimaryValue, v.SecondaryValue, v.TertiaryValue,
           CAST(ISNULL(v.MetricConflict, 0) AS bit) AS MetricConflict
    FROM Cohort AS c CROSS JOIN Months AS m
    LEFT JOIN ImportPresence AS p ON p.EnterpriseId = c.EnterpriseId AND p.ForMonth = m.ForMonth
    LEFT JOIN MetricValues AS v ON v.EnterpriseId = c.EnterpriseId AND v.ForMonth = m.ForMonth
    LEFT JOIN Files AS f ON f.EnterpriseId = c.EnterpriseId AND f.ForMonth = m.ForMonth
    ORDER BY m.ForMonth, c.EnterpriseId
    OPTION (MAXRECURSION 24);
END
