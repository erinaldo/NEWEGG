USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[category]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[category](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](150) NULL,
	[Layer] [int] NOT NULL,
	[ParentID] [int] NOT NULL,
	[CategoryfromwsID] [int] NOT NULL,
	[Showorder] [int] NOT NULL,
	[SellerID] [int] NOT NULL,
	[DeviceID] [int] NOT NULL,
	[ShowAll] [int] NOT NULL,
	[VerSion] [int] NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_category_category_createdate]  DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NULL,
	[Updated] [int] NULL CONSTRAINT [DF_category_category_updated]  DEFAULT ((0)),
	[UpdateDate] [datetime] NULL,
	[TranslateCountryID] [int] NULL,
	[TranslateID] [int] NULL,
	[GrossMargin] [decimal](10, 2) NOT NULL DEFAULT ((0)),
	[ClassName] [varchar](50) NULL,
	[IsMobile] [int] NULL,
	[MobileOrder] [int] NULL,
	[ImagePath] [nvarchar](256) NULL,
	[ImageHref] [nvarchar](256) NULL,
	[SubTitle] [nvarchar](100) NULL,
 CONSTRAINT [PK_category_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
