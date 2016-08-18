USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[itemsearch]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[itemsearch](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](200) NULL,
	[Sdesc] [nvarchar](500) NULL,
	[Saletype] [int] NULL,
	[PayType] [int] NULL,
	[DateStart] [datetime] NULL,
	[DateEnd] [datetime] NULL,
	[Pricesgst] [decimal](10, 2) NULL,
	[Pricecash] [decimal](10, 2) NULL,
	[SellingQty] [int] NULL,
	[Qtyreg] [int] NULL,
	[PhotoName] [nvarchar](50) NULL,
	[Showorder] [int] NULL,
	[CategoryID] [int] NULL,
	[CategoryLayer] [int] NULL,
	[CategoryTitle] [nvarchar](50) NULL,
	[ManufactureID] [int] NULL,
	[ManufactureName] [nvarchar](500) NULL,
	[ProductModel] [varchar](30) NULL,
	[ItemUpdateDate] [datetime] NULL,
	[ItemSpechead] [nvarchar](30) NULL,
	[ItemHtmlName] [nvarchar](50) NULL,
	[ProductName] [nvarchar](200) NULL,
	[SellerProductID] [varchar](150) NULL,
	[SellerID] [int] NULL,
	[SellerName] [nvarchar](50) NULL,
	[CountryID] [int] NULL,
	[PriceGlobalship] [decimal](10, 2) NULL,
 CONSTRAINT [PK_itemsearch_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
