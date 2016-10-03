











CREATE VIEW [At].[vwApplicationPermission] AS

SELECT 
	app.Id as ApplicationId,
	app.ApplicationName,
	g.Id as GroupId,
	g.GroupName,
	u.Id as UserId,
	u.UserName,
	acc.Id as RoleID,
	acc.RoleName,
	permission.IsActive
FROM AtM.tblApplicationPermission permission
LEFT JOIN AtM.tblApplication app	
	ON app.Id = permission.ApplicationId and app.IsActive = 1
LEFT JOIN AtM.tblGroup g	
	ON g.Id = permission.GroupId and g.IsActive = 1
LEFT JOIN AtM.tblUser u	
	ON u.Id = permission.UserId and u.IsActive = 1
LEFT JOIN AtM.tblRole acc	
	ON acc.Id = permission.RoleId and acc.IsActive = 1
Where permission.IsActive = 1 and app.IsActive = 1









