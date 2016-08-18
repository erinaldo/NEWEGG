--癈四機回收四聯單

USE TWBACKENDDB
--USE TWBACKENDDB_PRD
GO




/****** Object:  Table [dbo].[Discard4Item]    Script Date: 04/28/2016 09:15:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PhoneAdSub](
	[ID] [int] IDENTITY(1000,1) NOT NULL,
	[PhoneAdID] [int] NOT NULL,	
	
	[CategoryID] [nvarchar](50) NOT NULL,
	[SalesorderitemCode] [nvarchar](50) NOT NULL,
	--[ItemID] [int] NOT NULL,	
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Discard4Item] ADD  DEFAULT (0) FOR [ItemID]
ALTER TABLE [dbo].[Discard4Item] ADD  DEFAULT ('') FOR [SalesorderCode]
ALTER TABLE [dbo].[Discard4Item] ADD  DEFAULT ('') FOR [SalesorderitemCode]
--ALTER TABLE [dbo].[Discard4Item] ADD  DEFAULT (0) FOR [ItemID]
ALTER TABLE [dbo].[Discard4Item] ADD  DEFAULT ('') FOR [NumberCode]
ALTER TABLE [dbo].[Discard4Item] ADD  DEFAULT ('') FOR [Discard4Flag]
ALTER TABLE [dbo].[Discard4Item] ADD  DEFAULT ('') FOR [CreateUser]
ALTER TABLE [dbo].[Discard4Item] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO


