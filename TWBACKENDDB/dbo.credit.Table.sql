USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[credit]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[credit](
	[ID] [int] NOT NULL,
	[StoreID] [int] NULL,
	[DealType] [int] NULL,
	[Dealid] [int] NULL,
	[CreditConfirm] [datetime] NULL,
	[Reply] [datetime] NULL,
	[ReplyUser] [nvarchar](50) NULL,
	[ReplyAmnt] [int] NULL,
	[Status] [int] NULL,
	[CheckType] [int] NULL,
	[CheckNote] [nvarchar](200) NULL,
	[PayType] [int] NULL,
	[PayDate] [datetime] NULL,
	[InvoiceNO] [nvarchar](10) NULL,
	[InvoiceDate] [datetime] NULL,
	[FinalDate] [datetime] NULL,
	[AuthType] [int] NULL,
	[AuthCode] [varchar](50) NULL,
	[AuthSN] [nvarchar](20) NULL,
	[VerifyEmail1] [datetime] NULL,
	[VerifyEmail2] [datetime] NULL,
	[VerifyNote] [nvarchar](50) NULL,
	[Chequerecv] [datetime] NULL,
	[ChequePay] [datetime] NULL,
	[InvoiceLock] [int] NULL,
	[InvoAmnt] [decimal](10, 2) NULL,
	[StatusDate] [datetime] NULL,
	[CreateDate] [datetime] NULL,
	[Updated] [int] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_credit_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
