USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[cHead]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cHead](
	[OrderID] [varchar](13) NOT NULL,
	[TranDate] [varchar](20) NULL,
	[BuyDate] [varchar](8) NULL,
	[BuyTime] [varchar](6) NOT NULL,
	[PayDate] [varchar](8) NULL,
	[PayTime] [varchar](6) NULL,
	[RMADate] [varchar](8) NULL,
	[RMAUser] [nvarchar](50) NULL,
	[ReceiverName] [nvarchar](50) NULL,
	[ReceiverAddress] [nvarchar](200) NULL,
	[ReceiverTele] [nvarchar](20) NULL,
	[ReceiverPhone] [varchar](20) NULL,
	[ReceiverSex] [varchar](1) NULL,
	[ReceiverZip] [varchar](3) NULL,
	[ReceiverCompanyCode] [varchar](8) NULL,
	[NeweggAccount] [nvarchar](13) NULL,
	[CreditCard] [varchar](4) NULL,
	[VeriCode] [varchar](8) NULL,
	[RealPrice] [varchar](8) NULL,
	[InUserID] [int] NULL,
	[InDate] [datetime] NULL CONSTRAINT [DF_cHead_Indate]  DEFAULT (getdate()),
	[UpdateUserID] [int] NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_cHead_Updatedate]  DEFAULT (getdate()),
	[ExportDate] [datetime] NULL,
 CONSTRAINT [PK_cHead] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
