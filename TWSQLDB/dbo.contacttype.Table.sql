USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[contacttype]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[contacttype](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ContactTypeName] [nvarchar](10) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](10) NULL,
	[InDate] [datetime] NOT NULL,
	[InUser] [nvarchar](10) NOT NULL,
 CONSTRAINT [PK_ContactType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
