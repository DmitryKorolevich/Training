DECLARE @customers_deleted TABLE (id INT)
INSERT INTO @customers_deleted
(id)
VALUES
(1066),
(1064),
(1065),
(1070),
(1074),
(1078),
(1069),
(1062),
(1063),
(1068),
(1067)
INSERT INTO @customers_deleted
(id)
SELECT id FROM AspNetUsers
WHERE id NOT IN (SELECT id FROM AdminProfiles)
BEGIN TRANSACTION
BEGIN TRY
	DELETE FROM AspNetUserClaims
	WHERE UserId IN (SELECT id FROM @customers_deleted)
	DELETE FROM AspNetUserLogins
	WHERE UserId IN (SELECT id FROM @customers_deleted)
	DELETE FROM AspNetUserRoles
	WHERE UserId IN (SELECT id FROM @customers_deleted)
	DELETE FROM AspNetUsers
	WHERE Id IN (SELECT id FROM @customers_deleted)
	COMMIT
END TRY
BEGIN CATCH
	ROLLBACK
	SELECT 
        ERROR_NUMBER() AS ErrorNumber,
        ERROR_MESSAGE() AS ErrorMessage;
END CATCH