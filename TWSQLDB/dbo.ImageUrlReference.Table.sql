USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ImageUrlReference]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImageUrlReference](
	[ItemID] [int] NOT NULL,
	[Size] [int] NOT NULL,
	[SizeIndex] [int] NOT NULL,
	[ImageUrl] [nvarchar](max) NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL CONSTRAINT [DF_ImageUrlReference_CreateUser]  DEFAULT (''),
	[Updated] [int] NOT NULL CONSTRAINT [DF_ImageUrlReference_Updated]  DEFAULT ((0)),
	[UpdateDate] [datetime] NULL CONSTRAINT [DF_ImageUrlReference_UpdateDate]  DEFAULT ('1900/01/01'),
	[UpdateUser] [nvarchar](50) NULL CONSTRAINT [DF_ImageUrlReference_UpdateUser]  DEFAULT (''),
 CONSTRAINT [PK_ImageUrlReference] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC,
	[Size] ASC,
	[SizeIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
