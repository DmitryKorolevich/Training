INSERT INTO RecordStatusCodes
(StatusCode, Name)
SELECT 'N', 'NotActive'
UNION
SELECT 'A', 'Active'
UNION
SELECT 'D', 'Deleted'