CREATE TABLE [dbo].[LocalizationItem] (
    [GroupId]   INT            NOT NULL,
    [ItemId]    INT            NOT NULL,
    [Comment]   NVARCHAR (MAX) NOT NULL,
    [GroupName] NVARCHAR (MAX) NOT NULL,
    [ItemName]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_LocalizationItem] PRIMARY KEY CLUSTERED ([GroupId] ASC, [ItemId] ASC)
);

