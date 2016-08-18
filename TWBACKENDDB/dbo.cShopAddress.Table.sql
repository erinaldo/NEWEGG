USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[cShopAddress]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cShopAddress](
	[TakeStoreID] [varchar](4) NOT NULL,
	[TakeStoreName] [nvarchar](50) NULL,
	[StateID] [varchar](2) NULL,
	[StateName] [nvarchar](50) NULL,
	[TownID] [varchar](3) NULL,
	[TownName] [nvarchar](50) NULL,
	[Address] [nvarchar](150) NULL,
	[Telephone] [nvarchar](20) NULL,
	[Type] [varchar](1) NULL,
	[InUserID] [int] NULL,
	[InDate] [datetime] NULL,
	[UpdateUserID] [int] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_cShopAddress] PRIMARY KEY CLUSTERED 
(
	[TakeStoreID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[cShopAddress] ADD  CONSTRAINT [DF_cShopAddress_Indate]  DEFAULT (getdate()) FOR [InDate]
GO
ALTER TABLE [dbo].[cShopAddress] ADD  CONSTRAINT [DF_cShopAddress_Updatedate]  DEFAULT (getdate()) FOR [UpdateDate]
GO
