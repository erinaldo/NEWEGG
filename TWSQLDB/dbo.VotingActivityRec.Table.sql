USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[VotingActivityRec]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VotingActivityRec](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GroupId] [int] NOT NULL,
	[AccountId] [varchar](50) NOT NULL,
	[AccountSource] [nvarchar](50) NOT NULL,
	[VoteDate] [varchar](10) NOT NULL,
	[Email] [varchar](256) NULL,
	[Rec] [nvarchar](max) NULL,
 CONSTRAINT [PK_VotingActivityRec] PRIMARY KEY CLUSTERED 
(
	[GroupId] DESC,
	[VoteDate] ASC,
	[AccountSource] ASC,
	[AccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
