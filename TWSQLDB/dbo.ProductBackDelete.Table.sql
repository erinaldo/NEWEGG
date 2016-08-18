USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ProductBackDelete]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProductBackDelete](
	[ID] [int] NOT NULL,
	[FK] [int] NOT NULL,
	[SourceTable] [nvarchar](150) NOT NULL,
	[SellerProductID] [varchar](150) NULL,
	[Name] [nvarchar](2000) NOT NULL,
	[NameTW] [nvarchar](2000) NULL,
	[Description] [nvarchar](max) NULL,
	[DescriptionTW] [nvarchar](max) NULL,
	[SPEC] [nvarchar](4000) NULL,
	[ManufactureID] [int] NOT NULL,
	[Model] [varchar](30) NULL,
	[BarCode] [varchar](50) NULL,
	[SellerID] [int] NOT NULL,
	[DelvType] [int] NULL,
	[PicStart] [int] NULL,
	[PicEnd] [int] NULL,
	[Cost] [decimal](10, 2) NULL,
	[Status] [int] NOT NULL,
	[InvoiceType] [int] NULL,
	[SaleType] [int] NULL,
	[Length] [decimal](10, 2) NULL,
	[Width] [decimal](10, 2) NULL,
	[Height] [decimal](10, 2) NULL,
	[Weight] [decimal](12, 4) NULL,
	[TradeTax] [decimal](10, 5) NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Updated] [int] NOT NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[Tax] [decimal](10, 5) NULL,
	[Warranty] [int] NULL,
	[UPC] [varchar](15) NULL,
	[Note] [nvarchar](500) NULL,
	[IsMarket] [char](1) NULL,
	[Is18] [char](1) NULL,
	[IsShipDanger] [char](1) NULL,
	[IsChokingDanger] [char](1) NULL,
	[MenufacturePartNum] [varchar](15) NULL,
	[SPECLabel] [nvarchar](4000) NULL,
	[SupplyShippingCharge] [decimal](10, 5) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
