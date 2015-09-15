IF OBJECT_ID(N'[dbo].[BugTickets]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[BugTickets] (
		[Id] [int] NOT NULL 
			CONSTRAINT PK_BugTickets PRIMARY KEY (Id) IDENTITY,
		[DateCreated] [datetime2] NOT NULL,
		[DateEdited] [datetime2] NOT NULL,
		[IdAddedBy] [int] NOT NULL
			CONSTRAINT FK_BugTicketsAddedByToAspNetUserss FOREIGN KEY (IdAddedBy) REFERENCES dbo.AspNetUsers (Id),
		[IdEditedBy] [int] NOT NULL
			CONSTRAINT FK_BugTicketsEditedByToAspNetUserss FOREIGN KEY (IdEditedBy) REFERENCES dbo.AspNetUsers (Id),
		[StatusCode] INT NOT NULL
			CONSTRAINT FK_BugTicketsToStatus FOREIGN KEY (StatusCode) REFERENCES dbo.RecordStatusCodes (StatusCode),
		[Priority] INT NOT NULL,
		[Summary] NVARCHAR(250) NOT NULL,
		[Description] NVARCHAR(2000) NOT NULL,
		[PublicId] UNIQUEIDENTIFIER NOT NULL,
	)

	ALTER TABLE [dbo].[BugTickets]
	ADD CONSTRAINT UQ_BugTickets_PublicId UNIQUE(PublicId)

	CREATE NONCLUSTERED INDEX IX_BugTickets_DateCreated ON BugTickets ([DateCreated])
	CREATE NONCLUSTERED INDEX IX_BugTickets_DateEdited ON BugTickets ([DateEdited])

	CREATE TABLE [dbo].[BugTicketComments] (
		[Id] [int] NOT NULL 
			CONSTRAINT PK_BugTicketComments PRIMARY KEY (Id) IDENTITY,
		[IdBugTicket] [int] NOT NULL
			CONSTRAINT FK_BugTicketCommentsToBugTickets FOREIGN KEY (IdBugTicket) REFERENCES dbo.BugTickets (Id),
		[DateCreated] [datetime2] NOT NULL,
		[DateEdited] [datetime2] NOT NULL,
		[StatusCode] INT NOT NULL
			CONSTRAINT FK_BugTicketCommentsToStatus FOREIGN KEY (StatusCode) REFERENCES dbo.RecordStatusCodes (StatusCode),
		[IdEditedBy] [int] NOT NULL
			CONSTRAINT FK_BugTicketCommentsToAspNetUserss FOREIGN KEY (IdEditedBy) REFERENCES dbo.AspNetUsers (Id),
		[Order] INT NOT NULL,
		[Comment] NVARCHAR(2000) NOT NULL,
		[PublicId] UNIQUEIDENTIFIER NOT NULL,
	)

	ALTER TABLE [dbo].[BugTickets]
	ADD CONSTRAINT UQ_BugTicketComments_PublicId UNIQUE(PublicId)
	
	CREATE NONCLUSTERED INDEX IX_BugTicketComments_IdBugTicket ON BugTicketComments ([IdBugTicket])
END

IF OBJECT_ID(N'[dbo].[BugFiles]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[BugFiles](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[IdBugTicket] [int] NULL,
		[IdBugTicketComment] [int] NULL,
		[UploadDate] [datetime2](7) NOT NULL,
		[FileName] [nvarchar](250) NOT NULL,
		[Description] [nvarchar](500) NOT NULL
	 CONSTRAINT [PK_BugFiles] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
	) ON [PRIMARY]


	ALTER TABLE [dbo].[BugFiles]  WITH CHECK ADD CONSTRAINT [FK_BugFiles_BugTickets] FOREIGN KEY([IdBugTicket])
	REFERENCES [dbo].[BugTickets] ([Id])

	ALTER TABLE [dbo].[BugFiles] CHECK CONSTRAINT [FK_BugFiles_BugTickets]

	ALTER TABLE [dbo].[BugFiles]  WITH CHECK ADD  CONSTRAINT [FK_BugFiles_BugTicketComments] FOREIGN KEY([IdBugTicketComment])
	REFERENCES [dbo].[BugTicketComments] ([Id])

	ALTER TABLE [dbo].[BugFiles] CHECK CONSTRAINT [FK_BugFiles_BugTicketComments]

END