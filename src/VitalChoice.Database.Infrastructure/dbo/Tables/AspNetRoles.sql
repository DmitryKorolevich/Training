CREATE TABLE [dbo].[AspNetRoles] (
    [Id]               NVARCHAR (450) NOT NULL,
    [ConcurrencyStamp] NVARCHAR (MAX) NULL,
    [Name]             NVARCHAR (MAX) NULL,
    [NormalizedName]   NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
);

