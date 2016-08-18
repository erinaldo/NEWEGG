USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[CartTemp]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CartTemp](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SerialNumber] [varchar](200) NOT NULL,
	[AccountID] [int] NOT NULL,
	[PayType] [int] NULL,
	[BankID] [int] NULL,
	[PayTypeGroupID] [int] NULL,
	[CartTypeID] [int] NULL,
	[SalesOrderGroupID] [int] NULL,
	[Status] [int] NOT NULL CONSTRAINT [DF_CartTemp_Status]  DEFAULT ((0)),
	[IPAddress] [varchar](100) NULL,
	[CreateDate] [datetime] NULL,
	[CreateUser] [nvarchar](50) NULL,
	[Updated] [int] NULL CONSTRAINT [DF_CartTemp_Updated]  DEFAULT ((0)),
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
 CONSTRAINT [PK_CartTemp] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改次數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CartTemp', @level2type=N'COLUMN',@level2name=N'Updated'
GO
