USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[GLAccounts]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GLAccounts](
	[AccNumber] [varchar](10) NOT NULL,
	[AccDescription] [nvarchar](50) NOT NULL,
	[UseFlag] [char](1) NOT NULL DEFAULT ((1)),
 CONSTRAINT [PK_GLAccounts_1] PRIMARY KEY CLUSTERED 
(
	[AccNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
