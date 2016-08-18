USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[categorytranslate]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[categorytranslate](
	[ID] [int] NOT NULL,
	[CountryID] [int] NOT NULL,
	[Description] [nvarchar](150) NOT NULL,
	[Createuser] [nvarchar](50) NOT NULL,
	[Createdate] [datetime] NOT NULL,
	[Updateuser] [nvarchar](50) NULL,
	[Updated] [int] NULL,
	[Updatedate] [datetime] NULL,
 CONSTRAINT [PK_categorytranslate_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
