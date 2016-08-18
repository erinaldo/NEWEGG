USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[NCCCTrans]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[NCCCTrans](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[OrderID] [varchar](50) NULL,
	[PAN] [varchar](19) NULL,
	[ExpireDate] [char](10) NULL,
	[TransCode] [char](2) NULL,
	[TransMode] [char](1) NULL,
	[TransAmt] [varchar](8) NULL,
	[TransDate] [varchar](8) NULL,
	[TransTime] [varchar](6) NULL,
	[ApproveCode] [varchar](8) NULL,
	[ResponseCode] [char](10) NULL,
	[ResponseMsg] [varchar](60) NULL,
	[Installment] [varchar](2) NULL,
	[InstallmentType] [char](1) NULL,
	[FirstAmt] [varchar](8) NULL,
	[EachAmt] [varchar](8) NULL,
	[Fee] [varchar](6) NULL,
	[RedeemBalance] [varchar](8) NULL,
	[RedeemType] [char](1) NULL,
	[RedeemUsed] [varchar](8) NULL,
	[CreditAmt] [varchar](8) NULL,
	[RiskMark] [char](1) NULL,
	[IsForeign] [char](1) NULL,
	[SecureStatus] [char](1) NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL CONSTRAINT [DF_NCCCTrans_CreateDate]  DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_NCCCTrans_UpdateDate]  DEFAULT (getdate()),
 CONSTRAINT [PK_NCCCTrans] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
