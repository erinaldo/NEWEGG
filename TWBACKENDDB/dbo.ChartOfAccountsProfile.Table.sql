USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[ChartOfAccountsProfile]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ChartOfAccountsProfile](
	[AccDocTypeCode] [int] NOT NULL,
	[DeliverTypeCode] [int] NOT NULL,
	[Seq] [int] NOT NULL,
	[ItemNo] [int] NOT NULL,
	[AccNumber] [varchar](10) NOT NULL,
	[AccPattern] [char](1) NOT NULL,
	[SignFlag] [char](1) NOT NULL DEFAULT ((1)),
	[ProfitCtr] [varchar](10) NULL,
	[UseFlag] [char](1) NOT NULL DEFAULT ((1)),
	[UseDate] [datetime] NULL,
	[CreateDate] [datetime] NOT NULL DEFAULT (getdate()),
 CONSTRAINT [PK_ChartOfAccounts] PRIMARY KEY CLUSTERED 
(
	[AccDocTypeCode] ASC,
	[DeliverTypeCode] ASC,
	[Seq] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
