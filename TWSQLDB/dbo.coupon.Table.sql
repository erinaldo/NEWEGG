USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[coupon]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[coupon](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[number] [nvarchar](13) NOT NULL,
	[eventid] [int] NOT NULL,
	[marketnumber] [nvarchar](20) NULL,
	[accountid] [nvarchar](50) NULL,
	[ordcode] [nvarchar](50) NULL,
	[title] [nvarchar](50) NULL,
	[categories] [nvarchar](max) NULL,
	[activetype] [int] NOT NULL CONSTRAINT [DF_coupon_validtype]  DEFAULT ((0)),
	[validstart] [datetime] NOT NULL,
	[validend] [datetime] NOT NULL,
	[validtype] [int] NOT NULL CONSTRAINT [DF_coupon_validtype_1]  DEFAULT ((0)),
	[visible] [int] NOT NULL CONSTRAINT [DF_coupon_visible]  DEFAULT ((1)),
	[note] [nvarchar](200) NULL,
	[usestatus] [int] NOT NULL CONSTRAINT [DF_coupon_usestatus]  DEFAULT ((0)),
	[value] [decimal](10, 2) NOT NULL CONSTRAINT [DF_coupon_value]  DEFAULT ((0)),
	[createdate] [datetime] NOT NULL CONSTRAINT [DF_coupon_createdate]  DEFAULT (getdate()),
	[createuser] [nvarchar](50) NOT NULL,
	[updated] [int] NOT NULL CONSTRAINT [DF_coupon_updated]  DEFAULT ((0)),
	[updatedate] [datetime] NULL,
	[updateuser] [nvarchar](50) NULL,
	[usedate] [datetime] NULL,
	[limit] [decimal](10, 2) NOT NULL CONSTRAINT [DF_coupon_limit]  DEFAULT ((0)),
	[SAIn] [int] NULL,
	[SAOut] [int] NULL,
	[items] [nvarchar](max) NULL,
 CONSTRAINT [PK_coupon] PRIMARY KEY CLUSTERED 
(
	[number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'序號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'coupon', @level2type=N'COLUMN',@level2name=N'id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'顯示給user的名稱,第1碼為英文,2013年為A, 2-6碼為eventid, 7-13碼為coupon編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'coupon', @level2type=N'COLUMN',@level2name=N'number'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'對應的event id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'coupon', @level2type=N'COLUMN',@level2name=N'eventid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'擁有者的account' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'coupon', @level2type=N'COLUMN',@level2name=N'accountid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'購買的訂購編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'coupon', @level2type=N'COLUMN',@level2name=N'ordcode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'項目' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'coupon', @level2type=N'COLUMN',@level2name=N'title'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'可使用類別, 此項必須與對應的event的相同, default 0:通用, 其他catetories以;分號隔開' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'coupon', @level2type=N'COLUMN',@level2name=N'categories'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'設定生效方式: 0:未生效, 1:系統啟動生效, 2:User兌點生效' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'coupon', @level2type=N'COLUMN',@level2name=N'activetype'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'生效日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'coupon', @level2type=N'COLUMN',@level2name=N'validstart'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'到期日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'coupon', @level2type=N'COLUMN',@level2name=N'validend'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'coupon實際生效的狀態, 0:等待系統啟動, 1:系統已啟動, 2由User兌碼啟動, 3:其他' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'coupon', @level2type=N'COLUMN',@level2name=N'validtype'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'顯示, default 0:不顯示, 1:顯示' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'coupon', @level2type=N'COLUMN',@level2name=N'visible'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'備註(此備註會給end user看)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'coupon', @level2type=N'COLUMN',@level2name=N'note'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'狀態, default 0:未設定, 1:生效可使用, 2:已使用, 3:未使用卻過期, 4:other(在note寫明原因), 5:暫時設定使用但未結帳' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'coupon', @level2type=N'COLUMN',@level2name=N'usestatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建檔時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'coupon', @level2type=N'COLUMN',@level2name=N'createdate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建樣人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'coupon', @level2type=N'COLUMN',@level2name=N'createuser'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改次數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'coupon', @level2type=N'COLUMN',@level2name=N'updated'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最後修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'coupon', @level2type=N'COLUMN',@level2name=N'updatedate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最後修改人' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'coupon', @level2type=N'COLUMN',@level2name=N'updateuser'
GO
