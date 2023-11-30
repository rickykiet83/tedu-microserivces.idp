IF EXISTS (
	SELECT *
FROM sysobjects
WHERE id = object_id(N'[Create_Permission]')
	and OBJECTPROPERTY(id, N'IsProcedure') = 1
) BEGIN
	DROP PROCEDURE [Create_Permission]
END
GO
CREATE PROCEDURE [Create_Permission]
	@roleId VARCHAR(50) NULL,
	@function varchar(50) NULL,
	@command varchar(50) NULL,
	@newID bigint OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON;
	SET XACT_ABORT ON;
	BEGIN TRAN
	BEGIN TRY IF NOT EXISTS (
	SELECT *
	FROM [Identity].Permissions
	WHERE [RoleId] = @roleId
		AND [Function] = @function
		AND [Command] = @command
) BEGIN
		INSERT INTO [Identity].Permissions
			([RoleId], [Function], [Command])
		VALUES
			(@roleId, @function, @command)
		SET @newID = SCOPE_IDENTITY();
	END COMMIT
END TRY BEGIN CATCH ROLLBACK
DECLARE @ErrorMessage VARCHAR(2000)
SELECT @ErrorMessage = 'ERROR: ' + ERROR_MESSAGE() RAISERROR(@ErrorMessage, 16, 1)
END CATCH
END