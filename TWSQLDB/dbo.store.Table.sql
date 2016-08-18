USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[store]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[store](
	[ID] [int] NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
	[DEPA] [nvarchar](50) NOT NULL,
	[Showall] [int] NOT NULL,
	[ShowOrder] [int] NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[Updated] [int] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_store_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[store] ADD  CONSTRAINT [DF_store_store_createdate]  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[store] ADD  CONSTRAINT [DF_store_store_updated]  DEFAULT ((0)) FOR [Updated]
GO
