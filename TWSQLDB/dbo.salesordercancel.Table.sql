USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[salesordercancel]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[salesordercancel](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SalesorderCode] [varchar](15) NOT NULL,
	[ItemID] [int] NOT NULL,
	[CauseNote] [nvarchar](500) NULL,
	[BankID] [nvarchar](50) NULL,
	[BankName] [nvarchar](50) NULL,
	[BankBranch] [nvarchar](30) NULL,
	[AccountNO] [varchar](30) NULL,
	[AccountName] [nvarchar](50) NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_salesordercancel_salesordercancel_createdate]  DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NULL,
	[Updated] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[CreateUser] [nchar](50) NULL,
 CONSTRAINT [PK_salesordercancel_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
