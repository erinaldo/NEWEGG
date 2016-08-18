USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[invoiceprizefooter]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[invoiceprizefooter](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Main] [varchar](10) NOT NULL,
	[InvoiceYYYMM] [varchar](10) NOT NULL,
	[TotRecordCnt] [int] NOT NULL,
	[TotPrizeAmt] [decimal](18, 3) NOT NULL,
	[RecAwardBegin] [varchar](10) NOT NULL,
	[RecAwardEnd] [varchar](10) NOT NULL,
 CONSTRAINT [PK_invoiceprizefooter] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
