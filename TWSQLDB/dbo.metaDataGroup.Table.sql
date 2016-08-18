USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[metaDataGroup]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[metaDataGroup](
	[ItemID] [int] NOT NULL,
	[ItemName] [nvarchar](200) NULL,
	[MetaTitle] [nvarchar](200) NULL,
	[MetaKeyword] [nvarchar](200) NULL,
	[MetaDescription] [nvarchar](300) NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[Updated] [int] NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_metaDataGroup_id] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
