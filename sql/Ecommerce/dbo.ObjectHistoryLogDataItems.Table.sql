/****** Object:  Table [dbo].[ObjectHistoryLogDataItems]    Script Date: 6/25/2016 2:11:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ObjectHistoryLogDataItems]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ObjectHistoryLogDataItems](
	[IdObjectHistoryLogDataItem] [bigint] IDENTITY(1,1) NOT NULL,
	[Data] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ObjectHistoryLogDataItems] PRIMARY KEY CLUSTERED 
(
	[IdObjectHistoryLogDataItem] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
END
GO
