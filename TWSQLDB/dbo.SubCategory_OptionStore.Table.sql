USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[SubCategory_OptionStore]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubCategory_OptionStore](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryID] [int] NULL,
	[Title] [nvarchar](50) NULL,
	[IsFormat] [bit] NULL,
	[FreeCost] [decimal](10, 2) NULL,
	[SellerID] [int] NULL,
	[ShowAll] [int] NULL,
	[Showorder] [int] NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[Updated] [int] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[DateStart] [datetime] NULL,
	[DateEnd] [datetime] NULL,
	[IsSpecOption] [int] NOT NULL CONSTRAINT [DF_SubCategory_OptionStore_IsSpecOption]  DEFAULT ((0)),
 CONSTRAINT [PK_SubCategory_OptionStore] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
