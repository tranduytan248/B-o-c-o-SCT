-- Deploy in BaseApp. Account/role/permission state is read fresh; caller parameters are server-owned.
CREATE FUNCTION dbo.f_Chatbot_UserScope(@UserId int,@ProvinceRoleIds varchar(500),@EnterpriseRoleIds varchar(500))
RETURNS TABLE AS RETURN (
SELECT Flags.*,
 CAST(CASE WHEN HasProvinceRole=1 AND HasEnterpriseRole=0 AND HasExplicitAssignments=0 AND EXISTS(
 SELECT 1 FROM dbo.Sys_UserRoles ur JOIN dbo.Sys_Roles r ON r.RoleId=ur.RoleId AND r.IsDeleted=0
 JOIN dbo.Sys_Permissions p ON p.RoleId=ur.RoleId JOIN dbo.Sys_Functions f ON f.FunctionId=p.FunctionId AND f.IsDeleted=0
 WHERE ur.UserId=Flags.UserId AND f.Area='Report' AND f.Name='Dashboard' AND p.Action='View') THEN 1 ELSE 0 END AS bit) CanReadProvince,
 CAST(CASE WHEN HasEnterpriseRole=0 AND EXISTS(
 SELECT 1 FROM dbo.Sys_UserRoles ur JOIN dbo.Sys_Roles r ON r.RoleId=ur.RoleId AND r.IsDeleted=0
 JOIN dbo.Sys_Permissions p ON p.RoleId=ur.RoleId JOIN dbo.Sys_Functions f ON f.FunctionId=p.FunctionId AND f.IsDeleted=0
 WHERE ur.UserId=Flags.UserId AND f.Area='Report' AND f.Name='Dashboard' AND p.Action='View') THEN 1 ELSE 0 END AS bit) CanReadDashboard,
 CAST(CASE WHEN EXISTS(
 SELECT 1 FROM dbo.Sys_UserRoles ur JOIN dbo.Sys_Roles r ON r.RoleId=ur.RoleId AND r.IsDeleted=0
 JOIN dbo.Sys_Permissions p ON p.RoleId=ur.RoleId JOIN dbo.Sys_Functions f ON f.FunctionId=p.FunctionId AND f.IsDeleted=0
 WHERE ur.UserId=Flags.UserId AND f.Area='Report' AND f.Name='Import' AND p.Action='View') THEN 1 ELSE 0 END AS bit) CanReadImports,
 CAST(CASE WHEN HasEnterpriseRole=0 AND EXISTS(
 SELECT 1 FROM dbo.Sys_UserRoles ur JOIN dbo.Sys_Roles r ON r.RoleId=ur.RoleId AND r.IsDeleted=0
 JOIN dbo.Sys_Permissions p ON p.RoleId=ur.RoleId JOIN dbo.Sys_Functions f ON f.FunctionId=p.FunctionId AND f.IsDeleted=0
 WHERE ur.UserId=Flags.UserId AND f.Area='Cate' AND f.Name='Enterprise' AND p.Action='View') THEN 1 ELSE 0 END AS bit) CanReadRegistryDetail
 FROM (
 SELECT u.*,
 CAST(CASE WHEN EXISTS(SELECT 1 FROM dbo.Sys_UserRoles ur JOIN dbo.Sys_Roles r ON r.RoleId=ur.RoleId AND r.IsDeleted=0
 WHERE ur.UserId=u.UserId AND CHARINDEX(','+CONVERT(varchar(11),ur.RoleId)+',',','+ISNULL(@EnterpriseRoleIds,'')+',')>0) THEN 1 ELSE 0 END AS bit) HasEnterpriseRole,
 CAST(CASE WHEN EXISTS(SELECT 1 FROM dbo.Sys_UserRoles ur JOIN dbo.Sys_Roles r ON r.RoleId=ur.RoleId AND r.IsDeleted=0
 WHERE ur.UserId=u.UserId AND CHARINDEX(','+CONVERT(varchar(11),ur.RoleId)+',',','+ISNULL(@ProvinceRoleIds,'')+',')>0) THEN 1 ELSE 0 END AS bit) HasProvinceRole,
 CAST(CASE WHEN EXISTS(SELECT 1 FROM dbo.Cate_EnterprisePermissions p WHERE p.ForUser=u.UserName) THEN 1 ELSE 0 END AS bit) HasExplicitAssignments
 FROM (
 SELECT u.UserId,u.UserName FROM dbo.Sys_Users u
 WHERE u.UserId=@UserId AND u.IsActive=1 AND u.IsDeleted=0 AND ISNULL(u.IsLocked,0)=0
) u
) Flags

);
