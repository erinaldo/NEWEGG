USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[InvoiceC0401]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InvoiceC0401](
	[InvoiceNumber] [char](10) NOT NULL,
	[InvoiceDate] [char](8) NOT NULL,
	[InvoiceTime] [char](9) NOT NULL,
	[SellerIdentifier] [varchar](50) NOT NULL,
	[SellerName] [nvarchar](60) NOT NULL,
	[SellerAddress] [nvarchar](200) NULL,
	[SellerPersonInCharge] [nvarchar](30) NULL,
	[SellerTelephoneNumber] [varchar](26) NULL,
	[SellerFacsimileNumber] [varchar](26) NULL,
	[SellerEmailAddress] [varchar](80) NULL,
	[SellerCustomerNumber] [varchar](20) NULL,
	[SellerRoleRemark] [varchar](40) NULL,
	[BuyerIdentifier] [varchar](50) NOT NULL,
	[BuyerName] [nvarchar](60) NOT NULL,
	[BuyerAddress] [nvarchar](200) NULL,
	[BuyerPersonInCharge] [nvarchar](30) NULL,
	[BuyerTelephoneNumber] [varchar](26) NULL,
	[BuyerFacsimileNumber] [varchar](26) NULL,
	[BuyerEmailAddress] [varchar](80) NULL,
	[BuyerCustomerNumber] [varchar](20) NULL,
	[BuyerRoleRemark] [varchar](40) NULL,
	[CheckNumber] [varchar](50) NULL,
	[BuyerRemark] [nvarchar](50) NULL,
	[MainRemark] [nvarchar](200) NULL,
	[CustomsClearanceMark] [nvarchar](20) NULL,
	[Category] [varchar](50) NULL,
	[RelateNumber] [varchar](20) NULL,
	[InvoiceType] [nvarchar](20) NOT NULL,
	[GroupMark] [char](1) NULL,
	[DonateMark] [nvarchar](20) NOT NULL,
	[CarrierType] [varchar](50) NULL,
	[CarrierId1] [varchar](64) NULL,
	[CarrierId2] [varchar](64) NULL,
	[PrintMark] [char](1) NOT NULL,
	[NPOBAN] [nvarchar](50) NULL,
	[RandomNumber] [char](4) NOT NULL,
	[SalesAmount] [decimal](10, 0) NOT NULL,
	[FreeTaxSalesAmount] [decimal](10, 0) NOT NULL,
	[ZeroTaxSalesAmount] [decimal](10, 0) NOT NULL,
	[TaxType] [nvarchar](10) NOT NULL,
	[TaxRate] [decimal](6, 5) NOT NULL,
	[TaxAmount] [decimal](10, 0) NOT NULL,
	[TotalAmount] [decimal](10, 0) NOT NULL,
	[DiscountAmount] [decimal](10, 0) NULL,
	[OriginalCurrencyAmount] [decimal](10, 0) NULL,
	[ExchangeRate] [decimal](10, 5) NULL,
	[Currency] [varchar](50) NULL,
	[CreateDate] [datetime] NOT NULL,
	[Status] [int] NOT NULL CONSTRAINT [Status]  DEFAULT ((1)),
	[ErrorMessage] [nvarchar](50) NULL,
 CONSTRAINT [PK_InvoiceC0401] PRIMARY KEY CLUSTERED 
(
	[InvoiceNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
