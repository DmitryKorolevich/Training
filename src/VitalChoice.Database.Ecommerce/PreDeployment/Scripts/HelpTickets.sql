IF OBJECT_ID(N'[dbo].[HelpTickets]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[HelpTickets] (
		[Id] [int] NOT NULL 
			CONSTRAINT PK_HelpTickets PRIMARY KEY (Id) IDENTITY,
		[IdOrder] [int] NOT NULL
			CONSTRAINT FK_HelpTicketsToOrders FOREIGN KEY (IdOrder) REFERENCES dbo.Orders (Id),
		[DateCreated] [datetime2] NOT NULL,
		[DateEdited] [datetime2] NOT NULL,
		[StatusCode] INT NOT NULL
			CONSTRAINT FK_HelpTicketsToStatus FOREIGN KEY (StatusCode) REFERENCES dbo.RecordStatusCodes (StatusCode),
		[Priority] INT NOT NULL,
		[Summary] NVARCHAR(250) NOT NULL,
		[Description] NVARCHAR(2000) NOT NULL
	)

	CREATE NONCLUSTERED INDEX IX_HelpTickets_DateCreated ON HelpTickets ([DateCreated])
	CREATE NONCLUSTERED INDEX IX_HelpTickets_DateEdited ON HelpTickets ([DateEdited])

	CREATE TABLE [dbo].[HelpTicketComments] (
		[Id] [int] NOT NULL 
			CONSTRAINT PK_HelpTicketComments PRIMARY KEY (Id) IDENTITY,
		[IdHelpTicket] [int] NOT NULL
			CONSTRAINT FK_HelpTicketCommentsToHelpTickets FOREIGN KEY (IdHelpTicket) REFERENCES dbo.HelpTickets (Id),
		[DateCreated] [datetime2] NOT NULL,
		[DateEdited] [datetime2] NOT NULL,
		[StatusCode] INT NOT NULL
			CONSTRAINT FK_HelpTicketCommentsToStatus FOREIGN KEY (StatusCode) REFERENCES dbo.RecordStatusCodes (StatusCode),
		[IdEditedBy] [int] NULL
			CONSTRAINT FK_HelpTicketCommentsToUsers FOREIGN KEY (IdEditedBy) REFERENCES dbo.Users (Id),
		[Order] INT NOT NULL,
		[Comment] NVARCHAR(2000) NOT NULL
	)
	
	CREATE NONCLUSTERED INDEX IX_HelpTicketComments_IdHelpTicket ON HelpTicketComments ([IdHelpTicket])
END