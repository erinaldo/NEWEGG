USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[product]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[product](
	[ID] [int] IDENTITY(1,1) NOT NULL,
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
	[PicStart] [int] NULL CONSTRAINT [DF_product_PicStart]  DEFAULT ((0)),
	[PicEnd] [int] NULL CONSTRAINT [DF_product_PicEnd]  DEFAULT ((0)),
	[Cost] [decimal](10, 2) NULL CONSTRAINT [DF_product_product_cost]  DEFAULT ((0.00)),
	[Status] [int] NOT NULL CONSTRAINT [DF_product_status]  DEFAULT ((0)),
	[InvoiceType] [int] NULL CONSTRAINT [DF_product_InvoiceType]  DEFAULT ((0)),
	[SaleType] [int] NULL CONSTRAINT [DF_product_SaleType]  DEFAULT ((0)),
	[Length] [decimal](10, 2) NULL CONSTRAINT [DF_product_Length]  DEFAULT ((0)),
	[Width] [decimal](10, 2) NULL CONSTRAINT [DF_product_Width]  DEFAULT ((0)),
	[Height] [decimal](10, 2) NULL CONSTRAINT [DF_product_Height]  DEFAULT ((0)),
	[Weight] [decimal](12, 4) NULL CONSTRAINT [DF_product_Weight]  DEFAULT ((0)),
	[TradeTax] [decimal](10, 5) NULL CONSTRAINT [DF_product_product_tradetax]  DEFAULT ((0.00)),
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_product_product_createdate]  DEFAULT (getdate()),
	[Updated] [int] NOT NULL CONSTRAINT [DF_product_product_updated]  DEFAULT ((0)),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[Tax] [decimal](10, 5) NULL CONSTRAINT [DF_product_Tax]  DEFAULT ((0)),
	[Warranty] [int] NULL CONSTRAINT [DF_product_Warranty]  DEFAULT ((0)),
	[UPC] [varchar](15) NULL,
	[Note] [nvarchar](500) NULL,
	[IsMarket] [char](1) NULL CONSTRAINT [DF_product_IsMarket]  DEFAULT ('N'),
	[Is18] [char](1) NULL CONSTRAINT [DF_product_Is18]  DEFAULT ('N'),
	[IsShipDanger] [char](1) NULL CONSTRAINT [DF_product_IsShipDanger]  DEFAULT ('N'),
	[IsChokingDanger] [char](1) NULL CONSTRAINT [DF_product_IsChokingDanger]  DEFAULT ('N'),
	[MenufacturePartNum] [varchar](15) NULL,
	[SPECLabel] [nvarchar](4000) NULL,
	[SupplyShippingCharge] [decimal](10, 5) NULL,
 CONSTRAINT [PK_product_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
