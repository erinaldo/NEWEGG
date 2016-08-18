USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[FinanceDocumentCreateNote3]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FinanceDocumentCreateNote3](
	[SalesOrderCode] [varchar](15) NOT NULL,
	[AccDocTypeCode] [int] NOT NULL,
	[SalesOrderGroupID] [int] NOT NULL,
	[XQDocNumber] [varchar](18) NULL,
	[XQCreateDate] [datetime] NULL,
	[XDDocNumber] [varchar](18) NULL,
	[XDCreateDate] [datetime] NULL,
	[XIDocNumber] [varchar](18) NULL,
	[XICreateDate] [datetime] NULL,
	[XIRMADocNumber] [varchar](18) NULL,
	[XIRMACreateDate] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
