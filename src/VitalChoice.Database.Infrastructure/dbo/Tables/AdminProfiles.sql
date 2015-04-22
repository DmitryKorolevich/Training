CREATE TABLE [dbo].[AdminProfiles] (
    [Id]                  INT              NOT NULL,
    [AgentId]             NVARCHAR (15)    NOT NULL,
    [ConfirmationToken]   UNIQUEIDENTIFIER NOT NULL,
    [TokenExpirationDate] DATETIME2 (7)    NOT NULL,
    [IsConfirmed]         BIT              NOT NULL,
    CONSTRAINT [PK_AdminProfiles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AdminProfiles_AspNetUsers_Id] FOREIGN KEY ([Id]) REFERENCES [dbo].[AspNetUsers] ([Id])
);







