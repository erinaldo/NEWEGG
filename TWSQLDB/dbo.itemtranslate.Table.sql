USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[itemtranslate]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[itemtranslate](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ItemID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[DescriptionTW] [nvarchar](max) NULL,
	[HasError] [char](1) NOT NULL CONSTRAINT [DF_Translate_HasError]  DEFAULT ('N'),
	[Status] [int] NOT NULL CONSTRAINT [DF_Translate_Status]  DEFAULT ((0)),
	[ItemName] [nvarchar](200) NULL,
	[CategoryL1] [nvarchar](100) NULL,
	[CategoryIDL1] [int] NULL,
	[CategoryL2] [nvarchar](100) NULL,
	[CategoryIDL2] [int] NULL,
	[CategoryL3] [nvarchar](100) NULL,
	[CategoryIDL3] [int] NULL,
	[CreateUser] [varchar](20) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[LastEditUser] [varchar](20) NULL,
	[LastEditDate] [datetime] NULL,
	[ApproveUser] [varchar](20) NULL,
 CONSTRAINT [PK_Translate] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
