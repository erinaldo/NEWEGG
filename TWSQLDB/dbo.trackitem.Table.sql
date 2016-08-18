USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[trackitem]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[trackitem](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TrackID] [int] NOT NULL,
	[ItemlistID] [int] NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_trackitem_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[trackitem] ADD  CONSTRAINT [DF_trackitem_trackitem_status]  DEFAULT ((0)) FOR [Status]
GO
