USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[FinanceDocumentCreateNote]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FinanceDocumentCreateNote](
	[SalesOrderCode] [varchar](15) NOT NULL,
	[AccDocTypeCode] [int] NOT NULL,
	[SalesOrderGroupID] [int] NOT NULL,
	[TransactionID] [nvarchar](18) NOT NULL,
	[DocNumber] [nvarchar](18) NOT NULL,
	[SalesOrderType] [int] NULL,
	[ReprocessingFlag] [char](1) NOT NULL CONSTRAINT [DF_FinanceDocumentCreateNote_ReprocessingFlag]  DEFAULT ('0'),
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_FinanceDocumentCreateNote] PRIMARY KEY CLUSTERED 
(
	[SalesOrderCode] ASC,
	[AccDocTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
