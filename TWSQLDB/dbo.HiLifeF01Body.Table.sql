USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[HiLifeF01Body]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HiLifeF01Body](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HeadId] [int] NOT NULL,
	[Rdfmt] [char](1) NOT NULL,
	[Stno] [varchar](7) NOT NULL,
	[Stnm] [nvarchar](18) NOT NULL,
	[Sttel] [nvarchar](20) NOT NULL,
	[Stcity] [nvarchar](10) NOT NULL,
	[Stcntry] [nvarchar](10) NOT NULL,
	[Stadr] [nvarchar](52) NOT NULL,
	[Zipcd] [varchar](5) NOT NULL,
	[Dcrono] [nvarchar](20) NOT NULL,
	[Sdate] [datetime] NOT NULL,
	[Edate] [datetime] NOT NULL,
	[Flag] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_F01-Body] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[HiLifeF01Body] ADD  DEFAULT ((0)) FOR [Flag]
GO
ALTER TABLE [dbo].[HiLifeF01Body] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
