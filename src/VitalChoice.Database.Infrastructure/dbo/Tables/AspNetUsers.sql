CREATE TABLE [dbo].[AspNetUsers] (
    [Id]                   NVARCHAR (450)     NOT NULL,
    [PublicId]             UNIQUEIDENTIFIER   NOT NULL,
    [AccessFailedCount]    INT                NOT NULL,
    [ConcurrencyStamp]     NVARCHAR (255)     NULL,
    [Email]                NVARCHAR (100)     NULL,
    [EmailConfirmed]       BIT                NOT NULL,
    [UserName]             NVARCHAR (100)     NULL,
    [FirstName]            NVARCHAR (100)     NOT NULL,
    [LastName]             NVARCHAR (100)     NOT NULL,
    [Status]               TINYINT            NOT NULL,
    [LockoutEnabled]       BIT                NOT NULL,
    [LockoutEnd]           DATETIMEOFFSET (7) NULL,
    [NormalizedEmail]      NVARCHAR (100)     NULL,
    [NormalizedUserName]   NVARCHAR (100)     NULL,
    [PasswordHash]         NVARCHAR (255)     NULL,
    [PhoneNumber]          NVARCHAR (100)     NULL,
    [PhoneNumberConfirmed] BIT                NOT NULL,
    [LastLoginDate]        DATETIME2 (7)      NULL,
    [CreateDate]           DATETIME2 (7)      NOT NULL,
    [UpdatedDate]          DATETIME2 (7)      NOT NULL,
    [DeletedDate]          DATETIME2 (7)      NULL,
    [SecurityStamp]        NVARCHAR (255)     NULL,
    [TwoFactorEnabled]     BIT                NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AspNetUsers_AdminProfiles_Id] FOREIGN KEY ([Id]) REFERENCES [dbo].[AdminProfiles] ([Id])
);






GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_AspNetUsers]
    ON [dbo].[AspNetUsers]([PublicId] ASC);

