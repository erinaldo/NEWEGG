USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[HiLifeOrderInfo]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HiLifeOrderInfo](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[StoreNumber] [nvarchar](20) NOT NULL,
	[StoreName] [nvarchar](20) NOT NULL,
	[ODNumber] [nvarchar](20) NOT NULL,
	[DirectRouteNO] [nvarchar](20) NOT NULL,
	[ProductNumber] [nvarchar](20) NOT NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[Updated] [int] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_HiLifeOrderInfo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Stno(取貨門市編號)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HiLifeOrderInfo', @level2type=N'COLUMN',@level2name=N'StoreNumber'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Stnm(取貨店名)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HiLifeOrderInfo', @level2type=N'COLUMN',@level2name=N'StoreName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ODNO(EC訂單編號)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HiLifeOrderInfo', @level2type=N'COLUMN',@level2name=N'ODNumber'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'DCRONO(路線路順)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HiLifeOrderInfo', @level2type=N'COLUMN',@level2name=N'DirectRouteNO'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Prodnm(商品類型)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'HiLifeOrderInfo', @level2type=N'COLUMN',@level2name=N'ProductNumber'
GO
