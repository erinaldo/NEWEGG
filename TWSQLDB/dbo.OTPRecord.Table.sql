USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[OTPRecord]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OTPRecord](
	[UserID] [int] NOT NULL,
	[Items] [varchar](200) NULL,
	[CartID] [varchar](10) NULL,
	[CreateDate] [datetime] NULL,
	[Amount] [int] NULL,
	[Phone] [varchar](30) NULL,
	[Status] [int] NULL,
	[StatusDate] [datetime] NULL,
	[FailCount] [int] NULL,
	[Password] [varchar](6) NULL,
	[SMSReturnID] [int] NULL,
 CONSTRAINT [PK_OTPRecord] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
