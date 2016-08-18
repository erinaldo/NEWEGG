USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ItemAndSubCategoryMapping_NormalStore]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemAndSubCategoryMapping_NormalStore](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SubCategoryID] [int] NOT NULL,
	[Zone] [int] NOT NULL,
	[StyleClass] [int] NOT NULL,
	[Showorder] [int] NOT NULL,
	[ItemID] [int] NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
	[ImageURL] [nvarchar](500) NULL,
	[LinkURL] [nvarchar](500) NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL CONSTRAINT [DF_ItemAndSubCategoryMapping_NormalStore_CreateDate]  DEFAULT (getdate()),
	[Updated] [int] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[Discription] [nvarchar](30) NULL,
 CONSTRAINT [PK_ItemAndSubCategoryMapping_NormalStore] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
