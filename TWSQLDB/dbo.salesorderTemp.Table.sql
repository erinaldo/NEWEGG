USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[salesorderTemp]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[salesorderTemp](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](15) NOT NULL,
	[salesorderGroupID] [int] NULL,
	[IDNO] [varchar](20) NULL,
	[Name] [nvarchar](50) NULL,
	[AccountID] [int] NOT NULL,
	[TelDay] [varchar](30) NULL,
	[TelNight] [varchar](30) NULL,
	[Mobile] [varchar](30) NULL,
	[Email] [varchar](256) NULL,
	[PayType] [int] NULL,
	[StarvlDate] [datetime] NULL,
	[CardHolder] [nvarchar](50) NULL,
	[CardTelDay] [varchar](30) NULL,
	[CardTelNight] [varchar](30) NULL,
	[CardMobile] [varchar](30) NULL,
	[CardLOC] [nvarchar](10) NULL,
	[CardZip] [char](5) NULL,
	[CardADDR] [nvarchar](150) NULL,
	[CardNo] [varchar](64) NULL,
	[CardNochk] [varchar](30) NULL,
	[CardType] [char](10) NULL,
	[CardBank] [nvarchar](50) NULL,
	[CardExpire] [char](10) NULL,
	[CardBirthday] [datetime] NULL,
	[InvoiceReceiver] [nvarchar](50) NULL,
	[InvoiceID] [nvarchar](10) NULL,
	[InvoiceTitle] [nvarchar](50) NULL,
	[InvoiceLoc] [nvarchar](10) NULL,
	[InvoiceZip] [char](5) NULL,
	[InvoiceAddr] [nvarchar](150) NULL,
	[RecvName] [nvarchar](50) NULL,
	[RecvEngName] [varchar](50) NULL,
	[RecvTelDay] [varchar](30) NULL,
	[RecvTelNight] [varchar](30) NULL,
	[RecvMobile] [varchar](30) NULL,
	[DelivType] [int] NULL,
	[DelivData] [nvarchar](50) NULL,
	[DelivLOC] [nvarchar](15) NULL,
	[DelivZip] [char](5) NULL,
	[DelivADDR] [nvarchar](150) NULL,
	[DelivEngADDR] [varchar](150) NULL,
	[DelivHitNote] [nvarchar](100) NULL,
	[ConfirmDate] [datetime] NULL,
	[ConfirmNote] [nvarchar](50) NULL,
	[AuthDate] [datetime] NULL,
	[AuthCode] [varchar](50) NULL,
	[AuthNote] [nvarchar](50) NULL,
	[HpType] [int] NULL,
	[RcptDate] [datetime] NULL,
	[RcptNote] [nvarchar](50) NULL,
	[Expire] [datetime] NULL,
	[DateDEL] [datetime] NULL,
	[CoServerName] [nvarchar](50) NULL,
	[ServerName] [nvarchar](50) NULL,
	[ActCode] [nvarchar](15) NULL,
	[Status] [int] NULL,
	[StatusNote] [nvarchar](50) NULL,
	[RemoteIP] [char](15) NULL,
	[Date] [datetime] NULL,
	[Note] [nvarchar](100) NULL,
	[Note2] [nvarchar](50) NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NOT NULL,
	[Updated] [int] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[WareHouse] [int] NULL,
	[PayTypeID] [int] NULL,
	[invoiceCarrierType] [varchar](20) NULL,
	[invoiceCarrierId1] [varchar](100) NULL,
	[invoiceCarrierId2] [varchar](100) NULL,
	[invoiceDonateCode] [varchar](20) NULL,
 CONSTRAINT [PK_salesorderTemp] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
