-- Deploy in BaseApp after f_Chatbot_UserScope. No email/full-name/creator matching.
CREATE FUNCTION dbo.f_Chatbot_AuthorizedEnterprises(@UserId int,@ProvinceRoleIds varchar(500),@EnterpriseRoleIds varchar(500),@AllowProvince bit)
RETURNS TABLE AS RETURN (
 SELECT e.EnterpriseId
 FROM dbo.Cate_Enterprises e CROSS JOIN dbo.f_Chatbot_UserScope(@UserId,@ProvinceRoleIds,@EnterpriseRoleIds) s
 WHERE e.IsActive=1 AND e.IsDeleted=0
 AND (
 (@AllowProvince=1 AND s.CanReadProvince=1)
 OR (e.IsApproved=1 AND s.HasExplicitAssignments=1 AND EXISTS(
 SELECT 1 FROM dbo.Cate_EnterprisePermissions p WHERE p.ForUser=s.UserName AND p.EnterpriseId=e.EnterpriseId))
 OR (e.IsApproved=1 AND s.HasExplicitAssignments=0 AND s.HasEnterpriseRole=1 AND e.TaxCode=s.UserName)
 )
);
