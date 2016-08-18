USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[mynewegg]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mynewegg](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Type] [int] NOT NULL,
	[GroupID] [int] NOT NULL,
	[TypeName] [nvarchar](30) NOT NULL,
	[ItemID] [int] NOT NULL,
	[MyNeweggOrder] [int] NOT NULL,
	[DateStart] [datetime] NULL,
	[DateEnd] [datetime] NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Updated] [int] NOT NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_mynewegg_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[mynewegg] ADD  CONSTRAINT [DF_mynewegg_mynewegg_type]  DEFAULT ((0)) FOR [Type]
GO
ALTER TABLE [dbo].[mynewegg] ADD  CONSTRAINT [DF_mynewegg_mynewegg_groupid]  DEFAULT ((0)) FOR [GroupID]
GO
ALTER TABLE [dbo].[mynewegg] ADD  CONSTRAINT [DF_mynewegg_mynewegg_createdate]  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[mynewegg] ADD  CONSTRAINT [DF_mynewegg_mynewegg_updated]  DEFAULT ((0)) FOR [Updated]
GO
