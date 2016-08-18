USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ItemBackDelete]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ItemBackDelete](
	[ID] [int] NOT NULL,
	[ItemtempID] [int] NULL,
	[Name] [nvarchar](200) NOT NULL,
	[ItemDesc] [nvarchar](max) NULL,
	[DescTW] [nvarchar](max) NULL,
	[Sdesc] [nvarchar](500) NOT NULL,
	[SpecDetail] [nvarchar](4000) NOT NULL,
	[Spechead] [nvarchar](30) NOT NULL,
	[SaleType] [int] NOT NULL,
	[PayType] [int] NOT NULL,
	[Layout] [int] NOT NULL,
	[DelvType] [int] NOT NULL,
	[DelvDate] [nvarchar](50) NOT NULL,
	[Itemnumber] [varchar](30) NOT NULL,
	[ProductID] [int] NOT NULL,
	[CategoryID] [int] NOT NULL,
	[Model] [varchar](30) NOT NULL,
	[SellerID] [int] NOT NULL,
	[DateStart] [datetime] NOT NULL,
	[DateEnd] [datetime] NOT NULL,
	[DateDel] [datetime] NOT NULL,
	[Pricesgst] [decimal](10, 2) NOT NULL,
	[PriceCard] [decimal](10, 2) NOT NULL,
	[PriceCash] [decimal](10, 2) NOT NULL,
	[ServicePrice] [decimal](10, 2) NOT NULL,
	[PricehpType1] [int] NOT NULL,
	[PricehpInst1] [decimal](10, 2) NOT NULL,
	[PricehpType2] [int] NOT NULL,
	[PricehpInst2] [decimal](10, 2) NOT NULL,
	[Inst0Rate] [int] NOT NULL,
	[RedmfdbckRate] [decimal](10, 2) NOT NULL,
	[Coupon] [nvarchar](15) NOT NULL,
	[PriceCoupon] [decimal](10, 2) NOT NULL,
	[PriceLocalship] [decimal](10, 2) NOT NULL,
	[PriceGlobalship] [decimal](10, 2) NOT NULL,
	[Qty] [int] NOT NULL,
	[SafeQty] [int] NOT NULL,
	[QtyLimit] [int] NOT NULL,
	[LimitRule] [nvarchar](50) NOT NULL,
	[QtyReg] [int] NOT NULL,
	[PhotoName] [nvarchar](50) NOT NULL,
	[HtmlName] [nvarchar](50) NOT NULL,
	[ShowOrder] [int] NOT NULL,
	[Class] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[ManufactureID] [int] NOT NULL,
	[StatusNote] [nvarchar](50) NOT NULL,
	[StatusDate] [datetime] NOT NULL,
	[Note] [nvarchar](500) NOT NULL,
	[PicStart] [int] NULL,
	[PicEnd] [int] NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[Updated] [int] NOT NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[CommissionFee] [decimal](15, 4) NULL,
	[StorageFee] [decimal](15, 4) NULL,
	[ShipFee] [decimal](15, 4) NULL,
	[MarketPrice] [decimal](15, 4) NULL,
	[ShipType] [char](1) NULL,
	[UPC] [varchar](15) NULL,
	[SellerPartNum] [varchar](15) NULL,
	[ItemPachage] [char](1) NULL,
	[Taxfee] [decimal](15, 4) NULL,
	[ItemPackage] [char](1) NULL,
	[WarehouseID] [int] NULL,
	[IsNew] [char](1) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
