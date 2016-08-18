USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[Seller_FinanMaster_bak]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Seller_FinanMaster_bak](
	[FinanStatus] [char](1) NOT NULL,
	[SettlementID] [varchar](20) NOT NULL,
	[SellerID] [int] NOT NULL,
	[IsOpen] [varchar](1) NOT NULL,
	[DateStart] [datetime] NOT NULL,
	[DateEnd] [datetime] NOT NULL,
	[SettleDate] [datetime] NOT NULL,
	[SettleMonth] [varchar](6) NOT NULL,
	[InvoDate] [datetime] NULL,
	[InvoNumber] [varchar](50) NULL,
	[RemitDate] [datetime] NULL,
	[BaseCurrency] [char](3) NOT NULL,
	[SettleCurrency] [char](3) NULL,
	[POPrice] [decimal](15, 2) NOT NULL,
	[POTax] [decimal](15, 2) NOT NULL,
	[RMAPrice] [decimal](15, 2) NOT NULL,
	[RMATax] [decimal](15, 2) NOT NULL,
	[WarehousePrice] [decimal](15, 2) NOT NULL,
	[WarehouseTax] [decimal](15, 2) NOT NULL,
	[TotalAmount] [decimal](15, 2) NOT NULL,
	[TotalTax] [decimal](15, 2) NOT NULL,
	[PaymentAmount] [decimal](15, 2) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUserID] [varchar](50) NULL,
	[InDate] [datetime] NULL,
	[InUserID] [varchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
