/****** Object:  Table [dbo].[AdminTeams]    Script Date: 6/25/2016 3:39:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AdminTeams]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AdminTeams](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_AdminTeams] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
