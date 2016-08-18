USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[InvoiceC0501]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InvoiceC0501](
	[CancelInvoiceNumber] [char](10) NOT NULL,
	[InvoiceDate] [char](8) NOT NULL,
	[SellerId] [char](10) NOT NULL,
	[BuyerId] [char](10) NOT NULL,
	[CancelDate] [char](8) NOT NULL,
	[CancelTime] [char](8) NOT NULL,
	[CancelReason] [nvarchar](20) NOT NULL,
	[ReturnTaxDocumentNumber] [nvarchar](60) NULL,
	[Remark] [nvarchar](200) NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_InvoiceC0501] PRIMARY KEY CLUSTERED 
(
	[CancelInvoiceNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
