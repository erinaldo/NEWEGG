USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[CouponActivity]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CouponActivity](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ActivityStart] [datetime] NOT NULL,
	[ActivityEnd] [datetime] NOT NULL,
	[EventID] [int] NOT NULL,
	[Qty] [int] NOT NULL,
 CONSTRAINT [PK_CouponActivity] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'活動開始日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CouponActivity', @level2type=N'COLUMN',@level2name=N'ActivityStart'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'活動結束日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CouponActivity', @level2type=N'COLUMN',@level2name=N'ActivityEnd'
GO
