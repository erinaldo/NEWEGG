USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[SMSMember]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SMSMember](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SMSMemberGroupID] [int] NULL,
	[Name] [nvarchar](20) NOT NULL,
	[MobilePhone] [char](10) NULL,
	[CreateUser] [nvarchar](10) NULL,
	[CreateDate] [datetime] NULL,
 CONSTRAINT [PK_SMSMember] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
