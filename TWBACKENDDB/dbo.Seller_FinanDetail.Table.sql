USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[Seller_FinanDetail]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Seller_FinanDetail](
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
	[UnitPrice] [decimal](15, 2) NOT NULL CONSTRAINT [DF_Seller_FinanDetail_UnitPrice]  DEFAULT ((0.00)),
	[UnitTax] [decimal](15, 2) NOT NULL CONSTRAINT [DF_Seller_FinanDetail_UnitTax]  DEFAULT ((0.00)),
	[SumPrice] [decimal](15, 2) NOT NULL CONSTRAINT [DF_Seller_FinanDetail_SumPrice]  DEFAULT ((0.00)),
	[SumTax] [decimal](15, 2) NOT NULL CONSTRAINT [DF_Seller_FinanDetail_SumPrice1]  DEFAULT ((0.00)),
	[Size] [decimal](15, 2) NULL,
	[ShipFee] [decimal](15, 2) NOT NULL CONSTRAINT [DF_Seller_FinanDetail_ShipFee]  DEFAULT ((0.00)),
	[ShipTax] [decimal](15, 2) NOT NULL CONSTRAINT [DF_Seller_FinanDetail_ShipTax]  DEFAULT ((0.00)),
	[LogisticAmount] [decimal](15, 2) NOT NULL CONSTRAINT [DF_Seller_FinanDetail_LogisticAmount]  DEFAULT ((0.00)),
	[LogisticTax] [decimal](15, 2) NOT NULL CONSTRAINT [DF_Seller_FinanDetail_LogisticTax]  DEFAULT ((0.00)),
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_Seller_FinanDetail_UpdateDate]  DEFAULT (getdate()),
	[UpdateUserID] [varchar](50) NULL,
	[InDate] [datetime] NULL CONSTRAINT [DF_Seller_FinanDetail_InDate]  DEFAULT (getdate()),
	[InUserID] [varchar](50) NULL,
 CONSTRAINT [PK_Seller_FinanDetail] PRIMARY KEY CLUSTERED 
(
	[SN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
