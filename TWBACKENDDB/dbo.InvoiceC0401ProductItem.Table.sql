USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[InvoiceC0401ProductItem]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InvoiceC0401ProductItem](
	[SN] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceNumber] [char](10) NOT NULL,
	[Description] [nvarchar](256) NOT NULL,
	[Quantity] [decimal](10, 0) NOT NULL,
	[Unit] [nvarchar](6) NULL,
	[UnitPrice] [decimal](10, 0) NOT NULL,
	[Amount] [decimal](10, 0) NULL,
	[SequenceNumber] [char](3) NOT NULL,
	[Remark] [nvarchar](40) NULL,
	[RelateNumber] [varchar](20) NULL,
	[TaxField] [decimal](10, 0) NULL CONSTRAINT [DF_InvoiceC0401ProductItem_TaxField_1]  DEFAULT ((0)),
 CONSTRAINT [PK_InvoiceC0401ProductItem] PRIMARY KEY CLUSTERED 
(
	[SN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
