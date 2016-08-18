USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[auth]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[auth](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SalesOrderItemCode] [varchar](15) NOT NULL CONSTRAINT [DF_auth_auth_orderitemcode]  DEFAULT (''),
	[AcqBank] [char](6) NULL CONSTRAINT [DF_auth_auth_acqbank]  DEFAULT (''),
	[CustomerID] [varchar](20) NULL CONSTRAINT [DF_auth_auth_customerid]  DEFAULT (''),
	[AgreementID] [char](10) NULL CONSTRAINT [DF_auth_auth_agreementid]  DEFAULT (''),
	[AccountID] [int] NULL CONSTRAINT [DF_auth_auth_userid]  DEFAULT (''),
	[OrderNO] [nvarchar](30) NULL CONSTRAINT [DF_auth_auth_orderno]  DEFAULT (''),
	[Qty] [int] NULL CONSTRAINT [DF_auth_auth_qty]  DEFAULT ((0)),
	[Amount] [int] NULL CONSTRAINT [DF_auth_auth_amount]  DEFAULT ((0)),
	[AmountSelf] [int] NULL CONSTRAINT [DF_auth_auth_amtself]  DEFAULT ((0)),
	[Bonus] [int] NULL CONSTRAINT [DF_auth_auth_bonus]  DEFAULT ((0)),
	[BonusBLN] [int] NULL CONSTRAINT [DF_auth_auth_bonusbln]  DEFAULT ((0)),
	[HpMark] [char](1) NULL CONSTRAINT [DF_auth_auth_hpmark]  DEFAULT (''),
	[PriceFirst] [int] NULL CONSTRAINT [DF_auth_auth_pricefirst]  DEFAULT ((0)),
	[PriceOther] [int] NULL CONSTRAINT [DF_auth_auth_priceother]  DEFAULT ((0)),
	[AuthCode] [varchar](50) NULL CONSTRAINT [DF_auth_auth_authcode]  DEFAULT (''),
	[AuthSN] [varchar](50) NULL CONSTRAINT [DF_auth_auth_authsn]  DEFAULT (''),
	[AuthDate] [datetime] NOT NULL CONSTRAINT [DF_auth_auth_authdate]  DEFAULT ('1900/1/1'),
	[RspCode] [char](20) NULL CONSTRAINT [DF_auth_auth_rspcode]  DEFAULT (''),
	[RspMSG] [nvarchar](80) NULL CONSTRAINT [DF_auth_auth_rspmsg]  DEFAULT (''),
	[RspOther] [nvarchar](200) NULL CONSTRAINT [DF_auth_auth_rspother]  DEFAULT (''),
	[CancelDate] [datetime] NOT NULL CONSTRAINT [DF_auth_auth_canceldate]  DEFAULT ('1900/1/1'),
	[CancelRspCode] [char](3) NULL CONSTRAINT [DF_auth_auth_cancelrspcode]  DEFAULT (''),
	[CancelRspMSG] [nvarchar](80) NULL CONSTRAINT [DF_auth_auth_cancelrspmsg]  DEFAULT (''),
	[FaildealUser] [nvarchar](50) NULL CONSTRAINT [DF_auth_auth_faildealuser]  DEFAULT (''),
	[FaildealNote] [nvarchar](50) NULL CONSTRAINT [DF_auth_auth_faildealnote]  DEFAULT (''),
	[FaildealDate] [datetime] NOT NULL CONSTRAINT [DF_auth_auth_faildealdate]  DEFAULT ('1900/1/1'),
	[EraseDate] [datetime] NOT NULL CONSTRAINT [DF_auth_auth_erasedate]  DEFAULT ('1900/1/1'),
	[CreateUser] [nvarchar](50) NULL CONSTRAINT [DF_auth_auth_createuser]  DEFAULT (''),
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_auth_auth_sysdate]  DEFAULT (getdate()),
	[Updated] [int] NULL CONSTRAINT [DF_auth_auth_updated]  DEFAULT ((0)),
	[UpdateUser] [nvarchar](50) NULL CONSTRAINT [DF_auth_auth_updateduser]  DEFAULT (''),
	[UpdateDate] [datetime] NOT NULL CONSTRAINT [DF_auth_auth_updateddate]  DEFAULT ('1900/1/1'),
	[SuccessFlag] [char](1) NULL,
	[SalesOrderGroupID] [int] NULL CONSTRAINT [DF_auth_SalesOrderGroupID]  DEFAULT ((0)),
 CONSTRAINT [PK_auth_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1: Succees, 0:fail;' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'auth', @level2type=N'COLUMN',@level2name=N'SuccessFlag'
GO
