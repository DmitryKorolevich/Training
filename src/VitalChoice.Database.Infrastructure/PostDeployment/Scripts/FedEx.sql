GO

IF OBJECT_ID(N'[dbo].[FedExZones]', N'U') IS NULL
BEGIN

CREATE TABLE [dbo].[FedExZones](
	[Id] [int] NOT NULL PRIMARY KEY,
	[Company] [nvarchar](250) NOT NULL,
	[Address] [nvarchar](250) NOT NULL,
	[City] [nvarchar](250) NOT NULL,
	[State] [nvarchar](10) NOT NULL,
	[Zip] [nvarchar](50) NOT NULL,
	[Phone] [nvarchar](250) NOT NULL,
	[Website] [nvarchar](250) NOT NULL,
	[Contact] [nvarchar](250) NOT NULL,
	[StatesCovered] [nvarchar](250) NOT NULL,
	[InUse] [bit] NOT NULL,
)

INSERT [dbo].[FedExZones] ([Id], [Company], [Address], [City], [State], [Zip], [Phone], [Website], [Contact], [StatesCovered], [InUse]) VALUES (1, N'Total Reclaim', N'5805 NE Columbia Boulevard', N'Portland', N'OR', N'97218', N'5037207689', N'www.totalreclaim.com', N'John Gogal', N'WA, OR, ID', 1)
INSERT [dbo].[FedExZones] ([Id], [Company], [Address], [City], [State], [Zip], [Phone], [Website], [Contact], [StatesCovered], [InUse]) VALUES (2, N'Foam Fabricators', N'1810 W. Santa Fe Ave', N'Compton', N'CA', N'90221', N'3105375760', N'www.foamfabricatorsinc.com', N'Bill Lopes', N'CA, AZ', 1)
INSERT [dbo].[FedExZones] ([Id], [Company], [Address], [City], [State], [Zip], [Phone], [Website], [Contact], [StatesCovered], [InUse]) VALUES (3, N'Sonoco', N'1100 Garden of The Gods Road', N'Colorado Springs', N'CO', N'80907', N'719-598-0602', N'', N'', N'CO, MT,  WY, NV, UT', 1)
INSERT [dbo].[FedExZones] ([Id], [Company], [Address], [City], [State], [Zip], [Phone], [Website], [Contact], [StatesCovered], [InUse]) VALUES (4, N'Foam Fabricators', N'417 W. Industrial Dr.', N'El Dorado Springs', N'MO', N'64744', N'4178766880', N'www.foamfabricatorsinc.com', N'Greg Castor', N'MO, KS, NE, LA, NM, SD, ND, MN', 1)
INSERT [dbo].[FedExZones] ([Id], [Company], [Address], [City], [State], [Zip], [Phone], [Website], [Contact], [StatesCovered], [InUse]) VALUES (5, N'Foam Fabricators', N'900 E. Keller Parkway, Suite 101- Fabrication Plant', N'Keller', N'TX', N'76248', N'8176230034', N'www.foamfabricatorsinc.com', N'Stacey Webb', N'TX, OK', 1)
INSERT [dbo].[FedExZones] ([Id], [Company], [Address], [City], [State], [Zip], [Phone], [Website], [Contact], [StatesCovered], [InUse]) VALUES (6, N'Foam Fabricators', N'950 Progress Blv.', N'New Albany', N'IN', N'47150', N'8129481696', N'', N'', N'IA, IL, WI, IN, MI', 1)
INSERT [dbo].[FedExZones] ([Id], [Company], [Address], [City], [State], [Zip], [Phone], [Website], [Contact], [StatesCovered], [InUse]) VALUES (7, N'Foam Fabricators', N'24 Colleg Pk. Cove', N'Jackson', N'TN', N'38301', N'7314233161', N'www.foamfabricatorsinc.com', N'Brett Rodgers', N'TN, AL, MS, WV, AR, KY', 1)
INSERT [dbo].[FedExZones] ([Id], [Company], [Address], [City], [State], [Zip], [Phone], [Website], [Contact], [StatesCovered], [InUse]) VALUES (8, N'Foam Fabricators', N'82 Galilee Church Rd', N'Jefferson', N'GA', N'30549', N'7063672616', N'www.foamfabricatorsinc.com', N'Dimitrius Collins', N'GA, NC, SC, VA', 1)
INSERT [dbo].[FedExZones] ([Id], [Company], [Address], [City], [State], [Zip], [Phone], [Website], [Contact], [StatesCovered], [InUse]) VALUES (9, N'Foam Fabricators', N'6550 W. 26th St', N'Erie', N'PA', N'16506', N'8148384538', N'www.foamfabricatorsinc.com', N'Mark Sabolcik', N'PA, OH', 1)
INSERT [dbo].[FedExZones] ([Id], [Company], [Address], [City], [State], [Zip], [Phone], [Website], [Contact], [StatesCovered], [InUse]) VALUES (10, N'Foam Fabricators Inc.', N'24 College Park Cove', N'Jackson', N'TN', N'38301', N'7314233161', N'www.foampackindustries.com', N'Brett Rodgers', N'FL', 1)
INSERT [dbo].[FedExZones] ([Id], [Company], [Address], [City], [State], [Zip], [Phone], [Website], [Contact], [StatesCovered], [InUse]) VALUES (11, N'Foam Pack', N'72 Fadem Rd.', N'Springfield', N'NJ', N'07081', N'9733763700', N'www.foampackindustries.com', N'Harvey Goodstein', N'NJ, NY, NH, CT, VT, MA, ME, MD, DC, DE, RI', 1)
INSERT [dbo].[FedExZones] ([Id], [Company], [Address], [City], [State], [Zip], [Phone], [Website], [Contact], [StatesCovered], [InUse]) VALUES (12, N'Pacific Allied Products', N'91-110 Kaomi Loop', N'Kapolei', N'HI', N'96707', N'8087928530', N'www.pacificalliedproducts.com', N'Bernie Coleman', N'HI', 1)
INSERT [dbo].[FedExZones] ([Id], [Company], [Address], [City], [State], [Zip], [Phone], [Website], [Contact], [StatesCovered], [InUse]) VALUES (13, N'Alaska', N'', N'', N'Ak', N'', N'', N'', N'', N'AK', 1)

END

GO

IF OBJECT_ID(N'[dbo].[VitalGreenRequests]', N'U') IS NULL
BEGIN

CREATE TABLE [dbo].[VitalGreenRequests](
	[Id] [int] NOT NULL PRIMARY KEY IDENTITY,
	[FirstName] [nvarchar](250) NULL,
	[LastName] [nvarchar](250) NULL,
	[Email] [nvarchar](250) NULL,
	[ZoneId] [int] NULL,
	[DateView] DATETIME2 NULL,
	[DateCompleted] DATETIME2 NULL,
	[Address] [nvarchar](250) NULL,
	[Address2] [nvarchar](250) NULL,
	[City] [nvarchar](250) NULL,
	[State] [nvarchar](10) NULL,
	[Zip] [nvarchar](50) NULL,
	CONSTRAINT [FK_VitalGreenRequest_ToFedExZone] FOREIGN KEY ([ZoneId]) REFERENCES [FedExZones]([Id]), 
)

END

GO