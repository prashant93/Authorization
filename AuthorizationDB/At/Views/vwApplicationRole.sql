













CREATE VIEW [At].[vwApplicationRole] AS

SELECT 
	approle.Id as RoleId,
	app.Id as ApplicationId,
	app.ApplicationName,
	app.ApplicationUrl,
	approle.RoleName
FROM AtM.tblRole approle
LEFT JOIN AtM.tblApplication app	
	ON app.Id = approle.ApplicatoinId and app.IsActive = 1

Where approle.IsActive = 1 











