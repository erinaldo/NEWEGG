USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[Seller_FinanMaster]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Seller_FinanMaster](
	[FinanStatus] [char](1) NOT NULL CONSTRAINT [DF_Seller_FinanMaster_FinanStatus]  DEFAULT ('S'),
	[SettlementID] [varchar](20) NOT NULL,
	[SellerID] [int] NOT NULL,
	[IsOpen] [varchar](1) NOT NULL CONSTRAINT [DF_Seller_FinanMaster_IsOpen]  DEFAULT ('N'),
	[DateStart] [datetime] NOT NULL,
	[DateEnd] [datetime] NOT NULL,
	[SettleDate] [datetime] NOT NULL,
	[SettleMonth] [varchar](6) NOT NULL,
	[InvoDate] [datetime] NULL,
	[InvoNumber] [varchar](50) NULL,
	[RemitDate] [datetime] NULL,
	[BaseCurrency] [char](3) NOT NULL CONSTRAINT [DF_Seller_FinanMaster_BaseCurrency]  DEFAULT ('TWD'),
	[SettleCurrency] [char](3) NULL,
	[POPrice] [decimal](15, 2) NOT NULL CONSTRAINT [DF_Seller_FinanMaster_POPrice]  DEFAULT ((0.00)),
	[POTax] [decimal](15, 2) NOT NULL CONSTRAINT [DF_Seller_FinanMaster_POTax]  DEFAULT ((0.00)),
	[RMAPrice] [decimal](15, 2) NOT NULL CONSTRAINT [DF_Seller_FinanMaster_RMAPrice]  DEFAULT ((0.00)),
	[RMATax] [decimal](15, 2) NOT NULL CONSTRAINT [DF_Seller_FinanMaster_RMATax]  DEFAULT ((0.00)),
	[WarehousePrice] [decimal](15, 2) NOT NULL CONSTRAINT [DF_Seller_FinanMaster_WarehousePrice]  DEFAULT ((0.00)),
	[WarehouseTax] [decimal](15, 2) NOT NULL CONSTRAINT [DF_Seller_FinanMaster_WarehouseTax]  DEFAULT ((0.00)),
	[TotalAmount] [decimal](15, 2) NOT NULL CONSTRAINT [DF_Seller_FinanMaster_TotalAmount]  DEFAULT ((0.00)),
	[TotalTax] [decimal](15, 2) NOT NULL CONSTRAINT [DF_Seller_FinanMaster_TotalTax]  DEFAULT ((0.00)),
	[PaymentAmount] [decimal](15, 2) NOT NULL CONSTRAINT [DF_Seller_FinanMaster_PaymentAmount]  DEFAULT ((0.00)),
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_Seller_FinanMaster_UpdateDate]  DEFAULT (getdate()),
	[UpdateUserID] [varchar](50) NULL,
	[InDate] [datetime] NULL CONSTRAINT [DF_Seller_FinanMaster_InDate]  DEFAULT (getdate()),
	[InUserID] [varchar](50) NULL,
 CONSTRAINT [PK_Seller_FinanMaster] PRIMARY KEY CLUSTERED 
(
	[SettlementID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
