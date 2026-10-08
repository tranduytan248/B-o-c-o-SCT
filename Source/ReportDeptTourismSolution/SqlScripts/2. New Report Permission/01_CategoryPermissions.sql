-- Deploy in the category database first. SQL Server 2012.
USE [baocaosct.khanhhoa.gov.vn.cate];
GO
IF OBJECT_ID('dbo.Report_UserBusinessIndustryPermissions','U') IS NULL
 CREATE TABLE dbo.Report_UserBusinessIndustryPermissions (
  UserId int NOT NULL,
  IndustryId int NOT NULL,
  SavedBy varchar(255) NOT NULL,
  SavedOn datetime NOT NULL CONSTRAINT DF_ReportIndustryPermission_SavedOn DEFAULT GETDATE(),
  CONSTRAINT PK_Report_UserBusinessIndustryPermissions PRIMARY KEY(UserId,IndustryId),
  CONSTRAINT FK_ReportIndustryPermission_User FOREIGN KEY(UserId) REFERENCES dbo.Sys_Users(UserId),
  CONSTRAINT FK_ReportIndustryPermission_Industry FOREIGN KEY(IndustryId) REFERENCES dbo.Cate_BusinessIndustry(IndustryId)
 );
GO
IF OBJECT_ID('dbo.f_Report_IndustryUserScope') IS NULL
 EXEC('CREATE FUNCTION dbo.f_Report_IndustryUserScope(@UserName varchar(255)) RETURNS TABLE AS RETURN SELECT 1 AS Placeholder');
GO
ALTER FUNCTION dbo.f_Report_IndustryUserScope(@UserName varchar(255))
RETURNS TABLE AS RETURN (
 SELECT u.UserId,u.UserName,CONVERT(bit,CASE WHEN EXISTS(
  SELECT 1 FROM dbo.Report_UserBusinessIndustryPermissions p WHERE p.UserId=u.UserId) THEN 1 ELSE 0 END) IsRestricted
 FROM dbo.Sys_Users u WHERE u.UserName=@UserName AND u.IsActive=1 AND u.IsDeleted=0
);
GO
IF OBJECT_ID('dbo.f_Report_IndustryEnterprises') IS NULL
 EXEC('CREATE FUNCTION dbo.f_Report_IndustryEnterprises(@UserName varchar(255)) RETURNS TABLE AS RETURN SELECT 1 AS Placeholder');
GO
ALTER FUNCTION dbo.f_Report_IndustryEnterprises(@UserName varchar(255))
RETURNS TABLE AS RETURN (
 SELECT e.* FROM dbo.Cate_Enterprises e
 WHERE EXISTS(SELECT 1 FROM dbo.f_Report_IndustryUserScope(@UserName) u WHERE u.IsRestricted=0 OR EXISTS(
  SELECT 1 FROM dbo.Cate_BusinessEnterprise b
  CROSS APPLY dbo.fnSplit(b.IndustryIds,',') token
  INNER JOIN dbo.Report_UserBusinessIndustryPermissions p ON p.UserId=u.UserId AND p.IndustryId=TRY_CONVERT(int,LTRIM(RTRIM(token.Name)))
  WHERE b.EnterpriseId=e.EnterpriseId))
);
GO
-- Administrative operation only: do not grant this procedure to report viewers.
IF OBJECT_ID('dbo.p_Report_IndustryPermissions_Save') IS NULL
 EXEC('CREATE PROCEDURE dbo.p_Report_IndustryPermissions_Save AS RETURN 0');
GO
ALTER PROCEDURE dbo.p_Report_IndustryPermissions_Save
 @UserId int,@IndustryIds varchar(max),@SavedBy varchar(255)
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 SET @IndustryIds=LTRIM(RTRIM(ISNULL(@IndustryIds,'')));
 IF LEN(@IndustryIds)>20000 OR @IndustryIds LIKE ',%' OR @IndustryIds LIKE '%,' OR @IndustryIds LIKE '%,,%'
  THROW 50001,'Invalid comma-separated industry IDs.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.Sys_Users WHERE UserId=@UserId AND IsActive=1 AND IsDeleted=0)
  THROW 50002,'Target user is inactive or unknown.',1;
 IF NOT EXISTS(SELECT 1 FROM dbo.Sys_Users WHERE UserName=@SavedBy AND IsActive=1 AND IsDeleted=0)
  THROW 50003,'Audit user is inactive or unknown.',1;
 IF @IndustryIds<>'' AND EXISTS(SELECT 1 FROM dbo.fnSplit(@IndustryIds,',') token
  WHERE TRY_CONVERT(int,LTRIM(RTRIM(token.Name))) IS NULL OR NOT EXISTS(
   SELECT 1 FROM dbo.Cate_BusinessIndustry i WHERE i.IndustryId=TRY_CONVERT(int,LTRIM(RTRIM(token.Name))) AND i.IsActive=1 AND i.IsDeleted=0))
  THROW 50004,'One or more industry IDs are invalid or inactive.',1;
 BEGIN TRY
  BEGIN TRANSACTION;
  DECLARE @LockedUser int;
  SELECT @LockedUser=UserId FROM dbo.Sys_Users WITH(UPDLOCK,HOLDLOCK) WHERE UserId=@UserId;
  DELETE dbo.Report_UserBusinessIndustryPermissions WHERE UserId=@UserId;
  IF @IndustryIds<>'' INSERT dbo.Report_UserBusinessIndustryPermissions(UserId,IndustryId,SavedBy)
   SELECT DISTINCT @UserId,TRY_CONVERT(int,LTRIM(RTRIM(Name))),@SavedBy FROM dbo.fnSplit(@IndustryIds,',');
  COMMIT;
 END TRY
 BEGIN CATCH
  IF @@TRANCOUNT>0 ROLLBACK;
  THROW;
 END CATCH;
 RETURN @UserId;
END;
GO
