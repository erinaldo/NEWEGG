USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[DeviceAdSet]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeviceAdSet](
	[ID] [int] IDENTITY(1000,1) NOT NULL,
	[Name] [nvarchar](50) NULL DEFAULT (''),
	[SubName] [nvarchar](50) NULL DEFAULT (''),
	[Parent] [int] NULL,
	[Flag] [nvarchar](50) NOT NULL DEFAULT (''),
	[Showorder] [int] NOT NULL,
	[ShowAll] [nvarchar](50) NOT NULL DEFAULT (''),
	[CreateUser] [nvarchar](50) NOT NULL DEFAULT (''),
	[CreateDate] [datetime] NOT NULL DEFAULT (getdate()),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_DeviceAdSet] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'子層時是:標題文字' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DeviceAdSet', @level2type=N'COLUMN',@level2name=N'SubName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'值是NULL=沒有父親, 有值時父親是ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DeviceAdSet', @level2type=N'COLUMN',@level2name=N'Parent'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'phone=手機, pad=平版, pc=桌機' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DeviceAdSet', @level2type=N'COLUMN',@level2name=N'Flag'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'顯示:show|1, 不顯示:hide|0' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DeviceAdSet', @level2type=N'COLUMN',@level2name=N'ShowAll'
GO
