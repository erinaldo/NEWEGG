USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[ShippingDetail]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [dbo].[ShippingDetail](
	[Index] [int] IDENTITY(1,1) NOT NULL,
	[ShipId] [int] NOT NULL,
	[Carton] [int] NOT NULL,
	[POCode] [varchar](15) NULL,
	[Pieces] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ShippingDetail] PRIMARY KEY CLUSTERED 
(
	[Index] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
