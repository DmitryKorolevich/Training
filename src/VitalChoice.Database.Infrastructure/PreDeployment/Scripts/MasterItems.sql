IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = 'IdEditedBy' AND [object_id] = OBJECT_ID(N'[dbo].[MasterContentItems]', N'U'))
BEGIN
	
	EXEC sp_RENAME 'MasterContentItems.UserId' , 'IdEditedBy', 'COLUMN'
END

GO