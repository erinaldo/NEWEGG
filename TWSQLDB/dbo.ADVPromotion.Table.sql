USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[ADVPromotion]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ADVPromotion](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[i_mcode] [varchar](32) NOT NULL,
	[i_gid] [varchar](32) NOT NULL,
	[i_amount] [varchar](32) NULL,
	[i_bid] [varchar](32) NOT NULL,
	[i_oid] [varchar](32) NOT NULL,
	[i_unique] [varchar](32) NULL,
	[i_unit] [char](3) NULL,
	[i_date] [varchar](32) NULL,
	[i_msub1] [varchar](32) NULL,
	[i_msub2] [varchar](32) NULL,
	[i_msub3] [varchar](32) NULL,
	[i_gno] [varchar](64) NOT NULL,
	[i_gname] [varchar](255) NOT NULL,
	[i_order_status] [varchar](32) NOT NULL,
	[i_order] [varchar](32) NULL,
	[a_source_id] [varchar](16) NULL,
 CONSTRAINT [PK_ADVPromotion] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
