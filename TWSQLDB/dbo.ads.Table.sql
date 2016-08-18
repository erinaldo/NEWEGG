USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ads]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ads](
	[Code] [varchar](30) NOT NULL,
	[Title] [nvarchar](200) NOT NULL,
	[Link] [varchar](100) NOT NULL,
	[ImgURL] [varchar](256) NOT NULL,
	[Html] [nvarchar](4000) NOT NULL,
	[Price] [int] NOT NULL,
	[Mode] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[TrackID] [varchar](30) NOT NULL,
	[SellerID] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[CssID] [int] NOT NULL,
	[ItemID] [int] NOT NULL,
	[CategoryID] [int] NOT NULL,
	[STitle] [nvarchar](150) NOT NULL,
	[SimgURL] [varchar](256) NOT NULL,
	[AdsContent] [nvarchar](4000) NOT NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NOT NULL,
	[Updated] [int] NOT NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedUser] [nvarchar](50) NULL,
 CONSTRAINT [PK_ads_id] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[ads] ADD  CONSTRAINT [DF_ads_ads_title]  DEFAULT ('') FOR [Title]
GO
ALTER TABLE [dbo].[ads] ADD  CONSTRAINT [DF_ads_ads_link]  DEFAULT ('') FOR [Link]
GO
ALTER TABLE [dbo].[ads] ADD  CONSTRAINT [DF_ads_ads_imgurl]  DEFAULT ('') FOR [ImgURL]
GO
ALTER TABLE [dbo].[ads] ADD  CONSTRAINT [DF_ads_ads_html]  DEFAULT ('') FOR [Html]
GO
ALTER TABLE [dbo].[ads] ADD  CONSTRAINT [DF_ads_ads_price]  DEFAULT ((0)) FOR [Price]
GO
ALTER TABLE [dbo].[ads] ADD  CONSTRAINT [DF_ads_ads_mode]  DEFAULT ((0)) FOR [Mode]
GO
ALTER TABLE [dbo].[ads] ADD  CONSTRAINT [DF_ads_ads_startda]  DEFAULT (getdate()) FOR [StartDate]
GO
ALTER TABLE [dbo].[ads] ADD  CONSTRAINT [DF_ads_ads_enddate]  DEFAULT ('2050/12/31') FOR [EndDate]
GO
ALTER TABLE [dbo].[ads] ADD  CONSTRAINT [DF_ads__ads_trackid]  DEFAULT ('') FOR [TrackID]
GO
ALTER TABLE [dbo].[ads] ADD  CONSTRAINT [DF_ads_ads_sellerid]  DEFAULT ((0)) FOR [SellerID]
GO
ALTER TABLE [dbo].[ads] ADD  CONSTRAINT [DF_ads_ads_status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[ads] ADD  CONSTRAINT [DF_ads_ads_cssid]  DEFAULT ((0)) FOR [CssID]
GO
ALTER TABLE [dbo].[ads] ADD  CONSTRAINT [DF_ads_ads_itemid]  DEFAULT ((0)) FOR [ItemID]
GO
ALTER TABLE [dbo].[ads] ADD  CONSTRAINT [DF_ads_ads_categor]  DEFAULT ((50)) FOR [CategoryID]
GO
ALTER TABLE [dbo].[ads] ADD  CONSTRAINT [DF_ads_ads_stitle]  DEFAULT ('') FOR [STitle]
GO
ALTER TABLE [dbo].[ads] ADD  CONSTRAINT [DF_ads_ads_simgurl]  DEFAULT ('') FOR [SimgURL]
GO
ALTER TABLE [dbo].[ads] ADD  CONSTRAINT [DF_ads_ads_content]  DEFAULT ('') FOR [AdsContent]
GO
ALTER TABLE [dbo].[ads] ADD  CONSTRAINT [DF_ads_ads_created]  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[ads] ADD  CONSTRAINT [DF_ads_ads_updated]  DEFAULT ((0)) FOR [Updated]
GO
