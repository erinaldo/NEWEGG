USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ItemSketch]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ItemSketch](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProducttempID] [int] NULL,
	[itemtempID] [int] NULL,
	[GroupID] [int] NULL,
	[ItemTempGroupID] [int] NULL,
	[SourceTable] [nvarchar](150) NOT NULL,
	[SellerProductID] [varchar](150) NULL,
	[Name] [nvarchar](200) NULL,
	[Description] [nvarchar](max) NULL,
	[SPEC] [nvarchar](4000) NULL,
	[ManufactureID] [int] NULL,
	[Model] [varchar](30) NULL,
	[BarCode] [varchar](50) NULL,
	[SellerID] [int] NULL,
	[DelvType] [int] NULL,
	[PicStart] [int] NULL,
	[PicEnd] [int] NULL,
	[Cost] [decimal](10, 2) NULL,
	[Length] [decimal](10, 2) NULL,
	[Width] [decimal](10, 2) NULL,
	[Height] [decimal](10, 2) NULL,
	[Weight] [decimal](12, 4) NULL,
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
	[Sdesc] [nvarchar](500) NULL,
	[SpecDetail] [nvarchar](4000) NULL,
	[Spechead] [nvarchar](30) NULL,
	[DelvDate] [nvarchar](50) NULL,
	[CategoryID] [int] NULL,
	[DateStart] [datetime] NULL,
	[DateEnd] [datetime] NULL,
	[DateDel] [datetime] NULL,
	[PriceCard] [decimal](10, 2) NULL,
	[PriceCash] [decimal](10, 2) NULL,
	[ServicePrice] [decimal](10, 2) NULL,
	[PriceLocalship] [decimal](10, 2) NULL,
	[PriceGlobalship] [decimal](10, 2) NULL,
	[ItemQty] [int] NULL,
	[ItemQtyReg] [int] NULL,
	[ItemSafeQty] [int] NULL,
	[QtyLimit] [int] NULL,
	[ShowOrder] [int] NULL,
	[Status] [int] NULL,
	[MarketPrice] [decimal](15, 4) NULL,
	[ShipType] [char](1) NULL,
	[ItemPackage] [char](1) NULL,
	[WarehouseID] [int] NULL,
	[IsNew] [char](1) NULL,
	[InventoryQty] [int] NULL,
	[InventoryQtyReg] [int] NULL,
	[InventorySafeQty] [int] NULL,
	[GrossMargin] [decimal](5, 2) NULL,
	[CreateDate] [datetime] NULL CONSTRAINT [DF_ItemSketch_CreateDate]  DEFAULT (getdate()),
	[CreateUser] [nvarchar](50) NOT NULL,
	[UpdateUser] [nvarchar](50) NOT NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_ItemSketch_Updatedate]  DEFAULT (getdate()),
	[Discard4] [nvarchar](10) NULL CONSTRAINT [DF_ItemSketch_Discard4]  DEFAULT (''),
 CONSTRAINT [PK_ItemSketch_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'廢四機商品, Y=是廢四機' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ItemSketch', @level2type=N'COLUMN',@level2name=N'Discard4'
GO
