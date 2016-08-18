USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[activity]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[activity](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[HtmlContext] [nvarchar](max) NULL,
	[ShowType] [int] NOT NULL CONSTRAINT [DF_activity_activity_showtype]  DEFAULT ((1)),
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_activity_activity_createdate]  DEFAULT (getdate()),
	[Updated] [int] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[ActionType] [int] NULL,
	[MetaTitle] [nvarchar](1024) NULL,
	[MetaKeyword] [nvarchar](1024) NULL,
	[MetaDescription] [nvarchar](1024) NULL,
	[SectionInfor] [nvarchar](200) NULL,
 CONSTRAINT [PK_activity_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
