USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[refund2c]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[refund2c](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](50) NULL,
	[Date] [datetime] NULL,
	[ReFund2cUser] [nvarchar](50) NULL,
	[PayDate] [datetime] NULL,
	[PostAge] [int] NULL,
	[Amount] [int] NULL,
	[AmountNOTax] [int] NULL,
	[Amountreal] [int] NULL,
	[FinalDate] [datetime] NULL,
	[Status] [int] NULL,
	[Cause] [int] NULL,
	[RefundType] [int] NULL,
	[BankID] [nvarchar](50) NULL,
	[BankName] [nvarchar](50) NULL,
	[SubBankName] [nvarchar](50) NULL,
	[AccountNO] [varchar](50) NULL,
	[AccountName] [nvarchar](50) NULL,
	[CauseNote] [nvarchar](500) NULL,
	[InvoiceConfirm] [datetime] NULL,
	[ConfirmUser] [nvarchar](50) NULL,
	[ConfirmDate] [datetime] NULL,
	[ApplyUser] [nvarchar](50) NULL,
	[ApplyDate] [datetime] NULL,
	[PreFundDate] [datetime] NULL,
	[ApproveDate] [datetime] NULL,
	[MailNote] [nvarchar](50) NULL,
	[ProcessID] [varchar](15) NULL,
	[RetgoodID] [int] NULL,
	[ApproveUser] [nvarchar](50) NULL,
	[InvoiceResult] [int] NULL,
	[RecvUser] [nvarchar](50) NULL,
	[DiscountDate] [datetime] NULL,
	[DiscountUser] [nvarchar](50) NULL,
	[Finisher] [nvarchar](50) NULL,
	[InvoiceNO] [varchar](50) NULL,
	[InvoiceDate] [datetime] NULL,
	[InvoicePrice] [int] NULL,
	[Refunding] [datetime] NULL,
	[FailCause] [int] NULL,
	[Refund2cFile] [nvarchar](50) NULL,
	[ADJPrice] [int] NULL,
	[StatusDate] [datetime] NULL,
	[CreateDate] [datetime] NULL,
	[Updated] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[CartID] [varchar](15) NULL,
	[Note] [nvarchar](500) NULL,
	[OnRefundDate] [datetime] NULL,
	[AbnRefundDate] [datetime] NULL,
	[CancelRefundDate] [datetime] NULL,
	[UpdateNote] [nvarchar](1500) NULL,
	[ABNRefundReason] [nvarchar](500) NULL,
	[CreateUser] [nchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
