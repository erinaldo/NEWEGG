USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[InvoicePrintLog]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InvoicePrintLog](
	[TransactionNumber] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceNumber] [char](10) NOT NULL,
	[PrintDate] [datetime] NOT NULL,
	[User] [nvarchar](30) NULL,
PRIMARY KEY CLUSTERED 
(
	[TransactionNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
