USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[Seller_FinanDetail_bak]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Seller_FinanDetail_bak](
	[SN] [int] IDENTITY(1,1) NOT NULL,
	[IsCheck] [char](1) NOT NULL,
	[SettlementID] [varchar](20) NULL,
	[SettleType] [int] NOT NULL,
	[OrderID] [varchar](50) NOT NULL,
	[OrderDetailID] [varchar](50) NULL,
	[CartDate] [datetime] NULL,
	[TrackDate] [datetime] NULL,
	[RMADate] [datetime] NULL,
	[POID] [varchar](50) NULL,
	[SellerID] [int] NOT NULL,
	[SellerProductID] [varchar](150) NULL,
	[BaseCurrency] [varchar](50) NULL,
	[ProductID] [int] NOT NULL,
	[ProductName] [nvarchar](200) NOT NULL,
	[Qty] [int] NULL,
	[UnitPrice] [decimal](15, 2) NOT NULL,
	[UnitTax] [decimal](15, 2) NOT NULL,
	[SumPrice] [decimal](15, 2) NOT NULL,
	[SumTax] [decimal](15, 2) NOT NULL,
	[Size] [decimal](15, 2) NULL,
	[ShipFee] [decimal](15, 2) NOT NULL,
	[ShipTax] [decimal](15, 2) NOT NULL,
	[LogisticAmount] [decimal](15, 2) NOT NULL,
	[LogisticTax] [decimal](15, 2) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUserID] [varchar](50) NULL,
	[InDate] [datetime] NULL,
	[InUserID] [varchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
