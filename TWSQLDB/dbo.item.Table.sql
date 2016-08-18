USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[item]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[item](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ItemtempID] [int] NULL,
	[Name] [nvarchar](200) NOT NULL,
	[ItemDesc] [nvarchar](max) NULL,
	[DescTW] [nvarchar](max) NULL,
	[Sdesc] [nvarchar](500) NOT NULL,
	[SpecDetail] [nvarchar](4000) NOT NULL CONSTRAINT [DF_item_SpecDetail]  DEFAULT (''),
	[Spechead] [nvarchar](200) NOT NULL,
	[SaleType] [int] NOT NULL CONSTRAINT [DF_Item_SaleType]  DEFAULT ((1)),
	[PayType] [int] NOT NULL CONSTRAINT [DF_Item_PayType]  DEFAULT ((0)),
	[Layout] [int] NOT NULL CONSTRAINT [DF_Item_Layout]  DEFAULT ((0)),
	[DelvType] [int] NOT NULL,
	[DelvDate] [nvarchar](50) NOT NULL,
	[Itemnumber] [varchar](30) NOT NULL CONSTRAINT [DF_Item_Itemnumber]  DEFAULT (''),
	[ProductID] [int] NOT NULL,
	[CategoryID] [int] NOT NULL,
	[Model] [varchar](30) NOT NULL CONSTRAINT [DF_Item_Model]  DEFAULT (''),
	[SellerID] [int] NOT NULL,
	[DateStart] [datetime] NOT NULL CONSTRAINT [DF_Item_DateStart]  DEFAULT (getdate()),
	[DateEnd] [datetime] NOT NULL CONSTRAINT [DF_Item_DateEnd]  DEFAULT (dateadd(year,(100),getdate())),
	[DateDel] [datetime] NOT NULL CONSTRAINT [DF_Item_DateDel]  DEFAULT (dateadd(day,(1),dateadd(year,(100),getdate()))),
	[Pricesgst] [decimal](10, 2) NOT NULL,
	[PriceCard] [decimal](10, 2) NOT NULL CONSTRAINT [DF_Item_PriceCard]  DEFAULT ((0)),
	[PriceCash] [decimal](10, 2) NOT NULL,
	[ServicePrice] [decimal](10, 2) NOT NULL CONSTRAINT [DF_item_item_serviceprice]  DEFAULT ((0)),
	[PricehpType1] [int] NOT NULL CONSTRAINT [DF_Item_PricehpType1]  DEFAULT ((0)),
	[PricehpInst1] [decimal](10, 2) NOT NULL CONSTRAINT [DF_Item_PricehpInst1]  DEFAULT ((0)),
	[PricehpType2] [int] NOT NULL CONSTRAINT [DF_Item_PricehpType2]  DEFAULT ((0)),
	[PricehpInst2] [decimal](10, 2) NOT NULL CONSTRAINT [DF_Item_PricehpInst2]  DEFAULT ((0)),
	[Inst0Rate] [int] NOT NULL CONSTRAINT [DF_Item_Inst0Rate]  DEFAULT ((0)),
	[RedmfdbckRate] [decimal](10, 2) NOT NULL CONSTRAINT [DF_Item_RedmfdbckRate]  DEFAULT ((0)),
	[Coupon] [nvarchar](15) NOT NULL CONSTRAINT [DF_Item_Coupon]  DEFAULT ((0)),
	[PriceCoupon] [decimal](10, 2) NOT NULL CONSTRAINT [DF_Item_PriceCoupon]  DEFAULT ((0)),
	[PriceLocalship] [decimal](10, 2) NOT NULL CONSTRAINT [DF_Item_PriceLocalship]  DEFAULT ((0)),
	[PriceGlobalship] [decimal](10, 2) NOT NULL CONSTRAINT [DF_Item_PriceGlobalship]  DEFAULT ((0)),
	[Qty] [int] NOT NULL CONSTRAINT [DF_Item_Qty]  DEFAULT ((0)),
	[SafeQty] [int] NOT NULL CONSTRAINT [DF_Item_SafeQty]  DEFAULT ((0)),
	[QtyLimit] [int] NOT NULL CONSTRAINT [DF_Item_QtyLimit]  DEFAULT ((0)),
	[LimitRule] [nvarchar](50) NOT NULL CONSTRAINT [DF_Item_LimitRule]  DEFAULT (''),
	[QtyReg] [int] NOT NULL CONSTRAINT [DF_Item_QtyReg]  DEFAULT ((0)),
	[PhotoName] [nvarchar](50) NOT NULL CONSTRAINT [DF_Item_PhotoName]  DEFAULT (''),
	[HtmlName] [nvarchar](50) NOT NULL CONSTRAINT [DF_Item_HtmlName]  DEFAULT (''),
	[ShowOrder] [int] NOT NULL CONSTRAINT [DF_Item_ShowOrder]  DEFAULT ((0)),
	[Class] [int] NOT NULL CONSTRAINT [DF_Item_Class]  DEFAULT ((1)),
	[Status] [int] NOT NULL CONSTRAINT [DF_Item_Status]  DEFAULT ((1)),
	[ManufactureID] [int] NOT NULL,
	[StatusNote] [nvarchar](50) NOT NULL CONSTRAINT [DF_Item_StatusNote]  DEFAULT (''),
	[StatusDate] [datetime] NOT NULL CONSTRAINT [DF_Item_StatusDate]  DEFAULT (getdate()),
	[Note] [nvarchar](500) NOT NULL CONSTRAINT [DF_Item_Note]  DEFAULT (''),
	[PicStart] [int] NULL CONSTRAINT [DF_item_PicStart]  DEFAULT ((0)),
	[PicEnd] [int] NULL CONSTRAINT [DF_item_PicEnd]  DEFAULT ((0)),
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_item_item_createdate]  DEFAULT (getdate()),
	[CreateUser] [nvarchar](50) NOT NULL,
	[Updated] [int] NOT NULL CONSTRAINT [DF_item_item_updated]  DEFAULT ((0)),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[CommissionFee] [decimal](15, 4) NULL,
	[StorageFee] [decimal](15, 4) NULL,
	[ShipFee] [decimal](15, 4) NULL,
	[MarketPrice] [decimal](15, 4) NULL CONSTRAINT [DF_item_MarketPrice]  DEFAULT (''),
	[ShipType] [char](1) NULL,
	[UPC] [varchar](15) NULL,
	[SellerPartNum] [varchar](15) NULL,
	[ItemPachage] [char](1) NULL,
	[Taxfee] [decimal](15, 4) NULL,
	[ItemPackage] [char](1) NULL,
	[WarehouseID] [int] NULL,
	[IsNew] [char](1) NULL,
	[Discard4] [nvarchar](10) NULL CONSTRAINT [DF_item_Discard4]  DEFAULT (''),
 CONSTRAINT [PK_item_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[item]  WITH NOCHECK ADD  CONSTRAINT [CK_item_datestart_le_dateend_l_datedel] CHECK  (([DateStart]<=[DateEnd] AND [DateEnd]<[DateDel]))
GO
ALTER TABLE [dbo].[item] CHECK CONSTRAINT [CK_item_datestart_le_dateend_l_datedel]
GO
ALTER TABLE [dbo].[item]  WITH NOCHECK ADD  CONSTRAINT [CK_item_ID] CHECK  (([ID]>(0)))
GO
ALTER TABLE [dbo].[item] CHECK CONSTRAINT [CK_item_ID]
GO
ALTER TABLE [dbo].[item]  WITH NOCHECK ADD  CONSTRAINT [CK_item_QtyReg] CHECK  ((([Qty]-[QtyReg])>=(0) AND [Qty]>(0) OR [Qty]=(0)))
GO
ALTER TABLE [dbo].[item] CHECK CONSTRAINT [CK_item_QtyReg]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'廢四機商品, Y=是廢四機' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'item', @level2type=N'COLUMN',@level2name=N'Discard4'
GO
