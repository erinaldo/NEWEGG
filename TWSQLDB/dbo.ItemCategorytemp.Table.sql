USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ItemCategorytemp]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ItemCategorytemp](
	[itemtempID] [int] NOT NULL,
	[ItemID] [int] NULL,
	[CategoryID] [int] NOT NULL,
	[FromSystem] [char](1) NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_ItemCategorytemp_CreateDate]  DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_ItemCategorytemp_UpdateDate]  DEFAULT (getdate()),
 CONSTRAINT [PK_ItemCategorytemp_id] PRIMARY KEY CLUSTERED 
(
	[itemtempID] ASC,
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
