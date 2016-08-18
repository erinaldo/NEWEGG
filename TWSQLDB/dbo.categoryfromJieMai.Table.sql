USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[categoryfromJieMai]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[categoryfromJieMai](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryID] [int] NULL,
	[NValue] [nvarchar](50) NULL,
	[Title_CH] [nvarchar](50) NULL,
	[Title_EN] [nvarchar](50) NULL,
	[Description] [nvarchar](150) NULL,
	[SortOrder] [int] NULL,
	[Layer] [int] NULL,
	[ParentID] [int] NULL,
	[Showorder] [int] NULL,
	[SellerID] [int] NULL,
	[DeviceID] [int] NULL,
	[ShowAll] [int] NULL,
	[Version] [int] NULL,
 CONSTRAINT [PK_categoryfromJieMai_sn] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
