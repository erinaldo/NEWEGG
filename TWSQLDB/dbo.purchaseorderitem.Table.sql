USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[purchaseorderitem]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[purchaseorderitem](
	[Code] [varchar](15) NOT NULL,
	[PurchaseorderCode] [varchar](15) NOT NULL,
	[SalesOrderItemCode] [varchar](15) NULL,
	[SellerOrderCode] [varchar](30) NULL,
	[CurrencyID] [int] NULL,
	[ItemID] [int] NOT NULL,
	[ItemlistID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[ProductlistID] [int] NOT NULL,
	[Name] [nvarchar](200) NULL,
	[SourcecurrencyID] [int] NULL,
	[Price] [decimal](13, 5) NULL,
	[Priceinst] [decimal](10, 2) NULL,
	[Qty] [int] NOT NULL,
	[PriceCoupon] [decimal](10, 2) NULL,
	[RedmtkOut] [int] NULL,
	[RedmBLN] [int] NULL,
	[RedmFDBCK] [int] NULL,
	[Status] [int] NULL,
	[StatusNote] [nvarchar](50) NULL,
	[Date] [datetime] NULL,
	[Attribs] [nvarchar](50) NULL,
	[Note] [nvarchar](50) NULL,
	[WftkOut] [int] NULL,
	[WfBLN] [int] NULL,
	[AdjPrice] [int] NULL,
	[ActID] [nvarchar](50) NULL,
	[ActtkOut] [int] NULL,
	[ProdcutCostID] [int] NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_purchaseorderitem_purchaseorderitem_createdate]  DEFAULT (getdate()),
	[Updated] [int] NULL CONSTRAINT [DF_purchaseorderitem_purchaseorderitem_updated]  DEFAULT ((0)),
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[ImportCost] [decimal](10, 2) NULL,
	[CCCCode] [varchar](15) NULL,
	[DutyRate] [decimal](13, 5) NULL,
	[ProductTax] [decimal](13, 5) NULL,
	[FullTaxValue] [decimal](10, 2) NULL,
	[ShippingFee] [decimal](10, 2) NULL,
	[RMACode] [nvarchar](50) NULL,
	[TaxandDuty] [decimal](10, 2) NULL,
	[WarehouseID] [int] NULL,
	[Customs_Charge] [decimal](10, 2) NULL,
	[TradeServiceCharges] [decimal](10, 2) NULL,
	[SellerID] [int] NULL,
	[LocalPriceinst] [decimal](10, 4) NULL,
	[Coupons] [nvarchar](20) NULL,
	[SupplyShippingCharge] [decimal](10, 2) NULL,
 CONSTRAINT [PK_purchaseorderitem_code] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
