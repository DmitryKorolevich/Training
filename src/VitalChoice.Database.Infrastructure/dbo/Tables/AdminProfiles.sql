CREATE TABLE [dbo].[AdminProfiles] (
    [Id]      NVARCHAR (450) NOT NULL,
    [AgentId] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_AdminProfiles] PRIMARY KEY CLUSTERED ([Id] ASC)
);

