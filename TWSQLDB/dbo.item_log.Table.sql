USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[item_log]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[item_log](
	[UserAction] [varchar](15) NULL,
	[EditDate] [datetime] NULL,
	[ID] [int] NULL,
	[DelvType] [int] NULL,
	[DelvDate] [nvarchar](50) NULL,
	[ProductID] [int] NULL,
	[CategoryID] [int] NULL,
	[SellerID] [int] NULL,
	[DateStart] [datetime] NULL,
	[DateEnd] [datetime] NULL,
	[DateDel] [datetime] NULL,
	[PriceCard] [decimal](10, 2) NULL,
	[PriceCash] [decimal](10, 2) NULL,
	[Qty] [int] NULL,
	[SafeQty] [int] NULL,
	[QtyLimit] [int] NULL,
	[QtyReg] [int] NULL,
	[Status] [int] NULL,
	[ManufactureID] [int] NULL,
	[PicStart] [int] NULL,
	[PicEnd] [int] NULL,
	[CreateDate] [datetime] NULL,
	[CreateUser] [nvarchar](50) NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[MarketPrice] [decimal](15, 4) NULL,
	[ShipType] [char](1) NULL,
	[UPC] [varchar](15) NULL,
	[SellerPartNum] [varchar](15) NULL,
	[ItemPackage] [char](1) NULL,
	[WarehouseID] [int] NULL,
	[IsNew] [char](1) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
