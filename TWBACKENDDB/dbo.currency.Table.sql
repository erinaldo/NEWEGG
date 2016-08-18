USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[currency]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[currency](
	[ID] [int] NOT NULL,
	[Bank] [nvarchar](50) NULL,
	[Type] [char](10) NULL,
	[Year] [char](4) NULL,
	[Month] [char](2) NULL,
	[AverageExchangeRate] [decimal](10, 4) NULL,
	[CountryID] [int] NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL CONSTRAINT [DF_currency_currency_createdate]  DEFAULT (getdate()),
	[Updated] [int] NULL CONSTRAINT [DF_currency_currency_updated]  DEFAULT ((0)),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_currency_currency_updatedate]  DEFAULT (getdate()),
 CONSTRAINT [PK_currency_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
