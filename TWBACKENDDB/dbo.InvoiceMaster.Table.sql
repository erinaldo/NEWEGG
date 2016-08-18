USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[InvoiceMaster]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InvoiceMaster](
	[SN] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceNo] [char](10) NOT NULL,
	[PriceSum] [decimal](10, 2) NOT NULL CONSTRAINT [DF_InvoiceMaster_PriceSum]  DEFAULT ((0.00)),
	[TaxSum] [decimal](10, 2) NOT NULL CONSTRAINT [DF_InvoiceMaster_TaxSum]  DEFAULT ((0.00)),
	[ShipSum] [decimal](10, 2) NOT NULL CONSTRAINT [DF_InvoiceMaster_ShipSum]  DEFAULT ((0)),
	[ServiceSum] [decimal](10, 2) NOT NULL CONSTRAINT [DF_InvoiceMaster_ServiceSum]  DEFAULT ((0)),
	[SendStatus] [char](2) NOT NULL CONSTRAINT [DF_Table1_Status]  DEFAULT ('N'),
	[InUser] [nvarchar](50) NOT NULL,
	[InDate] [datetime] NOT NULL,
	[EditUser] [nvarchar](50) NULL,
	[EditDate] [datetime] NULL,
	[InstallmentFeeSum] [decimal](10, 2) NULL CONSTRAINT [DF_InvoiceMaster_InstallmentFeeSum]  DEFAULT ((0)),
 CONSTRAINT [PK_InvoiceMaster_SN] PRIMARY KEY CLUSTERED 
(
	[SN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
