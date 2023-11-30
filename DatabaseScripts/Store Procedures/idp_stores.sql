IF EXISTS ( SELECT *
FROM sysobjects
WHERE  id = object_id(N'[Create_Permission]')
	and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
BEGIN
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
	BEGIN TRY

	IF NOT EXISTS (SELECT *
	FROM [Identity].Permissions
	WHERE [RoleId] = @roleId AND
		[Function] = @function AND
		[Command] = @command)
   	BEGIN
		INSERT INTO [Identity].Permissions
			([RoleId], [Function], [Command])
		VALUES
			(@roleId, @function, @command)
		SET @newID = SCOPE_IDENTITY();
	END

	COMMIT
	END TRY
	BEGIN CATCH
		ROLLBACK
		  DECLARE @ErrorMessage VARCHAR(2000)
		  SELECT @ErrorMessage = 'ERROR: ' + ERROR_MESSAGE()
		  RAISERROR(@ErrorMessage, 16, 1)
	END CATCH
END

GO

IF EXISTS ( SELECT *
FROM sysobjects
WHERE  id = object_id(N'[dbo].[Delete_Permission]')
	and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
BEGIN
	DROP PROCEDURE [dbo].[Delete_Permission]
END

GO

CREATE PROCEDURE [dbo].[Delete_Permission]
	@roleId varchar(50),
	@function varchar(50),
	@command varchar(50)
AS
BEGIN
	DELETE
    FROM [Identity].Permissions
    WHERE [RoleId] = @roleId
		AND [Function] = @function
		AND [Command] = @command
END

GO

IF EXISTS (
	SELECT *
FROM sysobjects
WHERE id = object_id(N'[Get_Permission_ByRoleId]')
	and OBJECTPROPERTY(id, N'IsProcedure') = 1
) BEGIN
	DROP PROCEDURE [Get_Permission_ByRoleId]
END

GO

CREATE PROCEDURE [Get_Permission_ByRoleId]
	@roleId varchar(50) null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT *
	FROM [Identity].Permissions
	WHERE RoleId = @roleId
END

GO

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

GO
