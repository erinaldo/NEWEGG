USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[subcategorylogo]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[subcategorylogo](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SubCategoryID] [int] NOT NULL,
	[ManufactureID] [int] NULL,
	[Showorder] [int] NOT NULL,
	[ShowAll] [int] NOT NULL,
	[Clickpath] [nvarchar](500) NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_logonormal_CateDate]  DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NULL,
	[Updated] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[ImageUrl] [nvarchar](256) NULL,
 CONSTRAINT [PK_logonormal] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
