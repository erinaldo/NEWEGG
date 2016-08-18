USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[smssubmit]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[smssubmit](
	[SmsID] [int] IDENTITY(1,1) NOT NULL,
	[ToNumber] [nvarchar](1000) NULL,
	[MessageNo] [int] NULL,
	[MessageContent] [nvarchar](1000) NULL,
	[SmsDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[SmsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
