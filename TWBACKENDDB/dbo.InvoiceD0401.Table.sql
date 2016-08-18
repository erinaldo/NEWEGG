USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[InvoiceD0401]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InvoiceD0401](
	[AllowanceNumber] [nchar](16) NOT NULL,
	[AllowanceDate] [char](8) NOT NULL,
	[SellerIdentifier] [varchar](50) NOT NULL,
	[SellerName] [nvarchar](60) NOT NULL,
	[SellerAddress] [nvarchar](200) NULL,
	[SellerPersonInCharge] [nvarchar](30) NULL,
	[SellerTelephoneNumber] [varchar](26) NULL,
	[SellerFacsimileNumber] [varchar](26) NULL,
	[SellerEmailAddress] [varchar](80) NULL,
	[SellerCustomerNumber] [varchar](20) NULL,
	[BuyerIdentifier] [varchar](50) NOT NULL,
	[BuyerName] [nvarchar](60) NOT NULL,
	[BuyerAddress] [nvarchar](200) NULL,
	[BuyerPersonInCharge] [nvarchar](30) NULL,
	[BuyerTelephoneNumber] [varchar](26) NULL,
	[BuyerFacsimileNumber] [varchar](26) NULL,
	[BuyerEmailAddress] [varchar](80) NULL,
	[BuyerCustomerNumber] [varchar](20) NULL,
	[AllowanceType] [nvarchar](20) NULL,
	[Attachment] [varchar](max) NULL,
	[TaxAmount] [decimal](10, 0) NOT NULL,
	[TotalAmount] [decimal](10, 0) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Status] [int] NOT NULL CONSTRAINT [D0401Status]  DEFAULT ((1)),
 CONSTRAINT [PK_InvoiceD0401] PRIMARY KEY CLUSTERED 
(
	[AllowanceNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
