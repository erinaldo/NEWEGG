USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ItemWarrantyBlackList]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemWarrantyBlackList](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Itemid] [int] NOT NULL,
	[Desc] [int] NOT NULL,
	[ProductProperty] [int] NOT NULL,
	[Update] [date] NULL,
	[Updated] [int] NULL,
	[UpdateUser] [nchar](10) NULL,
 CONSTRAINT [PK_ItemWarrantyBlackList] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
