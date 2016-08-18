USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[HiTrustQueryLog]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HiTrustQueryLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserAction] [varchar](15) NULL,
	[EditDate] [datetime] NULL,
	[MerConfigName] [nvarchar](300) NULL,
	[ordernumber] [varchar](20) NULL,
	[SerConfigName] [nvarchar](300) NULL,
	[retcode] [varchar](6) NULL,
	[currency] [varchar](3) NULL,
	[orderdate] [varchar](26) NULL,
	[orderstatus] [varchar](2) NULL,
	[approveamount] [varchar](12) NULL,
	[authCode] [varchar](6) NULL,
	[authRRN] [varchar](12) NULL,
	[depositamount] [varchar](12) NULL,
	[paybatchnumber] [varchar](9) NULL,
	[capDate] [varchar](26) NULL,
	[credamount] [varchar](12) NULL,
	[credbatchnumber] [varchar](9) NULL,
	[credRRN] [varchar](12) NULL,
	[credCode] [varchar](6) NULL,
	[creddate] [varchar](26) NULL,
	[eci] [varchar](6) NULL,
	[E06] [varchar](2) NULL,
	[E07] [varchar](12) NULL,
	[E08] [varchar](12) NULL,
	[E09] [varchar](12) NULL,
	[redemordernum] [varchar](50) NULL,
	[redem_discount_point] [varchar](50) NULL,
	[redem_discount_amount] [varchar](50) NULL,
	[redem_purchase_amount] [varchar](50) NULL,
	[redem_balance_point] [varchar](50) NULL,
	[acquirer] [varchar](100) NULL,
	[cardtype] [varchar](100) NULL,
	[except] [nvarchar](2000) NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL CONSTRAINT [DF_HiTrustQueryLog_createdate]  DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_HiTrustQueryLog_updatedate]  DEFAULT (getdate()),
 CONSTRAINT [PK_HiTrustQueryLog_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
