USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[PMPageInfo]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PMPageInfo](
	[PageID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[Description] [nvarchar](500) NULL,
	[Path] [nvarchar](200) NOT NULL,
	[ParentID] [int] NULL,
	[PageOrder] [int] NULL,
	[Status] [char](1) NOT NULL,
	[Width] [int] NULL,
	[Height] [int] NULL,
	[BackgroundImg] [varchar](200) NULL,
	[InUser] [nvarchar](100) NOT NULL,
	[InDate] [datetime] NOT NULL,
	[LastEditUser] [nvarchar](100) NULL,
	[LastEditDate] [datetime] NULL,
 CONSTRAINT [PK_PageInfo_1] PRIMARY KEY CLUSTERED 
(
	[PageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
