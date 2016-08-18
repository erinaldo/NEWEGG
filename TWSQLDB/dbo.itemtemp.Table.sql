USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[itemtemp]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[itemtemp](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProduttempID] [int] NULL,
	[ItemID] [int] NULL,
	[ItemStatus] [int] NULL CONSTRAINT [DF_itemtemp_ItemStatus]  DEFAULT ((1)),
	[Name] [nvarchar](200) NOT NULL,
	[Sdesc] [nvarchar](500) NOT NULL,
	[DescTW] [nvarchar](max) NULL,
	[ItemTempDesc] [nvarchar](max) NULL,
	[SpecDetail] [nvarchar](4000) NOT NULL CONSTRAINT [DF_itemtemp_SpecDetail]  DEFAULT (''),
	[Spechead] [nvarchar](200) NOT NULL,
	[SaleType] [int] NOT NULL CONSTRAINT [DF_itemtemp_SaleType]  DEFAULT ((1)),
	[PayType] [int] NOT NULL CONSTRAINT [DF_itemtemp_PayType]  DEFAULT ((0)),
	[Layout] [int] NOT NULL CONSTRAINT [DF_itemtemp_Layout]  DEFAULT ((0)),
	[DelvType] [int] NOT NULL,
	[DelvData] [nvarchar](50) NOT NULL,
	[ItemNumber] [varchar](30) NOT NULL CONSTRAINT [DF_itemtemp_ItemNumber]  DEFAULT (''),
	[ProductID] [int] NULL,
	[CategoryID] [int] NOT NULL,
	[Model] [varchar](30) NOT NULL CONSTRAINT [DF_itemtemp_Model]  DEFAULT (''),
	[SellerID] [int] NOT NULL,
	[DateStart] [datetime] NOT NULL CONSTRAINT [DF_itemtemp_DateStart]  DEFAULT (getdate()),
	[DateEnd] [datetime] NOT NULL CONSTRAINT [DF_itemtemp_DateEnd]  DEFAULT (dateadd(year,(100),getdate())),
	[DateDel] [datetime] NOT NULL CONSTRAINT [DF_itemtemp_DateDel]  DEFAULT (dateadd(day,(1),dateadd(year,(100),getdate()))),
	[Pricesgst] [decimal](10, 2) NOT NULL,
	[PriceCard] [decimal](10, 2) NOT NULL CONSTRAINT [DF_itemtemp_PriceCard]  DEFAULT ((0)),
	[PriceCash] [decimal](10, 2) NOT NULL,
	[ServicePrice] [decimal](10, 2) NOT NULL CONSTRAINT [DF_itemtemp_itemtemp_serviceprice]  DEFAULT ((0)),
	[PricehpType1] [int] NOT NULL CONSTRAINT [DF_itemtemp_PricehpType1]  DEFAULT ((0)),
	[Pricehpinst1] [decimal](10, 2) NOT NULL CONSTRAINT [DF_itemtemp_PricehpInst1]  DEFAULT ((0)),
	[PricehpType2] [int] NOT NULL CONSTRAINT [DF_itemtemp_PricehpType2]  DEFAULT ((0)),
	[Pricehpinst2] [decimal](10, 2) NOT NULL CONSTRAINT [DF_itemtemp_PricehpInst2]  DEFAULT ((0)),
	[Inst0Rate] [int] NOT NULL CONSTRAINT [DF_itemtemp_Inst0Rate]  DEFAULT ((0)),
	[RedmfdbckRate] [decimal](10, 2) NOT NULL CONSTRAINT [DF_itemtemp_RedmfdbckRate]  DEFAULT ((0)),
	[Coupon] [nvarchar](15) NOT NULL CONSTRAINT [DF_itemtemp_Coupon]  DEFAULT ((0)),
	[PriceCoupon] [decimal](10, 2) NOT NULL CONSTRAINT [DF_itemtemp_PriceCoupon]  DEFAULT ((0)),
	[PriceLocalship] [decimal](10, 2) NOT NULL CONSTRAINT [DF_itemtemp_PriceLocalship]  DEFAULT ((0)),
	[PriceGlobalship] [decimal](10, 2) NOT NULL CONSTRAINT [DF_itemtemp_PriceGlobalship]  DEFAULT ((0)),
	[PriceTrade] [int] NOT NULL CONSTRAINT [DF_itemtemp_PriceTrade]  DEFAULT ((0)),
	[Qty] [int] NOT NULL CONSTRAINT [DF_Itemtemp_Qty]  DEFAULT ((0)),
	[SafeQty] [int] NOT NULL CONSTRAINT [DF_itemtemp_SafeQty]  DEFAULT ((0)),
	[QtyLimit] [int] NOT NULL CONSTRAINT [DF_itemtemp_QtyLimit]  DEFAULT ((0)),
	[LimitRule] [nvarchar](50) NOT NULL CONSTRAINT [DF_itemtemp_LimitRule]  DEFAULT (''),
	[QtyReg] [int] NOT NULL CONSTRAINT [DF_itemtemp_QtyReg]  DEFAULT ((0)),
	[PhotoName] [nvarchar](50) NOT NULL CONSTRAINT [DF_itemtemp_PhotoName]  DEFAULT (''),
	[HtmlName] [nvarchar](50) NOT NULL CONSTRAINT [DF_itemtemp_HtmlName]  DEFAULT (''),
	[Showorder] [int] NOT NULL CONSTRAINT [DF_itemtemp_ShowOrder]  DEFAULT ((0)),
	[Class] [int] NOT NULL CONSTRAINT [DF_itemtemp_Class]  DEFAULT ((1)),
	[Status] [int] NOT NULL CONSTRAINT [DF_itemtemp_Status]  DEFAULT ((1)),
	[ManufactureID] [int] NOT NULL,
	[StatusNote] [nvarchar](50) NOT NULL CONSTRAINT [DF_itemtemp_StatusNote]  DEFAULT (''),
	[StatusDate] [datetime] NOT NULL CONSTRAINT [DF_itemtemp_StatusDate]  DEFAULT (getdate()),
	[Note] [nvarchar](500) NOT NULL CONSTRAINT [DF_itemtemp_Note]  DEFAULT (''),
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_itemtemp_itemtemp_createdate]  DEFAULT (getdate()),
	[CreateUser] [nvarchar](50) NOT NULL,
	[Updated] [int] NOT NULL CONSTRAINT [DF_itemtemp_itemtemp_updated]  DEFAULT ((0)),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[PicStart] [int] NULL CONSTRAINT [DF_itemtemp_PicStart]  DEFAULT ((0)),
	[PicEnd] [int] NULL CONSTRAINT [DF_itemtemp_PicEnd]  DEFAULT ((0)),
	[MarketPrice] [decimal](15, 4) NULL CONSTRAINT [DF_itemtemp_MarketPrice]  DEFAULT (''),
	[WareHouseID] [int] NULL,
	[ShipType] [char](1) NULL,
	[Taxfee] [decimal](15, 4) NULL,
	[ItemPackage] [char](1) NULL,
	[IsNew] [char](1) NULL,
	[GrossMargin] [decimal](5, 2) NULL CONSTRAINT [DF_itemtemp_GrossMargin]  DEFAULT ((0)),
	[ApproveMan] [nvarchar](50) NULL,
	[ApproveDate] [datetime] NULL,
	[SubmitMan] [nvarchar](50) NULL,
	[SubmitDate] [datetime] NULL CONSTRAINT [DF_itemtemp_SubmitDate]  DEFAULT (getdate()),
	[Discard4] [nvarchar](10) NULL CONSTRAINT [DF_itemTemp_Discard4]  DEFAULT (''),
 CONSTRAINT [PK_itemtemp_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'廢四機商品, Y=是廢四機' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'itemtemp', @level2type=N'COLUMN',@level2name=N'Discard4'
GO
