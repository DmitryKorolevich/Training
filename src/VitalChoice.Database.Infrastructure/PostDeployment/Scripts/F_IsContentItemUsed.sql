DROP FUNCTION [dbo].[IsContentItemUsed]
GO

CREATE FUNCTION IsContentItemUsed
(
	@contentItemId INT
)
RETURNS bit
AS
BEGIN
	DECLARE @exist bit

	SET @exist =(SELECT TOP 1 Id FROM Recipes
				WHERE ContentItemId=@contentItemId)
	IF(@exist IS NULL)
	BEGIN
		SET @exist =(SELECT TOP 1 Id FROM ContentCategories
					WHERE ContentItemId=@contentItemId)
	END
		IF(@exist IS NULL)
	BEGIN
		SET @exist =(SELECT TOP 1 Id FROM ContentPages
					WHERE ContentItemId=@contentItemId)
	END

	IF(@exist IS NULL)
	BEGIN
		SET @exist=0
	END
	ELSE
	BEGIN
		SET @exist=1
	END
	RETURN @exist

END
GO

