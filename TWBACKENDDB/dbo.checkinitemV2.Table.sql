USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[checkinitemV2]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[checkinitemV2](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CheckinID] [int] NULL,
	[PoCode] [varchar](15) NULL,
	[PoitemCode] [varchar](15) NULL,
	[ProductID] [int] NULL,
	[SpNO] [varchar](200) NULL,
	[Attribs] [nvarchar](200) NULL,
	[Qty] [int] NULL,
	[QtyStckResv] [int] NULL,
	[QtypaID] [int] NULL,
	[Price] [decimal](10, 2) NULL,
	[TaxType] [int] NULL,
	[Parent] [int] NULL,
	[Frm] [nvarchar](200) NULL,
	[Frmid] [int] NULL,
	[AmntpaId] [int] NULL,
	[RetID] [int] NULL,
	[StckDate] [datetime] NULL,
	[StckUser] [nvarchar](50) NULL,
	[ScmretcFmDate] [datetime] NULL,
	[DcmRetcfmUser] [nvarchar](50) NULL,
	[ScmRetcfmNote] [nvarchar](200) NULL,
	[EraseDate] [datetime] NULL,
	[EraseCause] [int] NULL,
	[EraseCauseNote] [nvarchar](200) NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[Updated] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[Note] [nvarchar](max) NULL,
	[FileName] [nvarchar](50) NULL,
	[ShippingFee] [decimal](10, 2) NULL,
	[TaxandDuty] [decimal](10, 2) NULL,
	[CustomsCharges] [decimal](10, 2) NULL,
	[TradeServiceCharges] [decimal](10, 2) NULL,
 CONSTRAINT [PK_checkinitemV2_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
