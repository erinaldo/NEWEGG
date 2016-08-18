USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[BankBonusTemp]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BankBonusTemp](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[BankBonusID] [int] NULL,
	[BankCode] [nvarchar](10) NOT NULL,
	[PhotoName] [nvarchar](200) NOT NULL,
	[Status] [int] NOT NULL,
	[PublishBank] [nvarchar](50) NOT NULL,
	[PublishBankPhone] [nvarchar](100) NOT NULL,
	[OffsetMax] [decimal](10, 2) NOT NULL,
	[ConsumeLimit] [decimal](10, 2) NOT NULL,
	[PointLimit] [decimal](10, 2) NOT NULL,
	[ProportionPoint] [decimal](10, 2) NOT NULL,
	[ProportionMoney] [decimal](10, 2) NOT NULL,
	[SerialNumber] [int] NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[Updated] [int] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[AuditUser] [nvarchar](50) NULL,
	[AuditDate] [datetime] NULL,
	[DescriptionFormat] [nvarchar](max) NULL,
	[BankID] [int] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
