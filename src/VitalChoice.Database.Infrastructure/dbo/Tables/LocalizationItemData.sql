CREATE TABLE [dbo].[LocalizationItemData] (
    [GroupId]   INT            NOT NULL,
    [ItemId]    INT            NOT NULL,
    [CultureId] NVARCHAR (450) NOT NULL,
    [Value]     NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_LocalizationItemData] PRIMARY KEY CLUSTERED ([GroupId] ASC, [ItemId] ASC, [CultureId] ASC),
    CONSTRAINT [FK_LocalizationItemData_LocalizationItem_GroupId_ItemId] FOREIGN KEY ([GroupId], [ItemId]) REFERENCES [dbo].[LocalizationItem] ([GroupId], [ItemId])
);

