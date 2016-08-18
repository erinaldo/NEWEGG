USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[InvoiceList]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InvoiceList](
	[SN] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceNumber] [char](10) NOT NULL,
	[SONumber] [varchar](15) NOT NULL,
	[SOPrice] [decimal](10, 2) NOT NULL CONSTRAINT [DF_InvoiceList_SOPrice]  DEFAULT ((0)),
	[SOTax] [decimal](10, 2) NOT NULL CONSTRAINT [DF_InvoiceList_SOTax]  DEFAULT ((0)),
	[SOShip] [decimal](10, 2) NOT NULL CONSTRAINT [DF_InvoiceList_SOShip]  DEFAULT ((0)),
	[SOServicePrice] [decimal](10, 2) NOT NULL CONSTRAINT [DF_InvoiceList_SOServicePrice]  DEFAULT ((0)),
	[InUser] [nvarchar](50) NOT NULL,
	[InDate] [datetime] NOT NULL CONSTRAINT [DF_InvoiceList_InDate]  DEFAULT (getdate()),
	[EditUser] [nvarchar](50) NULL,
	[EditDate] [datetime] NULL,
	[SOInstallmentFee] [decimal](10, 2) NULL CONSTRAINT [DF_InvoiceList_SOInstallmentFee]  DEFAULT ((0)),
 CONSTRAINT [PK_InvoiceList_SN] PRIMARY KEY CLUSTERED 
(
	[SN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
