USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[track]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[track](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ACCID] [int] NOT NULL,
	[ItemID] [int] NOT NULL,
	[Status] [int] NOT NULL CONSTRAINT [DF_tick_tick_status]  DEFAULT ((0)),
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_track_track_createdate]  DEFAULT (getdate()),
	[CategoryID] [int] NULL,
	[CategoryType] [int] NULL,
	[Qty] [int] NULL,
 CONSTRAINT [PK_track_id] PRIMARY KEY CLUSTERED 
(
	[ID] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
