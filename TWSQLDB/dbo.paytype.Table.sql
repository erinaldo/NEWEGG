USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[paytype]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[paytype](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[BankID] [int] NULL,
	[InsRate] [decimal](10, 5) NULL CONSTRAINT [DF_paytype_paytype_insrate]  DEFAULT ((0.00)),
	[PayType0rateNum] [int] NULL,
	[PayType0rateType] [int] NULL CONSTRAINT [DF_Table_1_paytype_ratetype]  DEFAULT ((0)),
	[OnlineStartDate] [datetime] NULL,
	[OnlineEndDate] [datetime] NULL,
	[Status] [int] NULL CONSTRAINT [DF_paytype_paytype_status]  DEFAULT ((0)),
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL CONSTRAINT [DF_paytype_paytype_createdate]  DEFAULT (getdate()),
	[Updated] [int] NULL CONSTRAINT [DF_paytype_paytype_updated]  DEFAULT ((0)),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_paytype_paytype_updatedate]  DEFAULT (getdate()),
	[PayTypeCode] [varchar](30) NULL,
	[InsRateForBank] [decimal](10, 5) NOT NULL DEFAULT ((0)),
	[BankList] [nvarchar](500) NULL,
	[ChooseOrder] [int] NULL,
	[BankIDList] [nvarchar](500) NULL,
	[Verification] [int] NULL,
 CONSTRAINT [PK_paytype_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
