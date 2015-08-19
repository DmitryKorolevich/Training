IF OBJECT_ID(N'dbo.MasterContentItems', N'U') IS NULL
BEGIN

PRINT N'Creating [dbo].[AdminProfiles]...';



CREATE TABLE [dbo].[AdminProfiles] (
    [Id]                  INT              NOT NULL,
    [AgentId]             NVARCHAR (15)    NOT NULL,
    [ConfirmationToken]   UNIQUEIDENTIFIER NOT NULL,
    [TokenExpirationDate] DATETIME2 (7)    NOT NULL,
    [IsConfirmed]         BIT              NOT NULL,
    CONSTRAINT [PK_AdminProfiles] PRIMARY KEY CLUSTERED ([Id] ASC)
);



PRINT N'Creating [dbo].[Articles]...';


CREATE TABLE [dbo].[Articles] (
    [Id]                  INT            IDENTITY (1, 1) NOT NULL,
    [Url]                 NVARCHAR (250) NOT NULL,
    [Name]                NVARCHAR (250) NOT NULL,
    [FileUrl]             NVARCHAR (250) NULL,
    [ContentItemId]       INT            NOT NULL,
    [MasterContentItemId] INT            NOT NULL,
    [StatusCode]          INT            NOT NULL,
    [PublishedDate]       DATETIME       NULL,
    [SubTitle]            NVARCHAR (250) NULL,
    [Author]              NVARCHAR (250) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



PRINT N'Creating [dbo].[ArticlesToContentCategories]...';



CREATE TABLE [dbo].[ArticlesToContentCategories] (
    [ArticleId]         INT NOT NULL,
    [ContentCategoryId] INT NOT NULL,
    [Id]                INT IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_ArticlesToContentCategories] PRIMARY KEY CLUSTERED ([Id] ASC)
);



PRINT N'Creating [dbo].[ArticlesToContentCategories].[IX_ArticlesToContentCategories_ContentCategoryId]...';



CREATE NONCLUSTERED INDEX [IX_ArticlesToContentCategories_ContentCategoryId]
    ON [dbo].[ArticlesToContentCategories]([ContentCategoryId] ASC) WITH (FILLFACTOR = 80);



PRINT N'Creating [dbo].[AspNetRoleClaims]...';



CREATE TABLE [dbo].[AspNetRoleClaims] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [ClaimType]  NVARCHAR (255) NOT NULL,
    [ClaimValue] NVARCHAR (255) NOT NULL,
    [RoleId]     INT            NOT NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED ([Id] ASC)
);



PRINT N'Creating [dbo].[AspNetRoles]...';



CREATE TABLE [dbo].[AspNetRoles] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [ConcurrencyStamp] NVARCHAR (255) NULL,
    [Name]             NVARCHAR (100) NOT NULL,
    [NormalizedName]   NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
);



PRINT N'Creating [dbo].[AspNetUserClaims]...';



CREATE TABLE [dbo].[AspNetUserClaims] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [ClaimType]  NVARCHAR (255) NOT NULL,
    [ClaimValue] NVARCHAR (255) NOT NULL,
    [UserId]     INT            NOT NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED ([Id] ASC)
);



PRINT N'Creating [dbo].[AspNetUserLogins]...';



CREATE TABLE [dbo].[AspNetUserLogins] (
    [LoginProvider]       NVARCHAR (450) NOT NULL,
    [ProviderKey]         NVARCHAR (450) NOT NULL,
    [ProviderDisplayName] NVARCHAR (255) NULL,
    [UserId]              INT            NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED ([LoginProvider] ASC, [ProviderKey] ASC)
);



PRINT N'Creating [dbo].[AspNetUserRoles]...';



CREATE TABLE [dbo].[AspNetUserRoles] (
    [UserId] INT NOT NULL,
    [RoleId] INT NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC)
);



PRINT N'Creating [dbo].[AspNetUsers]...';



CREATE TABLE [dbo].[AspNetUsers] (
    [Id]                   INT                IDENTITY (1, 1) NOT NULL,
    [PublicId]             UNIQUEIDENTIFIER   NOT NULL,
    [AccessFailedCount]    INT                NOT NULL,
    [ConcurrencyStamp]     NVARCHAR (255)     NULL,
    [Email]                NVARCHAR (100)     NOT NULL,
    [EmailConfirmed]       BIT                NOT NULL,
    [UserName]             NVARCHAR (100)     NULL,
    [FirstName]            NVARCHAR (100)     NOT NULL,
    [LastName]             NVARCHAR (100)     NOT NULL,
    [Status]               TINYINT            NOT NULL,
    [LockoutEnabled]       BIT                NOT NULL,
    [LockoutEnd]           DATETIMEOFFSET (7) NULL,
    [NormalizedEmail]      NVARCHAR (100)     NOT NULL,
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
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);



PRINT N'Creating [dbo].[AspNetUsers].[IX_AspNetUsers]...';



CREATE UNIQUE NONCLUSTERED INDEX [IX_AspNetUsers]
    ON [dbo].[AspNetUsers]([PublicId] ASC);



PRINT N'Creating [dbo].[ContentCategories]...';



CREATE TABLE [dbo].[ContentCategories] (
    [Id]                  INT            IDENTITY (1, 1) NOT NULL,
    [ContentItemId]       INT            NOT NULL,
    [MasterContentItemId] INT            NOT NULL,
    [Name]                NVARCHAR (250) NOT NULL,
    [FileUrl]             NVARCHAR (250) NULL,
    [ParentId]            INT            NULL,
    [StatusCode]          INT            NOT NULL,
    [Url]                 NVARCHAR (250) NOT NULL,
    [Type]                INT            NOT NULL,
    [Order]               INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



PRINT N'Creating [dbo].[ContentCategories].[IX_ContentCategories_Url]...';



CREATE NONCLUSTERED INDEX [IX_ContentCategories_Url]
    ON [dbo].[ContentCategories]([Url] ASC) WITH (FILLFACTOR = 80);



PRINT N'Creating [dbo].[ContentCategories].[IX_ContentCategories_MasterContentItemId]...';



CREATE NONCLUSTERED INDEX [IX_ContentCategories_MasterContentItemId]
    ON [dbo].[ContentCategories]([MasterContentItemId] ASC) WITH (FILLFACTOR = 80);



PRINT N'Creating [dbo].[ContentCategories].[IX_ContentCategories_ParentId]...';



CREATE NONCLUSTERED INDEX [IX_ContentCategories_ParentId]
    ON [dbo].[ContentCategories]([ParentId] ASC) WITH (FILLFACTOR = 80);



PRINT N'Creating [dbo].[ContentItems]...';



CREATE TABLE [dbo].[ContentItems] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [Created]         DATETIME       NOT NULL,
    [Updated]         DATETIME       NOT NULL,
    [Template]        NVARCHAR (MAX) NOT NULL,
    [Description]     NVARCHAR (MAX) NOT NULL,
    [Title]           NVARCHAR (250) NULL,
    [MetaKeywords]    NVARCHAR (250) NULL,
    [MetaDescription] NVARCHAR (250) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



PRINT N'Creating [dbo].[ContentItemsToContentProcessors]...';



CREATE TABLE [dbo].[ContentItemsToContentProcessors] (
    [ContentItemId]      INT NOT NULL,
    [ContentItemProcessorId] INT NOT NULL,
    [Id]                 INT IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_ContentItemsToContentProcessors] PRIMARY KEY CLUSTERED ([Id] ASC)
);



PRINT N'Creating [dbo].[ContentItemsToContentProcessors].[IX_ContentItemsToContentProcessors_ContentItemId]...';



CREATE NONCLUSTERED INDEX [IX_ContentItemsToContentProcessors_ContentItemId]
    ON [dbo].[ContentItemsToContentProcessors]([ContentItemId] ASC) WITH (FILLFACTOR = 80);



PRINT N'Creating [dbo].[ContentPages]...';



CREATE TABLE [dbo].[ContentPages] (
    [Id]                  INT            IDENTITY (1, 1) NOT NULL,
    [Url]                 NVARCHAR (250) NOT NULL,
    [Name]                NVARCHAR (250) NOT NULL,
    [FileUrl]             NVARCHAR (250) NULL,
    [ContentItemId]       INT            NOT NULL,
    [MasterContentItemId] INT            NOT NULL,
    [StatusCode]          INT            NOT NULL,
    [Assigned]            INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



PRINT N'Creating [dbo].[ContentPages].[IX_ContentPages_Url]...';



CREATE NONCLUSTERED INDEX [IX_ContentPages_Url]
    ON [dbo].[ContentPages]([Url] ASC) WITH (FILLFACTOR = 80);



PRINT N'Creating [dbo].[ContentPagesToContentCategories]...';



CREATE TABLE [dbo].[ContentPagesToContentCategories] (
    [ContentPageId]     INT NOT NULL,
    [ContentCategoryId] INT NOT NULL,
    [Id]                INT IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_ContentPagesToContentCategories] PRIMARY KEY CLUSTERED ([Id] ASC)
);



PRINT N'Creating [dbo].[ContentPagesToContentCategories].[IX_ContentPagesToContentCategories_ContentCategoryId]...';



CREATE NONCLUSTERED INDEX [IX_ContentPagesToContentCategories_ContentCategoryId]
    ON [dbo].[ContentPagesToContentCategories]([ContentCategoryId] ASC) WITH (FILLFACTOR = 80);



PRINT N'Creating [dbo].[ContentProcessors]...';



CREATE TABLE [dbo].[ContentProcessors] (
    [Id]          INT            NOT NULL,
    [Type]        NVARCHAR (250) NOT NULL,
    [Name]        NVARCHAR (250) NOT NULL,
    [Description] NVARCHAR (250) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



PRINT N'Creating [dbo].[ContentTypes]...';



CREATE TABLE [dbo].[ContentTypes] (
    [Id]                         INT            NOT NULL,
    [Name]                       NVARCHAR (250) NOT NULL,
    [DefaultMasterContentItemId] INT            NULL,
    CONSTRAINT [PK_ContentTypes] PRIMARY KEY CLUSTERED ([Id] ASC)
);



PRINT N'Creating [dbo].[FAQs]...';



CREATE TABLE [dbo].[FAQs] (
    [Id]                  INT            IDENTITY (1, 1) NOT NULL,
    [Url]                 NVARCHAR (250) NOT NULL,
    [Name]                NVARCHAR (250) NOT NULL,
    [FileUrl]             NVARCHAR (250) NULL,
    [ContentItemId]       INT            NOT NULL,
    [MasterContentItemId] INT            NOT NULL,
    [StatusCode]          INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



PRINT N'Creating [dbo].[FAQs].[IX_FAQs_Url]...';



CREATE NONCLUSTERED INDEX [IX_FAQs_Url]
    ON [dbo].[FAQs]([Url] ASC) WITH (FILLFACTOR = 80);



PRINT N'Creating [dbo].[FAQs].[IX_FAQs_MasterContentItemId]...';



CREATE NONCLUSTERED INDEX [IX_FAQs_MasterContentItemId]
    ON [dbo].[FAQs]([MasterContentItemId] ASC) WITH (FILLFACTOR = 80);



PRINT N'Creating [dbo].[FAQsToContentCategories]...';



CREATE TABLE [dbo].[FAQsToContentCategories] (
    [FAQId]             INT NOT NULL,
    [ContentCategoryId] INT NOT NULL,
    [Id]                INT IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_FAQsToContentCategories] PRIMARY KEY CLUSTERED ([Id] ASC)
);



PRINT N'Creating [dbo].[FAQsToContentCategories].[IX_FAQsToContentCategories_ContentCategoryId]...';



CREATE NONCLUSTERED INDEX [IX_FAQsToContentCategories_ContentCategoryId]
    ON [dbo].[FAQsToContentCategories]([ContentCategoryId] ASC) WITH (FILLFACTOR = 80);



PRINT N'Creating [dbo].[LocalizationItem]...';



CREATE TABLE [dbo].[LocalizationItem] (
    [GroupId]   INT            NOT NULL,
    [ItemId]    INT            NOT NULL,
    [Comment]   NVARCHAR (MAX) NOT NULL,
    [GroupName] NVARCHAR (MAX) NOT NULL,
    [ItemName]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_LocalizationItem] PRIMARY KEY CLUSTERED ([GroupId] ASC, [ItemId] ASC)
);



PRINT N'Creating [dbo].[LocalizationItemData]...';



CREATE TABLE [dbo].[LocalizationItemData] (
    [GroupId]   INT            NOT NULL,
    [ItemId]    INT            NOT NULL,
    [CultureId] NVARCHAR (450) NOT NULL,
    [Value]     NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_LocalizationItemData] PRIMARY KEY CLUSTERED ([GroupId] ASC, [ItemId] ASC, [CultureId] ASC)
);



PRINT N'Creating [dbo].[MasterContentItems]...';



CREATE TABLE [dbo].[MasterContentItems] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (250) NOT NULL,
    [TypeId]     INT            NOT NULL,
    [Template]   NVARCHAR (MAX) NOT NULL,
    [Created]    DATETIME       NOT NULL,
    [Updated]    DATETIME       NOT NULL,
    [StatusCode] INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



PRINT N'Creating [dbo].[MasterContentItemsToContentProcessors]...';



CREATE TABLE [dbo].[MasterContentItemsToContentProcessors] (
    [MasterContentItemId] INT NOT NULL,
    [ContentProcessorId]  INT NOT NULL,
    [Id]                  INT IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_MasterContentItemsToContentProcessors] PRIMARY KEY CLUSTERED ([Id] ASC)
);



PRINT N'Creating [dbo].[MasterContentItemsToContentProcessors].[IX_ContentItemsToContentProcessors_MasterContentItemId]...';



CREATE NONCLUSTERED INDEX [IX_ContentItemsToContentProcessors_MasterContentItemId]
    ON [dbo].[MasterContentItemsToContentProcessors]([MasterContentItemId] ASC) WITH (FILLFACTOR = 80);



PRINT N'Creating [dbo].[Recipes]...';



CREATE TABLE [dbo].[Recipes] (
    [Id]                  INT            IDENTITY (1, 1) NOT NULL,
    [Url]                 NVARCHAR (250) NOT NULL,
    [Name]                NVARCHAR (250) NOT NULL,
    [FileUrl]             NVARCHAR (250) NULL,
    [ContentItemId]       INT            NOT NULL,
    [MasterContentItemId] INT            NOT NULL,
    [StatusCode]          INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



PRINT N'Creating [dbo].[Recipes].[IX_Articles_Url]...';



CREATE NONCLUSTERED INDEX [IX_Articles_Url]
    ON [dbo].[Recipes]([Url] ASC) WITH (FILLFACTOR = 80);



PRINT N'Creating [dbo].[Recipes].[IX_Articles_MasterContentItemId]...';



CREATE NONCLUSTERED INDEX [IX_Articles_MasterContentItemId]
    ON [dbo].[Recipes]([MasterContentItemId] ASC) WITH (FILLFACTOR = 80);



PRINT N'Creating [dbo].[Recipes].[IX_ContentPages_MasterContentItemId]...';



CREATE NONCLUSTERED INDEX [IX_ContentPages_MasterContentItemId]
    ON [dbo].[Recipes]([MasterContentItemId] ASC) WITH (FILLFACTOR = 80);



PRINT N'Creating [dbo].[Recipes].[IX_Recipes_Url]...';



CREATE NONCLUSTERED INDEX [IX_Recipes_Url]
    ON [dbo].[Recipes]([Url] ASC) WITH (FILLFACTOR = 80);



PRINT N'Creating [dbo].[Recipes].[IX_Recipes_MasterContentItemId]...';



CREATE NONCLUSTERED INDEX [IX_Recipes_MasterContentItemId]
    ON [dbo].[Recipes]([MasterContentItemId] ASC) WITH (FILLFACTOR = 80);



PRINT N'Creating [dbo].[RecipesToContentCategories]...';



CREATE TABLE [dbo].[RecipesToContentCategories] (
    [RecipeId]          INT NOT NULL,
    [ContentCategoryId] INT NOT NULL,
    [Id]                INT IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_RecipesToContentCategories] PRIMARY KEY CLUSTERED ([Id] ASC)
);



PRINT N'Creating [dbo].[RecipesToContentCategories].[IX_RecipesToContentCategories_ContentCategoryId]...';



CREATE NONCLUSTERED INDEX [IX_RecipesToContentCategories_ContentCategoryId]
    ON [dbo].[RecipesToContentCategories]([ContentCategoryId] ASC) WITH (FILLFACTOR = 80);



PRINT N'Creating [dbo].[RecordStatusCodes]...';



CREATE TABLE [dbo].[RecordStatusCodes] (
    [StatusCode] INT           NOT NULL,
    [Name]       NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([StatusCode] ASC)
);



PRINT N'Creating unnamed constraint on [dbo].[Articles]...';



ALTER TABLE [dbo].[Articles]
    ADD DEFAULT 1 FOR [StatusCode];



PRINT N'Creating unnamed constraint on [dbo].[ContentCategories]...';



ALTER TABLE [dbo].[ContentCategories]
    ADD DEFAULT 1 FOR [StatusCode];



PRINT N'Creating unnamed constraint on [dbo].[ContentItems]...';



ALTER TABLE [dbo].[ContentItems]
    ADD DEFAULT GETDATE() FOR [Created];



PRINT N'Creating unnamed constraint on [dbo].[ContentItems]...';



ALTER TABLE [dbo].[ContentItems]
    ADD DEFAULT GETDATE() FOR [Updated];



PRINT N'Creating unnamed constraint on [dbo].[ContentPages]...';



ALTER TABLE [dbo].[ContentPages]
    ADD DEFAULT 1 FOR [StatusCode];



PRINT N'Creating unnamed constraint on [dbo].[ContentPages]...';



ALTER TABLE [dbo].[ContentPages]
    ADD DEFAULT 1 FOR [Assigned];



PRINT N'Creating unnamed constraint on [dbo].[FAQs]...';



ALTER TABLE [dbo].[FAQs]
    ADD DEFAULT 1 FOR [StatusCode];



PRINT N'Creating unnamed constraint on [dbo].[MasterContentItems]...';



ALTER TABLE [dbo].[MasterContentItems]
    ADD DEFAULT GETDATE() FOR [Created];



PRINT N'Creating unnamed constraint on [dbo].[MasterContentItems]...';



ALTER TABLE [dbo].[MasterContentItems]
    ADD DEFAULT GETDATE() FOR [Updated];



PRINT N'Creating unnamed constraint on [dbo].[MasterContentItems]...';



ALTER TABLE [dbo].[MasterContentItems]
    ADD DEFAULT 1 FOR [StatusCode];



PRINT N'Creating unnamed constraint on [dbo].[Recipes]...';



ALTER TABLE [dbo].[Recipes]
    ADD DEFAULT 1 FOR [StatusCode];



PRINT N'Creating [dbo].[FK_AdminProfiles_AspNetUsers_Id]...';



ALTER TABLE [dbo].[AdminProfiles]
    ADD CONSTRAINT [FK_AdminProfiles_AspNetUsers_Id] FOREIGN KEY ([Id]) REFERENCES [dbo].[AspNetUsers] ([Id]);



PRINT N'Creating [dbo].[FK_Articles_ToContentItem]...';



ALTER TABLE [dbo].[Articles]
    ADD CONSTRAINT [FK_Articles_ToContentItem] FOREIGN KEY ([ContentItemId]) REFERENCES [dbo].[ContentItems] ([Id]);



PRINT N'Creating [dbo].[FK_Articles_ToMasterContentItem]...';



ALTER TABLE [dbo].[Articles]
    ADD CONSTRAINT [FK_Articles_ToMasterContentItem] FOREIGN KEY ([MasterContentItemId]) REFERENCES [dbo].[MasterContentItems] ([Id]);



PRINT N'Creating [dbo].[FK_Articles_ToRecordStatusCode]...';



ALTER TABLE [dbo].[Articles]
    ADD CONSTRAINT [FK_Articles_ToRecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [dbo].[RecordStatusCodes] ([StatusCode]);



PRINT N'Creating [dbo].[FK_ArticlesToContentCategories_Articles]...';



ALTER TABLE [dbo].[ArticlesToContentCategories]
    ADD CONSTRAINT [FK_ArticlesToContentCategories_Articles] FOREIGN KEY ([ArticleId]) REFERENCES [dbo].[Articles] ([Id]) ON DELETE CASCADE;



PRINT N'Creating [dbo].[FK_ArticlesToContentCategories_ContentCategories]...';



ALTER TABLE [dbo].[ArticlesToContentCategories]
    ADD CONSTRAINT [FK_ArticlesToContentCategories_ContentCategories] FOREIGN KEY ([ContentCategoryId]) REFERENCES [dbo].[ContentCategories] ([Id]) ON DELETE CASCADE;



PRINT N'Creating [dbo].[FK_AspNetRoleClaims_AspNetRoles_RoleId]...';



ALTER TABLE [dbo].[AspNetRoleClaims]
    ADD CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]);



PRINT N'Creating [dbo].[FK_AspNetUserClaims_AspNetUsers_UserId]...';



ALTER TABLE [dbo].[AspNetUserClaims]
    ADD CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]);



PRINT N'Creating [dbo].[FK_AspNetUserLogins_AspNetUsers_UserId]...';



ALTER TABLE [dbo].[AspNetUserLogins]
    ADD CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]);



PRINT N'Creating [dbo].[FK_AspNetUserRoles_AspNetRoles_RoleId]...';



ALTER TABLE [dbo].[AspNetUserRoles]
    ADD CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]);



PRINT N'Creating [dbo].[FK_AspNetUserRoles_AspNetUsers_UserId]...';



ALTER TABLE [dbo].[AspNetUserRoles]
    ADD CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]);



PRINT N'Creating [dbo].[FK_ContentCategories_ToContentCategory]...';



ALTER TABLE [dbo].[ContentCategories]
    ADD CONSTRAINT [FK_ContentCategories_ToContentCategory] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[ContentCategories] ([Id]);



PRINT N'Creating [dbo].[FK_ContentCategories_ToMasterContentItem]...';



ALTER TABLE [dbo].[ContentCategories]
    ADD CONSTRAINT [FK_ContentCategories_ToMasterContentItem] FOREIGN KEY ([MasterContentItemId]) REFERENCES [dbo].[MasterContentItems] ([Id]);



PRINT N'Creating [dbo].[FK_ContentCategories_ToContentItem]...';



ALTER TABLE [dbo].[ContentCategories]
    ADD CONSTRAINT [FK_ContentCategories_ToContentItem] FOREIGN KEY ([ContentItemId]) REFERENCES [dbo].[ContentItems] ([Id]);



PRINT N'Creating [dbo].[FK_ContentCategories_ToRecordStatusCode]...';



ALTER TABLE [dbo].[ContentCategories]
    ADD CONSTRAINT [FK_ContentCategories_ToRecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [dbo].[RecordStatusCodes] ([StatusCode]);



PRINT N'Creating [dbo].[FK_ContentCategories_ToContentTypes]...';



ALTER TABLE [dbo].[ContentCategories]
    ADD CONSTRAINT [FK_ContentCategories_ToContentTypes] FOREIGN KEY ([Type]) REFERENCES [dbo].[ContentTypes] ([Id]);



PRINT N'Creating [dbo].[FK_ContentItemsToContentProcessors_ContentItems]...';



ALTER TABLE [dbo].[ContentItemsToContentProcessors]
    ADD CONSTRAINT [FK_ContentItemsToContentProcessors_ContentItems] FOREIGN KEY ([ContentItemId]) REFERENCES [dbo].[ContentItems] ([Id]) ON DELETE CASCADE;



PRINT N'Creating [dbo].[FK_ContentItemsToContentProcessors_ContentProcessors]...';



ALTER TABLE [dbo].[ContentItemsToContentProcessors]
    ADD CONSTRAINT [FK_ContentItemsToContentProcessors_ContentProcessors] FOREIGN KEY ([ContentProcessorId]) REFERENCES [dbo].[ContentProcessors] ([Id]) ON DELETE CASCADE;



PRINT N'Creating [dbo].[FK_ContentPages_ToContentItem]...';



ALTER TABLE [dbo].[ContentPages]
    ADD CONSTRAINT [FK_ContentPages_ToContentItem] FOREIGN KEY ([ContentItemId]) REFERENCES [dbo].[ContentItems] ([Id]);



PRINT N'Creating [dbo].[FK_ContentPages_ToMasterContentItem]...';



ALTER TABLE [dbo].[ContentPages]
    ADD CONSTRAINT [FK_ContentPages_ToMasterContentItem] FOREIGN KEY ([MasterContentItemId]) REFERENCES [dbo].[MasterContentItems] ([Id]);



PRINT N'Creating [dbo].[FK_ContentPages_ToRecordStatusCode]...';



ALTER TABLE [dbo].[ContentPages]
    ADD CONSTRAINT [FK_ContentPages_ToRecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [dbo].[RecordStatusCodes] ([StatusCode]);



PRINT N'Creating [dbo].[FK_ContentPagesToContentCategories_Recipes]...';



ALTER TABLE [dbo].[ContentPagesToContentCategories]
    ADD CONSTRAINT [FK_ContentPagesToContentCategories_Recipes] FOREIGN KEY ([ContentPageId]) REFERENCES [dbo].[ContentPages] ([Id]) ON DELETE CASCADE;



PRINT N'Creating [dbo].[FK_ContentPagesToContentCategories_ContentCategories]...';



ALTER TABLE [dbo].[ContentPagesToContentCategories]
    ADD CONSTRAINT [FK_ContentPagesToContentCategories_ContentCategories] FOREIGN KEY ([ContentCategoryId]) REFERENCES [dbo].[ContentCategories] ([Id]) ON DELETE CASCADE;



PRINT N'Creating [dbo].[FK_FAQs_ToContentItem]...';



ALTER TABLE [dbo].[FAQs]
    ADD CONSTRAINT [FK_FAQs_ToContentItem] FOREIGN KEY ([ContentItemId]) REFERENCES [dbo].[ContentItems] ([Id]);



PRINT N'Creating [dbo].[FK_FAQs_ToMasterContentItem]...';



ALTER TABLE [dbo].[FAQs]
    ADD CONSTRAINT [FK_FAQs_ToMasterContentItem] FOREIGN KEY ([MasterContentItemId]) REFERENCES [dbo].[MasterContentItems] ([Id]);



PRINT N'Creating [dbo].[FK_FAQs_ToRecordStatusCode]...';



ALTER TABLE [dbo].[FAQs]
    ADD CONSTRAINT [FK_FAQs_ToRecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [dbo].[RecordStatusCodes] ([StatusCode]);



PRINT N'Creating [dbo].[FK_FAQsToContentCategories_FAQ]...';



ALTER TABLE [dbo].[FAQsToContentCategories]
    ADD CONSTRAINT [FK_FAQsToContentCategories_FAQ] FOREIGN KEY ([FAQId]) REFERENCES [dbo].[FAQs] ([Id]) ON DELETE CASCADE;



PRINT N'Creating [dbo].[FK_FAQsToContentCategories_ContentCategories]...';



ALTER TABLE [dbo].[FAQsToContentCategories]
    ADD CONSTRAINT [FK_FAQsToContentCategories_ContentCategories] FOREIGN KEY ([ContentCategoryId]) REFERENCES [dbo].[ContentCategories] ([Id]) ON DELETE CASCADE;



PRINT N'Creating [dbo].[FK_LocalizationItemData_LocalizationItem_GroupId_ItemId]...';



ALTER TABLE [dbo].[LocalizationItemData]
    ADD CONSTRAINT [FK_LocalizationItemData_LocalizationItem_GroupId_ItemId] FOREIGN KEY ([GroupId], [ItemId]) REFERENCES [dbo].[LocalizationItem] ([GroupId], [ItemId]);



PRINT N'Creating [dbo].[FK_MasterContentItems_ToContentTypes]...';



ALTER TABLE [dbo].[MasterContentItems]
    ADD CONSTRAINT [FK_MasterContentItems_ToContentTypes] FOREIGN KEY ([TypeId]) REFERENCES [dbo].[ContentTypes] ([Id]);



PRINT N'Creating [dbo].[FK_MasterContentItemsToContentProcessors_MasterContentItems]...';



ALTER TABLE [dbo].[MasterContentItemsToContentProcessors]
    ADD CONSTRAINT [FK_MasterContentItemsToContentProcessors_MasterContentItems] FOREIGN KEY ([MasterContentItemId]) REFERENCES [dbo].[MasterContentItems] ([Id]) ON DELETE CASCADE;



PRINT N'Creating [dbo].[FK_MasterContentItemsToContentProcessors_ContentProcessors]...';



ALTER TABLE [dbo].[MasterContentItemsToContentProcessors]
    ADD CONSTRAINT [FK_MasterContentItemsToContentProcessors_ContentProcessors] FOREIGN KEY ([ContentProcessorId]) REFERENCES [dbo].[ContentProcessors] ([Id]) ON DELETE CASCADE;



PRINT N'Creating [dbo].[FK_Recipes_ToContentItem]...';



ALTER TABLE [dbo].[Recipes]
    ADD CONSTRAINT [FK_Recipes_ToContentItem] FOREIGN KEY ([ContentItemId]) REFERENCES [dbo].[ContentItems] ([Id]);



PRINT N'Creating [dbo].[FK_Recipes_ToMasterContentItem]...';



ALTER TABLE [dbo].[Recipes]
    ADD CONSTRAINT [FK_Recipes_ToMasterContentItem] FOREIGN KEY ([MasterContentItemId]) REFERENCES [dbo].[MasterContentItems] ([Id]);



PRINT N'Creating [dbo].[FK_Recipes_ToRecordStatusCode]...';



ALTER TABLE [dbo].[Recipes]
    ADD CONSTRAINT [FK_Recipes_ToRecordStatusCode] FOREIGN KEY ([StatusCode]) REFERENCES [dbo].[RecordStatusCodes] ([StatusCode]);



PRINT N'Creating [dbo].[FK_RecipesToContentCategories_Recipes]...';



ALTER TABLE [dbo].[RecipesToContentCategories]
    ADD CONSTRAINT [FK_RecipesToContentCategories_Recipes] FOREIGN KEY ([RecipeId]) REFERENCES [dbo].[Recipes] ([Id]) ON DELETE CASCADE;



PRINT N'Creating [dbo].[FK_RecipesToContentCategories_ContentCategories]...';



ALTER TABLE [dbo].[RecipesToContentCategories]
    ADD CONSTRAINT [FK_RecipesToContentCategories_ContentCategories] FOREIGN KEY ([ContentCategoryId]) REFERENCES [dbo].[ContentCategories] ([Id]) ON DELETE CASCADE;


END

GO

PRINT N'Creating [dbo].[TG_ContentItems_Update]...';

GO

IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'TG_ContentItems_Update' AND type = 'TR')
   DROP TRIGGER TG_ContentItems_Update
GO
CREATE TRIGGER TG_ContentItems_Update
   ON ContentItems 
   AFTER UPDATE
AS 
BEGIN
	UPDATE ContentItems
	SET Updated=GETDATE()
	WHERE Id IN
	(SELECT Id FROM inserted)
END

PRINT N'Creating [dbo].[TG_MasterContentItems_Update]...';

GO

IF EXISTS (SELECT name FROM sysobjects
      WHERE name = 'TG_MasterContentItems_Update' AND type = 'TR')
   DROP TRIGGER TG_MasterContentItems_Update
GO
CREATE TRIGGER TG_MasterContentItems_Update
   ON MasterContentItems 
   AFTER UPDATE
AS 
BEGIN
	UPDATE MasterContentItems
	SET Updated=GETDATE()
	WHERE Id IN
	(SELECT Id FROM inserted)
END