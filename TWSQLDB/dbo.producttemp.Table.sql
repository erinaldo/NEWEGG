USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[producttemp]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[producttemp](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NULL,
	[SellerProductID] [varchar](150) NULL,
	[Name] [nvarchar](4000) NOT NULL,
	[NameTW] [nvarchar](4000) NULL,
	[Description] [nvarchar](max) NULL,
	[DescriptionTW] [nvarchar](max) NULL,
	[SPEC] [nvarchar](4000) NULL,
	[ManufactureID] [int] NOT NULL,
	[Model] [varchar](300) NULL,
	[BarCode] [varchar](500) NULL,
	[SellerID] [int] NOT NULL,
	[DelvType] [int] NULL,
	[PicStart] [int] NULL CONSTRAINT [DF_producttemp_PicStart]  DEFAULT ((0)),
	[PicEnd] [int] NULL CONSTRAINT [DF_producttemp_PicEnd]  DEFAULT ((0)),
	[Cost] [decimal](10, 2) NULL,
	[Status] [int] NOT NULL CONSTRAINT [DF_producttemp_status]  DEFAULT ((0)),
	[InvoiceType] [int] NULL CONSTRAINT [DF_producttemp_InvoiceType]  DEFAULT ((0)),
	[SaleType] [int] NULL CONSTRAINT [DF_producttemp_SaleType]  DEFAULT ((0)),
	[Length] [decimal](10, 2) NULL CONSTRAINT [DF_producttemp_Length]  DEFAULT ((0)),
	[Width] [decimal](10, 2) NULL CONSTRAINT [DF_producttemp_Width]  DEFAULT ((0)),
	[Height] [decimal](10, 2) NULL CONSTRAINT [DF_producttemp_Height]  DEFAULT ((0)),
	[Weight] [decimal](10, 2) NULL CONSTRAINT [DF_producttemp_Weight]  DEFAULT ((0)),
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_producttemp_producttemp_createdate]  DEFAULT (getdate()),
	[Updated] [int] NOT NULL CONSTRAINT [DF_producttemp_producttemp_updated]  DEFAULT ((0)),
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[TradeTax] [decimal](10, 2) NULL CONSTRAINT [DF_producttemp_TradeTax]  DEFAULT ((0)),
	[Tax] [decimal](10, 2) NULL CONSTRAINT [DF_producttemp_Tax]  DEFAULT ((0)),
	[Warranty] [int] NULL CONSTRAINT [DF_producttemp_Warranty]  DEFAULT ((0)),
	[UPC] [varchar](15) NULL,
	[Note] [nvarchar](500) NULL,
	[IsMarket] [char](1) NULL CONSTRAINT [DF_producttemp_IsMarket]  DEFAULT ('N'),
	[Is18] [char](1) NULL CONSTRAINT [DF_producttemp_Is18]  DEFAULT ('N'),
	[IsShipDanger] [char](1) NULL CONSTRAINT [DF_producttemp_IsShipDanger]  DEFAULT ('N'),
	[IsChokingDanger] [char](1) NULL CONSTRAINT [DF_producttemp_IsChokingDanger]  DEFAULT ('N'),
	[MenufacturePartNum] [varchar](15) NULL,
	[SPECLabel] [nvarchar](4000) NULL,
	[SupplyShippingCharge] [decimal](10, 5) NULL,
 CONSTRAINT [PK_producttemp] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
