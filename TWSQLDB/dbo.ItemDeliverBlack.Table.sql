USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ItemDeliverBlack]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemDeliverBlack](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ItemID] [int] NOT NULL,
	[DeliverType] [int] NOT NULL,
	[DeliverCode] [int] NOT NULL,
	[PayTypeID] [int] NOT NULL,
	[IsEnable] [int] NOT NULL CONSTRAINT [DF_ItemDeliverBlack_IsEnable]  DEFAULT ((0)),
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[Updated] [int] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_ItemDeliverBlack] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'賣場編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ItemDeliverBlack', @level2type=N'COLUMN',@level2name=N'ItemID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'配送方式' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ItemDeliverBlack', @level2type=N'COLUMN',@level2name=N'DeliverType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'配送方式明細' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ItemDeliverBlack', @level2type=N'COLUMN',@level2name=N'DeliverCode'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'啟用:0 ; 關閉:1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ItemDeliverBlack', @level2type=N'COLUMN',@level2name=N'IsEnable'
GO
