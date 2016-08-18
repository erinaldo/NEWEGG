USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[categoryfromws]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[categoryfromws](
	[SN] [int] IDENTITY(1,1) NOT NULL,
	[ID] [int] NOT NULL,
	[CategoryID] [int] NOT NULL,
	[NValue] [nvarchar](50) NULL,
	[Title] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](150) NULL,
	[Layer] [int] NOT NULL CONSTRAINT [DF_categoryfromws_categoryfromws_layer]  DEFAULT ((0)),
	[ParentID] [int] NOT NULL CONSTRAINT [DF_Table_1_categoryfromws_parent_id]  DEFAULT ((0)),
	[Showorder] [int] NOT NULL CONSTRAINT [DF_categoryfromws_categoryfromws_showorder]  DEFAULT ((0)),
	[SellerID] [int] NOT NULL,
	[DeviceID] [int] NOT NULL,
	[ShowAll] [int] NOT NULL CONSTRAINT [DF_categoryfromws_categoryfromws_showall]  DEFAULT ((1)),
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_categoryfromws_sn] PRIMARY KEY CLUSTERED 
(
	[SN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
