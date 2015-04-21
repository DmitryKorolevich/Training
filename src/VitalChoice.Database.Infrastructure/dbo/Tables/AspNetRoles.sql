CREATE TABLE [dbo].[AspNetRoles] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [ConcurrencyStamp] NVARCHAR (255) NULL,
    [Name]             NVARCHAR (100) NOT NULL,
    [NormalizedName]   NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
);



