USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[HiLifeF09Body]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HiLifeF09Body](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HeadId] [int] NULL,
	[Rdfmt] [char](1) NOT NULL,
	[Ret_m] [varchar](3) NOT NULL,
	[Ecno] [varchar](3) NOT NULL,
	[Odno] [nvarchar](11) NOT NULL,
	[Flag] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_F09Body] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[HiLifeF09Body] ADD  DEFAULT ((0)) FOR [Flag]
GO
ALTER TABLE [dbo].[HiLifeF09Body] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
