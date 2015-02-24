CREATE TABLE [dbo].[Comment] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [CreationDate] DATETIME2 (7)  NOT NULL,
    [Text]         NVARCHAR (MAX) NULL,
    [AuthorId]     NVARCHAR (450) NULL,
    CONSTRAINT [PK_Comment] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Comment_AspNetUsers_AuthorId] FOREIGN KEY ([AuthorId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);

