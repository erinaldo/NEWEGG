USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[ItemInStock_transV2]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ItemInStock_transV2](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CheckinID] [int] NULL,
	[StckID] [int] NULL,
	[CheckoutID] [int] NULL,
	[POCode] [varchar](15) NULL,
	[SOCode] [varchar](15) NULL,
	[PoItemCode] [varchar](15) NULL,
	[SoItemCode] [varchar](15) NULL,
	[ProductID] [int] NULL,
	[SpNO] [nvarchar](200) NULL,
	[Attribs] [nvarchar](200) NULL,
	[Qty] [int] NULL,
	[QtyStckresv] [int] NULL,
	[QtypaID] [int] NULL,
	[Price] [decimal](10, 2) NULL,
	[TaxType] [int] NULL,
	[Parent] [int] NULL,
	[FRM] [nvarchar](200) NULL,
	[FRMID] [int] NULL,
	[AmntpaID] [int] NULL,
	[RetID] [int] NULL,
	[StckDate] [datetime] NULL,
	[StckUser] [nvarchar](50) NULL,
	[ScmretcfmDate] [datetime] NULL,
	[Scmretcfmuser] [nvarchar](50) NULL,
	[ScmretcfmNote] [nvarchar](200) NULL,
	[EraseDate] [datetime] NULL,
	[EraseCause] [int] NULL,
	[EraseCauseNote] [nvarchar](200) NULL,
	[CreateDate] [datetime] NULL,
	[Updated] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[PreQty] [int] NULL,
	[PreAVGCost] [decimal](10, 2) NULL,
	[PreTotalCost] [decimal](10, 2) NULL,
	[AfterQty] [int] NULL,
	[AfterAVGCost] [decimal](10, 2) NULL,
	[AfterTotalCost] [decimal](10, 2) NULL,
	[Cost] [decimal](10, 2) NULL,
	[PreTotalShippingFee] [decimal](10, 2) NULL,
	[PreTotalTaxandDuty] [decimal](10, 2) NULL,
	[PreTotalCustomsCharges] [decimal](10, 2) NULL,
	[PreTotalServiceCharges] [decimal](10, 2) NULL,
	[AfterTotalShippingFee] [decimal](10, 2) NULL,
	[AfterTotalTaxandDuty] [decimal](10, 2) NULL,
	[AfterTotalCustomsCharges] [decimal](10, 2) NULL,
	[AfterTotalServiceCharges] [decimal](10, 2) NULL,
 CONSTRAINT [PK_ItemInStock_transV2_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
