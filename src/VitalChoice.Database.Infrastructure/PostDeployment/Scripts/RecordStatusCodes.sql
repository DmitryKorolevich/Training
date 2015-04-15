IF((SELECT count(*) FROM RecordStatusCodes
WHERE StatusCode IN (1,2,3))!=3)
BEGIN

INSERT INTO RecordStatusCodes
(StatusCode, Name)
SELECT 1, 'NotActive'
UNION
SELECT 2, 'Active'
UNION
SELECT 3, 'Deleted'

END