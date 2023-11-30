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