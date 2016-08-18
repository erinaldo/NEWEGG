USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[starproduct]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[starproduct](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryID] [int] NOT NULL,
	[CategoryLayer] [int] NOT NULL,
	[ManufactureID] [int] NOT NULL CONSTRAINT [DF_starproduct_starproduct_manufactureid]  DEFAULT ((0)),
	[ItemID] [int] NOT NULL,
	[SellerID] [int] NOT NULL,
	[ShowType] [int] NOT NULL CONSTRAINT [DF_starproduct_starproduct_showtype]  DEFAULT ((0)),
	[StarProductOrder] [int] NOT NULL,
	[DateStart] [datetime] NOT NULL,
	[DateEnd] [datetime] NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_starproduct_starproduct_createdate]  DEFAULT (getdate()),
	[Updated] [int] NOT NULL CONSTRAINT [DF_starproduct_starproduct_updated]  DEFAULT ((0)),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
	[StarTitle] [nvarchar](100) NULL,
 CONSTRAINT [PK_starproduct_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
