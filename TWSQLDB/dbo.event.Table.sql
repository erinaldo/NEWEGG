USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[event]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[event](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[creator] [nvarchar](50) NOT NULL,
	[eventdescription] [nvarchar](500) NOT NULL,
	[couponmarketnumber] [nvarchar](20) NULL,
	[couponcategories] [nvarchar](max) NULL,
	[couponmax] [int] NOT NULL CONSTRAINT [DF_event_couponmax]  DEFAULT ((0)),
	[couponsum] [int] NOT NULL CONSTRAINT [DF_event_couponsum]  DEFAULT ((0)),
	[couponreget] [int] NOT NULL CONSTRAINT [DF_event_couponreget]  DEFAULT ((1)),
	[datestart] [datetime] NOT NULL,
	[dateend] [datetime] NOT NULL,
	[couponactiveusagedays] [int] NOT NULL CONSTRAINT [DF_event_couponactiveusagedays]  DEFAULT ((0)),
	[visible] [int] NOT NULL CONSTRAINT [DF_event_visible]  DEFAULT ((0)),
	[note] [nvarchar](200) NULL,
	[coupondescription] [nvarchar](200) NULL,
	[grantstart] [datetime] NULL,
	[grantend] [datetime] NULL,
	[grantstatus] [int] NOT NULL CONSTRAINT [DF_event_grantstatus]  DEFAULT ((0)),
	[couponvalidstart] [datetime] NOT NULL,
	[couponvalidend] [datetime] NOT NULL,
	[couponactivetype] [int] NOT NULL CONSTRAINT [DF_event_couponactivetype]  DEFAULT ((1)),
	[value] [decimal](10, 2) NOT NULL CONSTRAINT [DF_event_value]  DEFAULT ((0)),
	[limit] [decimal](10, 2) NULL CONSTRAINT [DF_event_limit]  DEFAULT ((0.00)),
	[limitdescription] [nvarchar](50) NOT NULL,
	[filter] [nvarchar](500) NULL,
	[filterfileusage] [int] NOT NULL,
	[filterfileid] [int] NULL,
	[createdate] [datetime] NOT NULL CONSTRAINT [DF_event_createdate]  DEFAULT (getdate()),
	[createuser] [nvarchar](50) NULL,
	[updated] [int] NOT NULL CONSTRAINT [DF_event_updated]  DEFAULT ((0)),
	[updatedate] [datetime] NULL,
	[updateuser] [nchar](10) NULL,
	[items] [nvarchar](max) NULL,
 CONSTRAINT [PK_event] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'序號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'event', @level2type=N'COLUMN',@level2name=N'id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'活動名稱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'event', @level2type=N'COLUMN',@level2name=N'name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'活動發起人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'event', @level2type=N'COLUMN',@level2name=N'creator'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'coupon券可使用的類別, defult 0:為通用,其他類別以;分號隔開, 如: ;1;2;3;' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'event', @level2type=N'COLUMN',@level2name=N'couponcategories'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'累積發放coupon最大數量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'event', @level2type=N'COLUMN',@level2name=N'couponmax'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'單一活動中, 單一User最多可領取的張數, 預設為1, 即最多領一張, 0為不限' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'event', @level2type=N'COLUMN',@level2name=N'couponreget'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'活動開始日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'event', @level2type=N'COLUMN',@level2name=N'datestart'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'活動結束日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'event', @level2type=N'COLUMN',@level2name=N'dateend'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'可使用的有效區間:自active算起的有效區間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'event', @level2type=N'COLUMN',@level2name=N'couponactiveusagedays'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'此活動是否顯示, default 0:不顯示, 1:顯示' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'event', @level2type=N'COLUMN',@level2name=N'visible'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'備註' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'event', @level2type=N'COLUMN',@level2name=N'note'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'發放coupon開始時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'event', @level2type=N'COLUMN',@level2name=N'grantstart'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'發放coupon結束時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'event', @level2type=N'COLUMN',@level2name=N'grantend'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'發放coupon狀態, default 0:未設定發送; 1:設定完成傳至系統發發; 2:coupon發放完成, 3: 手動關閉發放coupon, 4: other(note需寫原因); 5: 系統發放coupon中' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'event', @level2type=N'COLUMN',@level2name=N'grantstatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Coupon啟動方式, 0:不啟動, 1:系統自動判定, 2:User兌碼啟動' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'event', @level2type=N'COLUMN',@level2name=N'couponactivetype'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'使用金額的下限' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'event', @level2type=N'COLUMN',@level2name=N'limit'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建檔時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'event', @level2type=N'COLUMN',@level2name=N'createdate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建檔人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'event', @level2type=N'COLUMN',@level2name=N'createuser'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最後修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'event', @level2type=N'COLUMN',@level2name=N'updatedate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最後修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'event', @level2type=N'COLUMN',@level2name=N'updateuser'
GO
