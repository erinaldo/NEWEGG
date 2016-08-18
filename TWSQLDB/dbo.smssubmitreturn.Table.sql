USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[smssubmitreturn]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[smssubmitreturn](
	[SmsID] [int] IDENTITY(1,1) NOT NULL,
	[ToNumber] [nvarchar](1000) NULL,
	[ReturnCode] [nvarchar](3) NULL,
	[MessageID] [nvarchar](10) NULL,
	[ReturnDescription] [nvarchar](1000) NULL,
	[SmsDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[SmsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
