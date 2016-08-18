USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[cHI_DCList]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cHI_DCList](
	[GoodStoreID] [varchar](4) NOT NULL,
	[GoodStoreName] [nvarchar](50) NULL,
	[InUserID] [int] NULL,
	[InDate] [datetime] NULL,
	[UpdateUserID] [int] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_cHI_DCList] PRIMARY KEY CLUSTERED 
(
	[GoodStoreID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[cHI_DCList] ADD  CONSTRAINT [DF_cHI_DCList_Indate]  DEFAULT (getdate()) FOR [InDate]
GO
ALTER TABLE [dbo].[cHI_DCList] ADD  CONSTRAINT [DF_cHI_DCList_Updatedate]  DEFAULT (getdate()) FOR [UpdateDate]
GO
