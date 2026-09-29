-- Province-wide classification facts; deploy with the dashboard application contract.
CREATE PROCEDURE dbo.p_Report_Dashboard_OverviewSummary @ForMonth date
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @MonthStart date = DATEFROMPARTS(YEAR(@ForMonth), MONTH(@ForMonth), 1);
    DECLARE @NextMonth date = DATEADD(month, 1, @MonthStart);

    SELECT COUNT(*) AS ActiveEnterprises,
           COALESCE(SUM(CASE WHEN b.HasIndustry = 1
                     AND (',' + REPLACE(ISNULL(e.TypeBusiness, ''), ' ', '') + ',' LIKE '%,1,%'
                       OR ',' + REPLACE(ISNULL(e.TypeBusiness, ''), ' ', '') + ',' LIKE '%,2,%'
                       OR ',' + REPLACE(ISNULL(e.TypeBusiness, ''), ' ', '') + ',' LIKE '%,3,%')
                    THEN 1 ELSE 0 END), 0) AS ConfiguredEnterprises,
           COALESCE(SUM(CASE WHEN b.BusinessRows = 0 THEN 1 ELSE 0 END), 0) AS MissingBusinessRow,
           COALESCE(SUM(CASE WHEN ISNULL(b.HasIndustry, 0) = 0 THEN 1 ELSE 0 END), 0) AS MissingIndustry,
           COALESCE(SUM(CASE WHEN ',' + REPLACE(ISNULL(e.TypeBusiness, ''), ' ', '') + ',' NOT LIKE '%,1,%'
                     AND ',' + REPLACE(ISNULL(e.TypeBusiness, ''), ' ', '') + ',' NOT LIKE '%,2,%'
                     AND ',' + REPLACE(ISNULL(e.TypeBusiness, ''), ' ', '') + ',' NOT LIKE '%,3,%'
                    THEN 1 ELSE 0 END), 0) AS MissingReportType,
           (SELECT COUNT(*) FROM dbo.Report_DataImports AS r
            WHERE r.IsDeleted = 0 AND r.TypeReport = 0
              AND r.ForMonth >= @MonthStart AND r.ForMonth < @NextMonth) AS TypeZeroRows,
           (SELECT COUNT(*) FROM dbo.Report_DataImports AS r
            WHERE r.IsDeleted = 0 AND r.ForMonth >= DATEADD(month, 1,
                DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))) AS FutureDatedRows
    FROM [baocao.sct.cenit.vn.cate].dbo.Cate_Enterprises AS e
    OUTER APPLY
    (
        SELECT COUNT(*) AS BusinessRows,
               MAX(CASE WHEN NULLIF(LTRIM(RTRIM(IndustryIds)), '') IS NOT NULL THEN 1 ELSE 0 END) AS HasIndustry
        FROM [baocao.sct.cenit.vn.cate].dbo.Cate_BusinessEnterprise
        WHERE EnterpriseId = e.EnterpriseId
    ) AS b
    WHERE e.IsActive = 1 AND e.IsDeleted = 0;
END
