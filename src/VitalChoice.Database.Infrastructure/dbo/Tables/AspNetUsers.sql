CREATE TABLE [dbo].[AspNetUsers] (
    [Id]                   NVARCHAR (450)     NOT NULL,
    [PublicId]             UNIQUEIDENTIFIER   NOT NULL,
    [AccessFailedCount]    INT                NOT NULL,
    [ConcurrencyStamp]     NVARCHAR (255)     NULL,
    [Email]                NVARCHAR (150)     NULL,
    [EmailConfirmed]       BIT                NOT NULL,
    [FirstName]            NVARCHAR (150)     NOT NULL,
    [LastName]             NVARCHAR (150)     NOT NULL,
    [LockoutEnabled]       BIT                NOT NULL,
    [LockoutEnd]           DATETIMEOFFSET (7) NULL,
    [NormalizedEmail]      NVARCHAR (150)     NULL,
    [NormalizedUserName]   NVARCHAR (150)     NULL,
    [PasswordHash]         NVARCHAR (255)     NULL,
    [PhoneNumber]          NVARCHAR (150)     NULL,
    [PhoneNumberConfirmed] BIT                NOT NULL,
    [SecurityStamp]        NVARCHAR (255)     NULL,
    [Status]               INT                NOT NULL,
    [TwoFactorEnabled]     BIT                NOT NULL,
    [UserName]             NVARCHAR (255)     NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AspNetUsers_AdminProfiles_Id] FOREIGN KEY ([Id]) REFERENCES [dbo].[AdminProfiles] ([Id])
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_AspNetUsers]
    ON [dbo].[AspNetUsers]([PublicId] ASC);

