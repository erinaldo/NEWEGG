--癈四機同意

USE TWSQLDB
--USE TWSQLDB_PRD
GO

/****** Object:  Table [dbo].[Discard4]    Script Date: 04/28/2016 09:15:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Discard4](
	[ID] [int] IDENTITY(1,1) NOT NULL,	
	[SalesOrderGroupID] [int] NOT NULL,
	[AgreedDiscard4] [nvarchar](50) NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	
 CONSTRAINT [PK_Discard4] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Discard4] ADD  DEFAULT ('') FOR [AgreedDiscard4]
ALTER TABLE [dbo].[Discard4] ADD  DEFAULT ('') FOR [CreateUser]
ALTER TABLE [dbo].[Discard4] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO


