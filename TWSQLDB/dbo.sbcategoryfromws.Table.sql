USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[sbcategoryfromws]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[sbcategoryfromws](
	[SN] [int] IDENTITY(1,1) NOT NULL,
	[NodeID] [int] NOT NULL,
	[StoreID] [int] NOT NULL,
	[CategoryID] [int] NOT NULL,
	[SubcategoryID] [int] NOT NULL,
	[CategoryType] [int] NOT NULL,
	[Description] [varchar](150) NOT NULL,
	[Showall] [int] NOT NULL,
	[ShowOrder] [int] NOT NULL,
 CONSTRAINT [PK_subcategoryfromws_sn] PRIMARY KEY CLUSTERED 
(
	[SN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
