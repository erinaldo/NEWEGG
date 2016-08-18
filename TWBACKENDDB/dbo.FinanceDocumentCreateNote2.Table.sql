USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[FinanceDocumentCreateNote2]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FinanceDocumentCreateNote2](
	[SalesOrderCode] [varchar](15) NOT NULL,
	[AccDocTypeCode] [int] NOT NULL,
	[SalesOrderGroupID] [int] NOT NULL,
	[DocNumber] [varchar](18) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_FinanceDocumentCreateNote2] PRIMARY KEY CLUSTERED 
(
	[SalesOrderCode] ASC,
	[AccDocTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
