USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[purchaseorderitemext]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[purchaseorderitemext](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseOrderItemCode] [varchar](15) NOT NULL,
	[PsproductID] [varchar](30) NOT NULL,
	[PsmProductID] [varchar](30) NULL,
	[PsoriPrice] [decimal](10, 2) NULL,
	[PssellCatID] [varchar](50) NULL,
	[PsAttribName] [nvarchar](50) NULL,
	[PsModelNO] [varchar](50) NULL,
	[PsCost] [int] NULL,
	[Psfvf] [int] NULL,
 CONSTRAINT [PK_purchaseorderitemext_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
