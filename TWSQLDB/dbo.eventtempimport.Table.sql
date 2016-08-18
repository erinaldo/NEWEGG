USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[eventtempimport]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[eventtempimport](
	[id] [int] NOT NULL,
	[event_id] [int] NOT NULL,
	[account_id] [int] NOT NULL,
	[account_email] [nvarchar](50) NULL,
	[usageflag] [int] NOT NULL,
 CONSTRAINT [PK_eventtempimport_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC,
	[event_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[eventtempimport] ADD  CONSTRAINT [DF_eventtempimport_usageflag]  DEFAULT ((0)) FOR [usageflag]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'對應的活動id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'eventtempimport', @level2type=N'COLUMN',@level2name=N'event_id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'end use id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'eventtempimport', @level2type=N'COLUMN',@level2name=N'account_id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'end user email' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'eventtempimport', @level2type=N'COLUMN',@level2name=N'account_email'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'使用狀態, default 0:未使用; 1:已使用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'eventtempimport', @level2type=N'COLUMN',@level2name=N'usageflag'
GO
