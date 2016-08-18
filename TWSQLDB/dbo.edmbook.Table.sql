USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[edmbook]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[edmbook](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AdvTypeCode] [int] NOT NULL,
	[EDMName] [nvarchar](50) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[ViewName] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [nvarchar](100) NOT NULL,
	[Updated] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](100) NULL,
	[HtmlContext] [nvarchar](max) NULL,
	[LettersRecord] [nvarchar](max) NULL,
	[TrackingCode] [nvarchar](500) NULL,
	[LandingFromEDM] [int] NOT NULL CONSTRAINT [DF_edmbook_LandingFromEDM]  DEFAULT ((0)),
	[EDMDisplay] [int] NOT NULL DEFAULT ((0)),
	[OpenLetter] [int] NOT NULL DEFAULT ((0)),
	[RepeatOpenLetter] [int] NOT NULL DEFAULT ((0)),
	[AllMembers] [int] NOT NULL DEFAULT ((0)),
	[RecipientsList] [nvarchar](max) NULL,
	[AutoSendDate] [datetime] NULL,
 CONSTRAINT [PK_edmbook] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
