USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[subcategoryfromws]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[subcategoryfromws](
	[SubcategoryID] [int] NOT NULL,
	[NodeID] [int] NOT NULL,
	[StoreID] [int] NOT NULL,
	[CategoryID] [int] NOT NULL,
	[NValue] [nvarchar](50) NULL,
	[CategoryType] [int] NOT NULL,
	[Description] [nvarchar](150) NOT NULL,
	[Showall] [int] NOT NULL,
	[ShowOrder] [int] NOT NULL,
	[SN] [int] NULL,
 CONSTRAINT [PK_subcategoryfromws_subcategoryid] PRIMARY KEY CLUSTERED 
(
	[SubcategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
