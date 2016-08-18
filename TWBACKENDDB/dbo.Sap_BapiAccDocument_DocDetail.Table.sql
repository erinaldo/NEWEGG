USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[Sap_BapiAccDocument_DocDetail]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sap_BapiAccDocument_DocDetail](
	[SN] [int] IDENTITY(1,1) NOT NULL,
	[TransactionType] [nvarchar](6) NULL,
	[TransactionID] [nvarchar](18) NOT NULL,
	[ITEMNO_ACC] [int] NULL,
	[ACCT_TYPE] [nvarchar](1) NULL,
	[GL_ACCOUNT] [nvarchar](10) NULL,
	[CUSTOMER_ID] [nvarchar](10) NULL,
	[VENDOR_ID] [nvarchar](50) NULL,
	[ALLOC_NMBR] [nvarchar](18) NULL,
	[ITEM_TEXT] [nvarchar](50) NULL,
	[BUS_AREA] [nvarchar](4) NULL,
	[ORDERID] [nvarchar](12) NULL,
	[COSTCENTER] [nvarchar](10) NULL,
	[PROFIT_CTR] [nvarchar](10) NULL,
	[VALUE_DATE] [datetime] NULL,
	[PMNTTREMS] [nvarchar](4) NULL,
	[BLINE_DATE] [datetime] NULL,
	[PYMT_METH] [nvarchar](1) NULL,
	[PMNT_BLOCK] [nvarchar](1) NULL,
	[SP_GL_IND] [nvarchar](1) NULL,
	[CURR_TYPE] [nvarchar](2) NULL,
	[CURRENCY] [nvarchar](5) NULL,
	[AMT_DOCCUR] [decimal](18, 2) NULL,
	[EXCH_RATE] [decimal](9, 5) NULL,
	[COST_CTR] [nvarchar](10) NULL,
 CONSTRAINT [PK_Sap_BapiAccDocument_DocDetail_1] PRIMARY KEY CLUSTERED 
(
	[SN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
