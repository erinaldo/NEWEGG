USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[NATW_SiteSearch]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[NATW_SiteSearch](
	[GID] [uniqueidentifier] NOT NULL,
	[Keywords] [nvarchar](100) NOT NULL,
	[InnerKeywords] [nvarchar](100) NULL,
	[CreateDate] [datetime] NOT NULL,
	[Device] [varchar](20) NOT NULL,
	[ResultCount] [int] NOT NULL,
	[IPv4] [nvarchar](15) NOT NULL,
	[UserID] [int] NULL,
 CONSTRAINT [PK_NATW_SiteSearch] PRIMARY KEY CLUSTERED 
(
	[GID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[NATW_SiteSearch] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
