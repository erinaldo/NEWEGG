USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[SMSMessageRecord]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SMSMessageRecord](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SalesOrderCode] [char](15) NULL,
	[ProblemCode] [char](15) NULL,
	[SMSMemberGroupID] [int] NULL,
	[ReceiverName] [nvarchar](50) NOT NULL,
	[ReceiverMobilePhone] [varchar](20) NOT NULL,
	[MessageTypeID] [int] NOT NULL,
	[MessageContent] [nvarchar](700) NOT NULL,
	[SendDate] [datetime] NULL,
	[IsSend] [bit] NOT NULL CONSTRAINT [DF_SMSMessageRecord_IsSend]  DEFAULT ((0)),
	[SenderName] [nvarchar](20) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_SMSMessageRecord] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
