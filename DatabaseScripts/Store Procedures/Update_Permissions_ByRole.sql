IF EXISTS (
    SELECT *
FROM sysobjects
WHERE id = object_id(N'[Update_Permissions_ByRole]')
    and OBJECTPROPERTY(id, N'IsProcedure') = 1
) BEGIN
    DROP PROCEDURE [Update_Permissions_ByRole]
END
DROP TYPE IF EXISTS [dbo].[Permission]
CREATE TYPE [dbo].[Permission] AS TABLE(
    [RoleId] varchar(50) NOT NULL,
    [Function] varchar(50) NOT NULL,
    [Command] varchar(50) NOT NULL
)
GO
CREATE PROCEDURE [Update_Permissions_ByRole]
    @roleId VARCHAR(50) NULL,
    @permissions Permission readonly
AS
BEGIN
    SET XACT_ABORT ON;
    BEGIN TRAN
    BEGIN TRY
DELETE FROM [Identity].Permissions
where RoleId = @roleId;
INSERT INTO [Identity].Permissions
        (RoleId, [Function], [Command])
    SELECT [RoleId],
        [Function],
        [Command]
    FROM @permissions COMMIT
END TRY BEGIN CATCH ROLLBACK
DECLARE @ErrorMessage VARCHAR(2000)
SELECT @ErrorMessage = 'ERROR: ' + ERROR_MESSAGE() RAISERROR(@ErrorMessage, 16, 1)
END CATCH
END