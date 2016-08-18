USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[tradecategory]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tradecategory](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SubcategoryID] [int] NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Rate] [decimal](10, 2) NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_tradecategory_tradecategory_createdate]  DEFAULT (getdate()),
	[Updated] [int] NULL CONSTRAINT [DF_tradecategory_tradecategory_updated]  DEFAULT ((0)),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_tradecategory_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
