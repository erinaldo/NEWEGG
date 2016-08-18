USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[companybook]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[companybook](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Accountid] [int] NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
	[Number] [char](8) NOT NULL,
	[Delivloc] [nvarchar](10) NOT NULL,
	[Delivzip] [char](5) NOT NULL,
	[Delivaddr] [nvarchar](150) NOT NULL,
	[Createdate] [datetime] NOT NULL,
	[Updated] [int] NOT NULL,
	[Updatedate] [datetime] NULL,
	[DelivZipName] [nvarchar](10) NULL,
	[DefaultSetting] [int] NULL,
 CONSTRAINT [PK_companybook_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
