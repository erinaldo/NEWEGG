USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[OmusicAccount]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OmusicAccount](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[OmusicSN] [varchar](25) NOT NULL,
	[AccountID] [int] NULL,
	[TakeDate] [datetime] NULL,
	[CreateUserID] [int] NULL,
	[CreateDate] [datetime] NULL CONSTRAINT [DF_OmusicAccount_CreateDate]  DEFAULT (getdate()),
	[UpdateUserID] [int] NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_OmusicAccount_UpdateDate]  DEFAULT (getdate()),
 CONSTRAINT [PK_OmusicAccount_ID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[OmusicSN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
