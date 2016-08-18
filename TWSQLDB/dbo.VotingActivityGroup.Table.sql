USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[VotingActivityGroup]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VotingActivityGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Description] [nvarchar](300) NULL,
	[DisplayStartDate] [datetime] NOT NULL,
	[DisplayEndDate] [datetime] NOT NULL,
	[VotingStartDate] [datetime] NOT NULL,
	[VotingEndDate] [datetime] NOT NULL,
	[OnlineStatus] [int] NOT NULL DEFAULT ((0)),
	[RestrictType] [int] NOT NULL DEFAULT ((2)),
	[RestrictLimit] [int] NOT NULL DEFAULT ((1)),
	[VotingItemRepeate] [int] NOT NULL DEFAULT ((0)),
	[RestrictAccount] [int] NOT NULL DEFAULT ((1)),
	[CreateUser] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL DEFAULT (getdate()),
	[UpdateUser] [int] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_VotingActivityGroup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
