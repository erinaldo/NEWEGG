USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[CartCouponTemp]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CartCouponTemp](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CartTempID] [int] NOT NULL,
	[ItemID] [int] NOT NULL,
	[CouponNumber] [nvarchar](13) NOT NULL,
	[CreateDate] [datetime] NULL,
	[CreateUser] [nvarchar](50) NULL,
	[Updated] [int] NULL CONSTRAINT [DF_CartCouponTemp_Updated]  DEFAULT ((0)),
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
 CONSTRAINT [PK_CartCouponTemp] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改次數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CartCouponTemp', @level2type=N'COLUMN',@level2name=N'Updated'
GO
