USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[bankcodemessage]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[bankcodemessage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BankCode] [char](10) NOT NULL,
	[TradeMethod] [int] NOT NULL,
	[MsgCode] [varchar](20) NOT NULL,
	[MsgDescription] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_bankcodemessage] PRIMARY KEY CLUSTERED 
(
	[BankCode] ASC,
	[TradeMethod] ASC,
	[MsgCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'reference bank.code' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'bankcodemessage', @level2type=N'COLUMN',@level2name=N'BankCode'
GO
