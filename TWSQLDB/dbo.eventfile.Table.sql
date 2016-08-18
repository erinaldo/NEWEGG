USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[eventfile]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[eventfile](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[eventid] [int] NULL,
	[subfilename] [nvarchar](5) NOT NULL,
	[createdate] [datetime] NOT NULL CONSTRAINT [DF_eventfile_createdate]  DEFAULT (getdate()),
	[createuser] [nvarchar](50) NOT NULL,
	[updatedate] [datetime] NULL,
	[updateuser] [nvarchar](50) NULL,
 CONSTRAINT [PK_eventfile] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'序號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'eventfile', @level2type=N'COLUMN',@level2name=N'id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'對應的event id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'eventfile', @level2type=N'COLUMN',@level2name=N'eventid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'附檔名稱, ex: .txt' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'eventfile', @level2type=N'COLUMN',@level2name=N'subfilename'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建檔時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'eventfile', @level2type=N'COLUMN',@level2name=N'createdate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建檔人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'eventfile', @level2type=N'COLUMN',@level2name=N'createuser'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最後修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'eventfile', @level2type=N'COLUMN',@level2name=N'updatedate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最後修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'eventfile', @level2type=N'COLUMN',@level2name=N'updateuser'
GO
