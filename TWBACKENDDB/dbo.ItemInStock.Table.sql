USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[ItemInStock]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ItemInStock](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[SellerProductId] [nvarchar](150) NULL,
	[ProductId] [int] NULL,
	[WarehouseNumber] [int] NULL,
	[InstockQuantity] [int] NULL,
	[AverageCost] [decimal](10, 2) NULL,
	[LastCost] [decimal](10, 2) NULL,
	[RMAGood] [int] NULL,
	[RMABad] [int] NULL,
	[RMAOnVendor] [int] NULL,
	[RMADead] [int] NULL,
	[Borrow] [int] NULL,
	[ExchangePrice] [decimal](10, 2) NULL,
	[ConsignmentQuantity] [int] NULL,
	[companycode] [int] NULL,
	[Available] [int] NULL CONSTRAINT [DF_ItemInstock_Available]  DEFAULT ((0)),
	[CurrencyCode] [char](5) NULL CONSTRAINT [DF_ItemInStock_CurrencyCode]  DEFAULT ('USD'),
	[createdate] [datetime] NULL,
	[createuser] [nvarchar](50) NULL,
	[updated] [int] NULL,
	[updateuser] [nvarchar](50) NULL,
	[updatedate] [datetime] NULL,
	[TotalCost] [decimal](10, 2) NULL,
	[TotalShippingFee] [decimal](10, 2) NULL,
	[TotalTaxandDuty] [decimal](10, 2) NULL,
	[TotalCustomsCharges] [decimal](10, 2) NULL,
	[TotalServiceCharges] [decimal](10, 2) NULL,
 CONSTRAINT [PK_ItemInStock_id] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
