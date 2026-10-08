-- Deployment proposal only. Review and run manually; no report data is modified.
CREATE PROCEDURE dbo.p_Report_Chatbot_ReportFiles
 @ChatbotUserId int,@ProvinceRoleIds varchar(500),@EnterpriseRoleIds varchar(500),
 @EnterpriseId int,@FromMonth date,@ToMonth date,@Offset int=0,@Limit int=10
AS BEGIN
 SET NOCOUNT ON;
IF NOT EXISTS(SELECT 1 FROM [baocaosct.khanhhoa.gov.vn.cate].dbo.f_Chatbot_UserScope(@ChatbotUserId,@ProvinceRoleIds,@EnterpriseRoleIds) WHERE CanReadDashboard=1) THROW 50110,'Chatbot dashboard access denied',1;

 IF @EnterpriseId IS NULL OR @FromMonth IS NULL OR @ToMonth IS NULL OR DAY(@FromMonth)<>1 OR DAY(@ToMonth)<>1
 OR @FromMonth<CONVERT(date,'20000101') OR @ToMonth<@FromMonth OR DATEDIFF(month,@FromMonth,@ToMonth)>11
 OR @ToMonth>DATEFROMPARTS(YEAR(GETDATE()),MONTH(GETDATE()),1)
 OR @Offset<0 OR @Offset>100000 OR @Limit<1 OR @Limit>50 THROW 50103,'Invalid submission query',1;
 ;WITH Source AS (
 SELECT f.EnterpriseId,e.BusinessName,DATEFROMPARTS(YEAR(f.ForMonth),MONTH(f.ForMonth),1) ReportMonth,
 f.ForMonth SourcePeriod,f.IsDeleted,f.IsLate,f.Reason,f.CreatedDate,f.LastModifiedDate
 FROM dbo.Report_ReportFiles f
 JOIN [baocaosct.khanhhoa.gov.vn.cate].dbo.Cate_Enterprises e ON e.EnterpriseId=f.EnterpriseId AND e.IsActive=1 AND e.IsDeleted=0
 WHERE EXISTS(SELECT 1 FROM [baocaosct.khanhhoa.gov.vn.cate].dbo.f_Chatbot_AuthorizedEnterprises(@ChatbotUserId,@ProvinceRoleIds,@EnterpriseRoleIds,1) a WHERE a.EnterpriseId=f.EnterpriseId) AND f.EnterpriseId=@EnterpriseId AND f.ForMonth>=@FromMonth AND f.ForMonth<DATEADD(month,1,@ToMonth)
 ), Ranked AS(SELECT *,ROW_NUMBER() OVER(ORDER BY ReportMonth DESC,SourcePeriod DESC,CreatedDate DESC) RowNo FROM Source),
 Total AS(SELECT COUNT(*) TotalRow FROM Source)
 SELECT t.TotalRow,r.EnterpriseId,r.BusinessName,r.ReportMonth,r.SourcePeriod,r.IsDeleted,r.IsLate,r.Reason,r.CreatedDate,r.LastModifiedDate
 FROM Total t OUTER APPLY(SELECT * FROM Ranked WHERE RowNo>@Offset AND RowNo<=CONVERT(bigint,@Offset)+@Limit) r;
END
