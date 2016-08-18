USE [TWSQLDB]
GO

/****** Object:  Table [dbo].[GreetingWords]    Script Date: 04/15/2016 09:15:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GreetingWords](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](50) NULL,
	[Showorder] [int] NOT NULL,
	[ShowAll] [int] NOT NULL,
	[Clickpath] [nvarchar](500) NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[CategoryId] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[CodeText] [nvarchar](50) NOT NULL,
	[ImageUrl] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_GreetingWords] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[GreetingWords] ADD  DEFAULT ('') FOR [Description]
GO

ALTER TABLE [dbo].[GreetingWords] ADD  DEFAULT ((0)) FOR [Showorder]
GO

ALTER TABLE [dbo].[GreetingWords] ADD  DEFAULT ((0)) FOR [ShowAll]
GO

ALTER TABLE [dbo].[GreetingWords] ADD  DEFAULT ('') FOR [Clickpath]
GO

ALTER TABLE [dbo].[GreetingWords] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO

ALTER TABLE [dbo].[GreetingWords] ADD  DEFAULT ((0)) FOR [CategoryId]
GO

ALTER TABLE [dbo].[GreetingWords] ADD  DEFAULT ('') FOR [CodeText]
GO

ALTER TABLE [dbo].[GreetingWords] ADD  DEFAULT ('') FOR [ImageUrl]
GO


