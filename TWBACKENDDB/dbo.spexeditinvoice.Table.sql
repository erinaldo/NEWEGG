USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[spexeditinvoice]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [dbo].[spexeditinvoice](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SO] [varchar](25) NOT NULL,
	[BoxNo] [varchar](20) NULL CONSTRAINT [DF_spexeditinvoice_spexeditinvoice_BoxNo]  DEFAULT (NULL),
	[InvoiceItem] [varchar](200) NOT NULL CONSTRAINT [DF_spexeditinvoice_spexeditinvoice_InvoiceItem]  DEFAULT (NULL),
	[Quantity] [varchar](20) NULL CONSTRAINT [DF_spexeditinvoice_spexeditinvoice_Quantity]  DEFAULT (NULL),
	[UnitPrice] [varchar](20) NULL CONSTRAINT [DF_spexeditinvoice_spexeditinvoice_UnitPrice]  DEFAULT (NULL),
	[BoxWeight] [varchar](20) NULL,
	[FileName] [varchar](50) NULL,
 CONSTRAINT [PK_spexeditinvoice] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
