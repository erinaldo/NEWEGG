USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[Discard4Item]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Discard4Item](
	[ID] [int] IDENTITY(1000,1) NOT NULL,
	[SalesorderCode] [nvarchar](50) NOT NULL,
	[SalesorderitemCode] [nvarchar](50) NOT NULL,
	[ItemID] [int] NOT NULL,
	[InstalledDate] [datetime] NULL,
	[NumberCode] [nvarchar](50) NULL,
	[Discard4Flag] [nvarchar](50) NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_Discard4Item] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Discard4Item] ADD  DEFAULT ('') FOR [SalesorderCode]
GO
ALTER TABLE [dbo].[Discard4Item] ADD  DEFAULT ('') FOR [SalesorderitemCode]
GO
ALTER TABLE [dbo].[Discard4Item] ADD  DEFAULT ((0)) FOR [ItemID]
GO
ALTER TABLE [dbo].[Discard4Item] ADD  DEFAULT ('') FOR [NumberCode]
GO
ALTER TABLE [dbo].[Discard4Item] ADD  DEFAULT ('') FOR [Discard4Flag]
GO
ALTER TABLE [dbo].[Discard4Item] ADD  DEFAULT ('') FOR [CreateUser]
GO
ALTER TABLE [dbo].[Discard4Item] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
