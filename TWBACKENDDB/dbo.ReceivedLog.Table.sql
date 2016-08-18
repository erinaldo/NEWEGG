USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[ReceivedLog]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ReceivedLog](
	[Index] [int] IDENTITY(1,1) NOT NULL,
	[POCode] [varchar](15) NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_ReceivedLog_CreateDate]  DEFAULT (getdate()),
	[Status] [int] NOT NULL DEFAULT ((0)),
	[IsConsolidated] [bit] NOT NULL DEFAULT ((0)),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_ReceivedLog_UpdateDate]  DEFAULT (getdate()),
 CONSTRAINT [PK_dbo.ReceivedLog] PRIMARY KEY CLUSTERED 
(
	[Index] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
