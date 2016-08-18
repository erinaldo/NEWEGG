USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[manufacture]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[manufacture](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_manufacture_manufacture_createdate]  DEFAULT (getdate()),
	[Updated] [int] NOT NULL CONSTRAINT [DF_manufacture_manufacture_updated]  DEFAULT ((0)),
	[UpdateUser] [nvarchar](50) NULL,
	[Updatedate] [datetime] NULL,
	[Showorder] [int] NULL CONSTRAINT [DF_manufacture_manufacture_showorder]  DEFAULT ((999)),
	[Status] [int] NULL CONSTRAINT [DF_manufacture_manufacture_status]  DEFAULT ((0)),
	[WebAddress] [nvarchar](200) NULL,
	[Phone] [nvarchar](50) NULL,
	[Address] [nvarchar](200) NULL,
	[SourceContry] [nchar](10) NULL,
	[BrandStory] [nvarchar](max) NULL,
 CONSTRAINT [PK_manufacture_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
