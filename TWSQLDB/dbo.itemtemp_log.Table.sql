USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[itemtemp_log]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[itemtemp_log](
	[SN] [int] IDENTITY(1,1) NOT NULL,
	[UserAction] [varchar](15) NULL,
	[EditDate] [datetime] NULL,
	[ID] [int] NULL,
	[ProduttempID] [int] NULL,
	[ItemID] [int] NULL,
	[ItemStatus] [int] NULL,
	[Name] [nvarchar](200) NULL,
	[Sdesc] [nvarchar](500) NULL,
	[DescTW] [nvarchar](max) NULL,
	[ItemTempDesc] [nvarchar](max) NULL,
	[SpecDetail] [nvarchar](4000) NULL,
	[Spechead] [nvarchar](200) NULL,
	[SaleType] [int] NULL,
	[PayType] [int] NULL,
	[Layout] [int] NULL,
	[DelvType] [int] NULL,
	[DelvData] [nvarchar](50) NULL,
	[ItemNumber] [varchar](30) NULL,
	[ProductID] [int] NULL,
	[CategoryID] [int] NULL,
	[Model] [varchar](30) NULL,
	[SellerID] [int] NULL,
	[DateStart] [datetime] NULL,
	[DateEnd] [datetime] NULL,
	[DateDel] [datetime] NULL,
	[Pricesgst] [decimal](10, 2) NULL,
	[PriceCard] [decimal](10, 2) NULL,
	[PriceCash] [decimal](10, 2) NULL,
	[ServicePrice] [decimal](10, 2) NULL,
	[PricehpType1] [int] NULL,
	[Pricehpinst1] [decimal](10, 2) NULL,
	[PricehpType2] [int] NULL,
	[Pricehpinst2] [decimal](10, 2) NULL,
	[Inst0Rate] [int] NULL,
	[RedmfdbckRate] [decimal](10, 2) NULL,
	[Coupon] [nvarchar](15) NULL,
	[PriceCoupon] [decimal](10, 2) NULL,
	[PriceLocalship] [decimal](10, 2) NULL,
	[PriceGlobalship] [decimal](10, 2) NULL,
	[PriceTrade] [int] NULL,
	[Qty] [int] NULL,
	[SafeQty] [int] NULL,
	[QtyLimit] [int] NULL,
	[LimitRule] [nvarchar](50) NULL,
	[QtyReg] [int] NULL,
	[PhotoName] [nvarchar](50) NULL,
	[HtmlName] [nvarchar](50) NULL,
	[Showorder] [int] NULL,
	[Class] [int] NULL,
	[Status] [int] NULL,
	[ManufactureID] [int] NULL,
	[StatusNote] [nvarchar](50) NULL,
	[StatusDate] [datetime] NULL,
	[Note] [nvarchar](500) NULL,
	[CreateDate] [datetime] NULL,
	[CreateUser] [nvarchar](50) NULL,
	[Updated] [int] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[PicStart] [int] NULL,
	[PicEnd] [int] NULL,
	[MarketPrice] [decimal](15, 4) NULL,
	[WareHouseID] [int] NULL,
	[ShipType] [char](1) NULL,
	[Taxfee] [decimal](15, 4) NULL,
	[ItemPackage] [char](1) NULL,
	[IsNew] [char](1) NULL,
	[GrossMargin] [decimal](5, 2) NULL,
	[ApproveMan] [nvarchar](50) NULL,
	[ApproveDate] [datetime] NULL,
	[SubmitMan] [nvarchar](50) NULL,
	[SubmitDate] [datetime] NULL,
 CONSTRAINT [PK_itemtemp_log] PRIMARY KEY CLUSTERED 
(
	[SN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
