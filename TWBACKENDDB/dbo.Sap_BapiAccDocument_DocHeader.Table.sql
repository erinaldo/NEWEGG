USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[Sap_BapiAccDocument_DocHeader]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sap_BapiAccDocument_DocHeader](
	[SN] [int] IDENTITY(1,1) NOT NULL,
	[TransactionType] [nvarchar](6) NULL,
	[TransactionID] [nvarchar](18) NOT NULL,
	[BUS_ACT] [nvarchar](4) NULL,
	[USERNAME] [nvarchar](12) NULL,
	[HEADER_TXT] [nvarchar](25) NULL,
	[COMP_CODE] [nvarchar](4) NULL,
	[DOC_DATE] [datetime] NULL,
	[PSTNG_DATE] [datetime] NULL,
	[TRANS_DATE] [datetime] NULL,
	[FISC_YEAR] [nvarchar](4) NULL,
	[FIS_PERIOD] [nvarchar](2) NULL,
	[DOC_TYPE] [nvarchar](6) NULL,
	[REF_DOC_NO] [nvarchar](16) NULL,
	[AC_DOC_NO] [nvarchar](18) NULL,
	[C_STATUS] [nvarchar](10) NULL,
	[P_STATUS] [nvarchar](10) NULL,
	[P_TIME] [datetime] NULL,
	[TransactionNumber] [int] NULL,
	[DOC_NUMBER] [nvarchar](18) NULL,
	[Receivalbe_ItemNOACC] [int] NULL,
	[Receivable_Customer] [nvarchar](10) NULL,
	[Receivable_BusArea] [nvarchar](4) NULL,
	[Receivable_Pmnttrms] [nvarchar](4) NULL,
	[Receivable_AllocNmbr] [nvarchar](18) NULL,
	[Receivable_ItemText] [nvarchar](50) NULL,
	[Receivable_SPGLIND] [nvarchar](1) NULL,
	[Payable_ItemNOACC] [int] NULL,
	[Payable_VendorNO] [nvarchar](50) NULL,
	[Payable_BusArea] [nvarchar](4) NULL,
	[Payable_Pmnttrms] [nvarchar](4) NULL,
	[Payable_AllocNmbr] [nvarchar](18) NULL,
	[Payable_ItemText] [nvarchar](50) NULL,
	[Payable_SPGLIND] [nvarchar](1) NULL,
 CONSTRAINT [PK_Sap_BapiAccDocument_DocHeader] PRIMARY KEY CLUSTERED 
(
	[SN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
