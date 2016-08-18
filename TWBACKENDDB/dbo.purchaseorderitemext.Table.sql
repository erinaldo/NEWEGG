USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[purchaseorderitemext]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[purchaseorderitemext](
	[ID] [int] NOT NULL,
	[PurchaseOrderItemCode] [varchar](15) NOT NULL,
	[PSProductID] [varchar](30) NOT NULL,
	[PSMProductid] [varchar](30) NULL,
	[PSORIPrice] [decimal](10, 2) NULL,
	[PSSellCatID] [varchar](50) NULL,
	[PSAttribName] [nvarchar](50) NULL,
	[PSModelNO] [varchar](50) NULL,
	[PSCost] [int] NULL,
	[PSFvf] [int] NULL,
 CONSTRAINT [PK_purchaseorderitemext_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
