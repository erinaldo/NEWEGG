USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[Accounting]    Script Date: 2016/08/18 14:12:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Accounting](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CodeOfTW] [nvarchar](10) NULL,
	[CodeOfUS] [nvarchar](10) NULL,
	[CompanyCode] [nvarchar](10) NULL,
	[DescriptionInCH] [nvarchar](50) NULL,
	[DescriptionInEN] [nvarchar](50) NULL,
 CONSTRAINT [PK_Accounting] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
