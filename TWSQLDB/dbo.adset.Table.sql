USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[adset]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[adset](
	[Code] [varchar](30) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[AdsetDesc] [nvarchar](50) NOT NULL,
	[TxtMax] [int] NOT NULL,
	[TxtMin] [int] NOT NULL,
	[Width] [int] NOT NULL,
	[Height] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[CacheMins] [int] NOT NULL,
	[Tmpl] [nvarchar](4000) NOT NULL,
	[PreViewURL] [varchar](256) NOT NULL,
	[ImgMaxSize] [int] NOT NULL,
	[StxtMax] [int] NOT NULL,
	[SimgMaxSize] [int] NOT NULL,
	[ContMax] [int] NOT NULL,
 CONSTRAINT [PK_adset_code] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[adset] ADD  CONSTRAINT [DF_adset_adset_code]  DEFAULT ('') FOR [Code]
GO
ALTER TABLE [dbo].[adset] ADD  CONSTRAINT [DF_adset_adset_name]  DEFAULT ('') FOR [Name]
GO
ALTER TABLE [dbo].[adset] ADD  CONSTRAINT [DF_adset_adset_desc]  DEFAULT ('') FOR [AdsetDesc]
GO
ALTER TABLE [dbo].[adset] ADD  CONSTRAINT [DF_adset_adset_txtmax]  DEFAULT ((0)) FOR [TxtMax]
GO
ALTER TABLE [dbo].[adset] ADD  CONSTRAINT [DF_adset_adset_txtmin]  DEFAULT ((0)) FOR [TxtMin]
GO
ALTER TABLE [dbo].[adset] ADD  CONSTRAINT [DF_adset_adset_width]  DEFAULT ((0)) FOR [Width]
GO
ALTER TABLE [dbo].[adset] ADD  CONSTRAINT [DF_adset_adset_height]  DEFAULT ((0)) FOR [Height]
GO
ALTER TABLE [dbo].[adset] ADD  CONSTRAINT [DF_adset_adset_type]  DEFAULT ((0)) FOR [Type]
GO
ALTER TABLE [dbo].[adset] ADD  CONSTRAINT [DF_adset_adset_cachemins]  DEFAULT ((60)) FOR [CacheMins]
GO
ALTER TABLE [dbo].[adset] ADD  CONSTRAINT [DF_adset_adset_tmpl]  DEFAULT ('') FOR [Tmpl]
GO
ALTER TABLE [dbo].[adset] ADD  CONSTRAINT [DF_adset_adset_previewurl]  DEFAULT ('') FOR [PreViewURL]
GO
ALTER TABLE [dbo].[adset] ADD  CONSTRAINT [DF__adset__adset_img__25518C17]  DEFAULT ((0)) FOR [ImgMaxSize]
GO
ALTER TABLE [dbo].[adset] ADD  CONSTRAINT [DF_adset_adset_stxtmax]  DEFAULT ((0)) FOR [StxtMax]
GO
ALTER TABLE [dbo].[adset] ADD  CONSTRAINT [DF_adset_adset_simgmaxsize]  DEFAULT ((0)) FOR [SimgMaxSize]
GO
ALTER TABLE [dbo].[adset] ADD  CONSTRAINT [DF_adset_adset_contmax]  DEFAULT ((0)) FOR [ContMax]
GO
