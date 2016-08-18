USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[VotingActivityItems]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VotingActivityItems](
	[GroupId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[Title] [nvarchar](50) NULL,
	[Description] [nvarchar](100) NULL,
	[DisplayStartDate] [datetime] NOT NULL,
	[DisplayEndDate] [datetime] NOT NULL,
	[VotingStartDate] [datetime] NOT NULL,
	[VotingEndDate] [datetime] NOT NULL,
	[OnlineStatus] [int] NOT NULL DEFAULT ((0)),
	[InitVoting] [int] NOT NULL DEFAULT ((0)),
	[SystemVoting] [int] NOT NULL DEFAULT ((0)),
	[RealVoting] [int] NOT NULL DEFAULT ((0)),
	[ShowOrder] [int] NOT NULL DEFAULT ((1)),
	[CreateUser] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL DEFAULT (getdate()),
	[UpdateUser] [int] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_VotingActivityItems] PRIMARY KEY CLUSTERED 
(
	[GroupId] ASC,
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
