USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ItemCategory_log]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ItemCategory_log](
	[UserAction] [varchar](15) NULL,
	[EditDate] [datetime] NULL CONSTRAINT [DF_ItemCategory_ItemCategory_log_EditDate]  DEFAULT (getdate()),
	[ItemID] [int] NULL,
	[CategoryID] [int] NULL,
	[FromSystem] [char](1) NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
