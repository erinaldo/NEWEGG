USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[HiTrustTrans]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HiTrustTrans](
	[MerConfigName] [nvarchar](300) NOT NULL,
	[ordernumber] [varchar](20) NOT NULL,
	[SerConfigName] [varchar](300) NOT NULL,
	[amount] [varchar](12) NOT NULL,
	[currency] [varchar](3) NOT NULL,
	[orderdesc] [varchar](40) NOT NULL,
	[depositflag] [varchar](1) NOT NULL,
	[queryflag] [varchar](1) NOT NULL,
	[merupdateURL] [nvarchar](256) NOT NULL,
	[updateURL] [nvarchar](256) NOT NULL,
	[returnURL] [nvarchar](256) NOT NULL,
	[ticketno] [varchar](20) NULL,
	[pan] [varchar](20) NULL,
	[expiry] [varchar](4) NULL,
	[E01] [varchar](3) NULL,
	[E03] [varchar](2) NULL,
	[E04] [varchar](1) NULL,
	[E05] [varchar](1) NULL,
	[E11] [varchar](3) NULL,
	[E12] [varchar](8) NULL,
	[E13] [varchar](8) NULL,
	[E14] [varchar](10) NULL,
	[retcode] [varchar](6) NULL,
	[token] [varchar](300) NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL CONSTRAINT [DF_HiTrustTrans_createdate]  DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_HiTrustTrans_updatedate]  DEFAULT (getdate()),
 CONSTRAINT [PK_HiTrustTrans] PRIMARY KEY CLUSTERED 
(
	[ordernumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
