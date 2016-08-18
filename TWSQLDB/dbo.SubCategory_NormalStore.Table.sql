USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[SubCategory_NormalStore]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubCategory_NormalStore](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](50) NULL,
	[CategoryID] [int] NULL,
	[StoreClass] [int] NULL,
	[StyleClass] [int] NULL,
	[ShowAll] [int] NULL,
	[Showorder] [int] NULL,
	[LogoImageURL] [nvarchar](500) NULL,
	[StoreImageURL] [nvarchar](500) NULL,
	[StoreImageLinkURL] [nvarchar](500) NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL CONSTRAINT [DF_SubCategory_NormalStore_CreateDate]  DEFAULT (getdate()),
	[Updated] [int] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_SubCategory_NormalStore] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
