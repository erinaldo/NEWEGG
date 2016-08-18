USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[HiLifeF00Body]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HiLifeF00Body](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Stno] [varchar](7) NOT NULL,
	[New_Stno] [varchar](7) NOT NULL,
	[Dcrono] [varchar](5) NOT NULL,
	[Flag] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_F00Body] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[HiLifeF00Body] ADD  DEFAULT ((0)) FOR [Flag]
GO
ALTER TABLE [dbo].[HiLifeF00Body] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
