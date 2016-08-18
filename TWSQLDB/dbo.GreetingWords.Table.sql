USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[GreetingWords]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GreetingWords](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](50) NULL DEFAULT (''),
	[Showorder] [int] NOT NULL DEFAULT ((0)),
	[ShowAll] [int] NOT NULL DEFAULT ((0)),
	[Clickpath] [nvarchar](500) NULL DEFAULT (''),
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[CategoryId] [int] NOT NULL DEFAULT ((0)),
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[CodeText] [nvarchar](50) NOT NULL DEFAULT (''),
	[ImageUrl] [nvarchar](100) NOT NULL DEFAULT (''),
 CONSTRAINT [PK_GreetingWords] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
