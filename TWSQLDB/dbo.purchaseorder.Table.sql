USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[purchaseorder]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[purchaseorder](
	[Code] [varchar](15) NOT NULL,
	[SalesorderCode] [varchar](15) NULL,
	[PurchaseorderGroupID] [int] NULL,
	[IDNO] [varchar](20) NULL,
	[Name] [nvarchar](50) NULL,
	[AccountID] [int] NOT NULL,
	[TelDay] [varchar](30) NULL,
	[TelNight] [varchar](30) NULL,
	[Mobile] [varchar](30) NULL,
	[Email] [varchar](256) NULL,
	[PayType] [int] NULL,
	[StarvlDate] [datetime] NULL,
	[CardHolder] [nvarchar](30) NULL,
	[CardTelDay] [varchar](30) NULL,
	[CardTelNight] [varchar](30) NULL,
	[CardMobile] [varchar](30) NULL,
	[CardLOC] [nvarchar](10) NULL,
	[CardZip] [char](5) NULL,
	[CardADDR] [nvarchar](150) NULL,
	[CardNO] [varchar](64) NULL,
	[CardNOCHK] [varchar](30) NULL,
	[CardType] [char](10) NULL,
	[CardBank] [nvarchar](50) NULL,
	[CardExpire] [char](10) NULL,
	[CardBirthday] [datetime] NULL,
	[InvoReceiver] [nvarchar](50) NULL,
	[Invoid] [char](10) NULL,
	[InvoTitle] [nvarchar](50) NULL,
	[InvoLOC] [nvarchar](10) NULL,
	[InvoZip] [char](5) NULL,
	[InvoADDR] [nvarchar](150) NULL,
	[RecvName] [nvarchar](50) NULL,
	[RecvENGName] [varchar](50) NULL,
	[RecvTelDay] [varchar](30) NULL,
	[RecvTelNight] [varchar](30) NULL,
	[RecvMobile] [varchar](30) NULL,
	[DelivType] [int] NULL,
	[DelivData] [nvarchar](50) NULL,
	[DelivLOC] [nvarchar](10) NULL,
	[DelivZip] [char](5) NULL,
	[DelivADDR] [nvarchar](150) NULL,
	[DelivENGADDR] [varchar](150) NULL,
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
	[CoserverName] [varchar](50) NULL,
	[ServerName] [varchar](50) NULL,
	[ActCode] [nvarchar](15) NULL,
	[Status] [int] NULL,
	[StatusNote] [nvarchar](50) NULL,
	[RemoteIP] [char](15) NULL,
	[Date] [datetime] NULL,
	[Note] [nvarchar](100) NULL,
	[Note2] [nvarchar](50) NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL CONSTRAINT [DF_purchaseorder_purchaseorder_createdate]  DEFAULT (getdate()),
	[Updated] [int] NULL CONSTRAINT [DF_purchaseorder_purchaseorder_updated]  DEFAULT ((0)),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[_MarkDelete_20130827_purchaseorder_ccccode] [varchar](15) NULL,
	[_MarkDelete_20130827_purchaseorder_dutyrate] [decimal](10, 2) NULL,
	[_MarkDelete_20130827_purchaseorder_producttax] [decimal](10, 2) NULL,
	[_MarkDelete_20130827_purchaseorder_fulltaxvalue] [decimal](10, 2) NULL,
	[_MarkDelete_20130827_purchaseorder_shippingfee] [decimal](10, 2) NULL,
 CONSTRAINT [PK_purchaseorder_code] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
