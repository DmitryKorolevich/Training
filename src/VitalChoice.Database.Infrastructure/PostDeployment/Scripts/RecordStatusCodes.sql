DELETE RecordStatusCodes 

INSERT INTO RecordStatusCodes
(StatusCode, Name)
SELECT 1, 'NotActive'
UNION
SELECT 2, 'Active'
UNION
SELECT 3, 'Deleted'