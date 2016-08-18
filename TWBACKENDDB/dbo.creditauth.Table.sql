USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[creditauth]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[creditauth](
	[ID] [int] NOT NULL,
	[Blng] [nvarchar](50) NULL,
	[BlngID] [int] NULL,
	[ProcessID] [int] NULL,
	[AuthID] [int] NULL,
	[PayGateway] [int] NULL,
	[OrderitemCode] [nvarchar](50) NULL,
	[DealType] [int] NULL,
	[AcqBank] [nvarchar](50) NULL,
	[CustomerID] [nvarchar](20) NULL,
	[AgreementID] [nvarchar](50) NULL,
	[UserID] [nvarchar](20) NULL,
	[OrderNO] [nvarchar](50) NULL,
	[Qty] [int] NULL,
	[Amnt] [decimal](10, 2) NULL,
	[AmountSelf] [decimal](10, 2) NULL,
	[AmountApply] [decimal](10, 2) NULL,
	[Bonus] [decimal](10, 2) NULL,
	[Bonusbln] [int] NULL,
	[Bonusrate] [decimal](10, 2) NULL,
	[HpMark] [nvarchar](50) NULL,
	[PriceFirst] [int] NULL,
	[PriceOther] [int] NULL,
	[AuthDate] [datetime] NULL,
	[AuthExpfile] [nvarchar](50) NULL,
	[AuthCode] [varchar](50) NULL,
	[AuthSN] [nvarchar](50) NULL,
	[AuthrspDate] [datetime] NULL,
	[AuthrspimpFile] [nvarchar](50) NULL,
	[AuthrspCode] [nvarchar](50) NULL,
	[AuthrspMSG] [nvarchar](50) NULL,
	[AuthrspOther] [nvarchar](50) NULL,
	[CancelDate] [datetime] NULL,
	[CancelrspCode] [nvarchar](50) NULL,
	[CancelrspMSG] [nvarchar](50) NULL,
	[Status] [int] NULL,
	[StatusDate] [datetime] NULL,
	[Note] [nvarchar](50) NULL,
	[EraseDate] [datetime] NULL,
	[FaildealUser] [nvarchar](50) NULL,
	[FaildealNote] [nvarchar](200) NULL,
	[FailDealDate] [datetime] NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[Updated] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[SuccessFlag] [char](1) NULL,
	[SalesOrderGroupID] [int] NOT NULL CONSTRAINT [DF_creditauth_SalesOrderGroupID]  DEFAULT ((0)),
 CONSTRAINT [PK_creditauth_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
