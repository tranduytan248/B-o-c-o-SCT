-- Industrial management dashboard. Deploy to baocao.sct.cenit.vn after reviewing
-- docs/industrial-dashboard-reconnaissance.md. Read-only against source data.
CREATE PROCEDURE dbo.p_Report_Dashboard_IndustrialSnapshot
    @ForMonth date,
    @WardId int = NULL,
    @EconomicSectorId int = NULL,
    @IndustryId int = NULL,
    @EnterpriseId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Start date = DATEFROMPARTS(YEAR(@ForMonth) - 1, 1, 1);
    DECLARE @End date = DATEFROMPARTS(YEAR(@ForMonth) + 1, 1, 1);

    ;WITH Months AS
    (
        SELECT @Start AS ForMonth
        UNION ALL
        SELECT DATEADD(month, 1, ForMonth)
        FROM Months
        WHERE DATEADD(month, 1, ForMonth) < @End
    ), Cohort AS
    (
        SELECT e.EnterpriseId, e.BusinessName, e.TaxCode, e.WardId,
               COALESCE(w.WardName, e.WardName) AS WardName,
               b.EconomicSectorId, s.Name AS EconomicSectorName,
               mainIndustry.IndustryId, i.IndustryName
        FROM [baocao.sct.cenit.vn.cate].dbo.Cate_BusinessEnterprise AS b
        INNER JOIN [baocao.sct.cenit.vn.cate].dbo.Cate_Enterprises AS e
            ON e.EnterpriseId = b.EnterpriseId
        LEFT JOIN [baocao.sct.cenit.vn.cate].dbo.Cate_EconomicSector AS s
            ON s.EconomicSectorId = b.EconomicSectorId
        LEFT JOIN [baocao.sct.cenit.vn.cate].dbo.Cate_Wards AS w
            ON w.WardId = e.WardId
        OUTER APPLY
        (
            SELECT TRY_CONVERT(int, LEFT(b.IndustryIds,
                CHARINDEX(',', b.IndustryIds + ',') - 1)) AS IndustryId
        ) AS mainIndustry
        LEFT JOIN [baocao.sct.cenit.vn.cate].dbo.Cate_BusinessIndustry AS i
            ON i.IndustryId = mainIndustry.IndustryId
        WHERE e.IsActive = 1 AND e.IsDeleted = 0
          AND (@WardId IS NULL OR e.WardId = @WardId)
          AND (@EconomicSectorId IS NULL OR b.EconomicSectorId = @EconomicSectorId)
          AND (@EnterpriseId IS NULL OR e.EnterpriseId = @EnterpriseId)
          AND (@IndustryId IS NULL OR ',' + ISNULL(b.IndustryIds, '') + ','
               LIKE '%,' + CONVERT(varchar(11), @IndustryId) + ',%')
    ), Indicators AS
    (
        SELECT r.EnterpriseId, r.ForMonth,
               CAST(SUM(CASE WHEN r.Code = '0101' THEN r.PerformInPeriod END) AS decimal(28, 4)) AS IndustrialRevenue,
               CAST(SUM(CASE WHEN r.Code = '06' THEN r.PerformInPeriod END) AS decimal(28, 4)) AS ExportValue,
               CAST(SUM(CASE WHEN r.Code = '07' THEN r.PerformInPeriod END) AS decimal(28, 4)) AS ImportValue
        FROM dbo.Report_DataImports AS r
        WHERE r.IsDeleted = 0 AND r.ForMonth >= @Start AND r.ForMonth < @End
          AND r.Code IN ('0101', '06', '07')
        GROUP BY r.EnterpriseId, r.ForMonth
    ), Files AS
    (
        SELECT f.EnterpriseId, f.ForMonth, f.IsLate, f.Reason,
               ROW_NUMBER() OVER (PARTITION BY f.EnterpriseId, f.ForMonth
                                  ORDER BY f.CreatedDate DESC) AS RowNo
        FROM dbo.Report_ReportFiles AS f
        WHERE f.IsDeleted = 0 AND f.ForMonth >= @Start AND f.ForMonth < @End
    )
    SELECT m.ForMonth, c.EnterpriseId, c.BusinessName, c.TaxCode,
           c.WardId, c.WardName, c.EconomicSectorId, c.EconomicSectorName,
           c.IndustryId, c.IndustryName,
           CAST(CASE WHEN f.EnterpriseId IS NULL THEN 0 ELSE 1 END AS bit) AS Submitted,
           ISNULL(f.IsLate, 0) AS IsLate, f.Reason,
           d.IndustrialRevenue, d.ExportValue, d.ImportValue
    FROM Cohort AS c
    CROSS JOIN Months AS m
    LEFT JOIN Indicators AS d ON d.EnterpriseId = c.EnterpriseId AND d.ForMonth = m.ForMonth
    LEFT JOIN Files AS f ON f.EnterpriseId = c.EnterpriseId AND f.ForMonth = m.ForMonth AND f.RowNo = 1
    ORDER BY m.ForMonth, c.EnterpriseId
    OPTION (MAXRECURSION 24);
END
