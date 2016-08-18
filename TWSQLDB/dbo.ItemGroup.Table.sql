USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ItemGroup]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemGroup](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SellerID] [int] NOT NULL,
	[Amount] [int] NOT NULL,
	[MasterPropertyID] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_ItemGroup_CreateDate]  DEFAULT (getdate()),
	[InUser] [int] NOT NULL,
	[UpdateDate] [datetime] NOT NULL CONSTRAINT [DF_ItemGroup_Updatedate]  DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ItemGroup_1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
