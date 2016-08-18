USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ImageUrlReferenceForProduct]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImageUrlReferenceForProduct](
	[ProductID] [int] NOT NULL,
	[Size] [int] NOT NULL,
	[SizeIndex] [int] NOT NULL,
	[ImageUrl] [nvarchar](max) NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[Updated] [int] NOT NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
 CONSTRAINT [PK_ImageUrlReferenceForProduct] PRIMARY KEY CLUSTERED 
(
	[ProductID] ASC,
	[Size] ASC,
	[SizeIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[ImageUrlReferenceForProduct] ADD  CONSTRAINT [DF_ImageUrlReferenceForProduct_CreateUser]  DEFAULT ('') FOR [CreateUser]
GO
ALTER TABLE [dbo].[ImageUrlReferenceForProduct] ADD  CONSTRAINT [DF_ImageUrlReferenceForProduct_Updated]  DEFAULT ((0)) FOR [Updated]
GO
ALTER TABLE [dbo].[ImageUrlReferenceForProduct] ADD  CONSTRAINT [DF_ImageUrlReferenceForProduct_UpdateDate]  DEFAULT ('1900/01/01') FOR [UpdateDate]
GO
ALTER TABLE [dbo].[ImageUrlReferenceForProduct] ADD  CONSTRAINT [DF_ImageUrlReferenceForProduct_UpdateUser]  DEFAULT ('') FOR [UpdateUser]
GO
