USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ItemWarranty]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ItemWarranty](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Itemid] [int] NOT NULL,
	[Createdate] [date] NULL,
	[Status] [int] NULL,
	[Update] [date] NULL,
	[Updated] [int] NULL,
	[Updateuser] [char](10) NULL,
	[ProductProperty] [int] NOT NULL,
	[Desc] [int] NOT NULL,
 CONSTRAINT [PK_ItemWarranty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
