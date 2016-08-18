USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ItemGroupDetailProperty]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemGroupDetailProperty](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[GroupID] [int] NOT NULL,
	[ItemID] [int] NULL,
	[ItemTempID] [int] NULL,
	[SellerID] [int] NOT NULL,
	[MasterPropertyID] [int] NOT NULL,
	[PropertyID] [int] NULL,
	[GroupValueID] [int] NOT NULL,
	[ValueID] [int] NOT NULL,
	[ValueName] [nvarchar](500) NOT NULL,
	[InputValue] [nvarchar](100) NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_ItemGroupDetailProperty_CreateDate]  DEFAULT (getdate()),
	[InUser] [int] NOT NULL,
	[UpdateDate] [datetime] NOT NULL CONSTRAINT [DF_ItemGroupDetailProperty_Updatedate]  DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ItemGroupDetailProperty] PRIMARY KEY CLUSTERED 
(
	[GroupID] ASC,
	[MasterPropertyID] ASC,
	[GroupValueID] ASC,
	[ValueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
