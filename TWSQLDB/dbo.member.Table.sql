USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[member]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[member](
	[AccID] [int] NOT NULL,
	[Sex] [int] NULL,
	[Firstname] [nvarchar](20) NULL,
	[Lastname] [nvarchar](20) NULL,
	[Nickname] [nvarchar](30) NULL,
	[Firstname_en] [nvarchar](30) NULL,
	[Lastname_en] [nvarchar](30) NULL,
	[Birthday] [char](10) NULL,
	[Mobile] [varchar](30) NULL,
	[TelZip] [varchar](4) NULL,
	[TelDay] [varchar](30) NULL,
	[TelExtension] [varchar](10) NULL,
	[Loc] [nvarchar](10) NULL,
	[Zip] [char](5) NULL,
	[Zipname] [nvarchar](10) NULL,
	[Address] [nvarchar](150) NULL,
	[Address_en] [nvarchar](300) NULL,
	[CreateDate] [datetime] NULL,
	[ModifyDate] [datetime] NULL,
 CONSTRAINT [PK_member] PRIMARY KEY CLUSTERED 
(
	[AccID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
