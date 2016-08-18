USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[InvoiceD0401ProductItem]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InvoiceD0401ProductItem](
	[SN] [int] IDENTITY(1,1) NOT NULL,
	[AllowanceNumber] [char](16) NOT NULL,
	[OriginalInvoiceNumber] [nchar](10) NOT NULL,
	[OriginalInvoiceDate] [nchar](8) NOT NULL,
	[OriginalSequenceNumber] [char](3) NOT NULL,
	[OriginalDescription] [nvarchar](256) NOT NULL,
	[Quantity] [decimal](10, 0) NOT NULL,
	[Unit] [nvarchar](6) NULL,
	[UnitPrice] [decimal](10, 0) NOT NULL,
	[Amount] [decimal](10, 0) NOT NULL,
	[Tax] [decimal](10, 0) NOT NULL,
	[AllowanceSequenceNumber] [char](3) NOT NULL,
	[TaxType] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_InvoiceD0401ProductItem] PRIMARY KEY CLUSTERED 
(
	[SN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
