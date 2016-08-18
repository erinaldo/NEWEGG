USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[creditinvoicetax]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[creditinvoicetax](
	[SN] [int] NOT NULL,
	[Yyymm] [nvarchar](50) NULL,
	[SalesNO] [nvarchar](50) NULL,
	[InvoiceNO] [nvarchar](50) NULL,
	[BuyerNO] [nvarchar](50) NULL,
	[Type] [int] NULL,
	[Mode] [int] NULL,
	[TaxType] [int] NULL,
	[Amount] [int] NULL,
	[Tax] [int] NULL,
	[Specail] [int] NULL,
	[Note] [int] NULL,
	[CigNote] [int] NULL,
	[DataType] [int] NULL,
	[Map] [int] NULL,
	[Page] [int] NULL,
	[Other] [nvarchar](50) NULL,
	[Douane] [nvarchar](50) NULL,
	[Apply] [int] NULL,
	[PaidID] [int] NULL,
	[FeeType] [int] NULL,
	[Date] [datetime] NULL,
	[Status] [int] NULL,
	[RegID] [int] NULL,
	[SysDate] [datetime] NULL,
 CONSTRAINT [PK_creditinvoicetax_sn] PRIMARY KEY CLUSTERED 
(
	[SN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
