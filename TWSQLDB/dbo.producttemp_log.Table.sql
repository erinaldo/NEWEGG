USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[producttemp_log]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[producttemp_log](
	[SN] [int] IDENTITY(1,1) NOT NULL,
	[UserAction] [varchar](15) NULL,
	[EditDate] [datetime] NULL,
	[ID] [int] NULL,
	[ProductID] [int] NULL,
	[SellerProductID] [varchar](150) NULL,
	[Name] [nvarchar](4000) NULL,
	[NameTW] [nvarchar](4000) NULL,
	[Description] [nvarchar](max) NULL,
	[DescriptionTW] [nvarchar](max) NULL,
	[SPEC] [nvarchar](4000) NULL,
	[ManufactureID] [int] NULL,
	[Model] [varchar](300) NULL,
	[BarCode] [varchar](500) NULL,
	[SellerID] [int] NULL,
	[DelvType] [int] NULL,
	[PicStart] [int] NULL,
	[PicEnd] [int] NULL,
	[Cost] [decimal](10, 2) NULL,
	[Status] [int] NULL,
	[InvoiceType] [int] NULL,
	[SaleType] [int] NULL,
	[Length] [decimal](10, 2) NULL,
	[Width] [decimal](10, 2) NULL,
	[Height] [decimal](10, 2) NULL,
	[Weight] [decimal](10, 2) NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[Updated] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[TradeTax] [decimal](10, 2) NULL,
	[Tax] [decimal](10, 2) NULL,
	[Warranty] [int] NULL,
	[UPC] [varchar](15) NULL,
	[Note] [nvarchar](500) NULL,
	[IsMarket] [char](1) NULL,
	[Is18] [char](1) NULL,
	[IsShipDanger] [char](1) NULL,
	[IsChokingDanger] [char](1) NULL,
	[MenufacturePartNum] [varchar](15) NULL,
	[SPECLabel] [nvarchar](4000) NULL,
	[SupplyShippingCharge] [decimal](10, 5) NULL,
 CONSTRAINT [PK_producttemp_log] PRIMARY KEY CLUSTERED 
(
	[SN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
