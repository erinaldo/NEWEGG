USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[changelog]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[changelog](
	[changelog_id] [int] IDENTITY(1,1) NOT NULL,
	[changelog_tablename] [varchar](50) NOT NULL,
	[changelog_columnname] [varchar](50) NOT NULL,
	[changelog_oldvalue] [nvarchar](150) NULL,
	[changelog_newvalue] [nvarchar](150) NULL,
	[changelog_sellerid] [int] NOT NULL,
	[changelog_updated] [int] NOT NULL,
	[changelog_updateuser] [nvarchar](50) NOT NULL,
	[changelog_updatedate] [datetime] NOT NULL,
 CONSTRAINT [PK_changelog_id] PRIMARY KEY CLUSTERED 
(
	[changelog_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[changelog] ADD  CONSTRAINT [DF_changelog_changelog_updated]  DEFAULT ((0)) FOR [changelog_updated]
GO
ALTER TABLE [dbo].[changelog] ADD  CONSTRAINT [DF_changelog_changelog_updatedate]  DEFAULT (getdate()) FOR [changelog_updatedate]
GO
