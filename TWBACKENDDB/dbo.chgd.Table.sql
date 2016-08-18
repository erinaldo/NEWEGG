USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[chgd]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[chgd](
	[ID] [int] NOT NULL,
	[ProcessID] [int] NULL,
	[SalesordritemCode] [varchar](15) NULL,
	[CreatedFRM] [int] NULL,
	[SupplierID] [int] NULL,
	[ChgdType] [int] NULL,
	[Scmenabled] [int] NULL,
	[PorTage] [int] NULL,
	[Cause] [int] NULL,
	[CauseNote] [nvarchar](200) NULL,
	[Qty] [int] NULL,
	[Atts] [nvarchar](200) NULL,
	[FrmName] [nvarchar](50) NULL,
	[FrmLocation] [nvarchar](10) NULL,
	[FrmZipcode] [char](5) NULL,
	[FrmADDR] [nvarchar](150) NULL,
	[FrmPhone] [varchar](30) NULL,
	[FrmPhone2] [varchar](30) NULL,
	[FrmMobile] [varchar](30) NULL,
	[FrmEmail] [varchar](256) NULL,
	[FrmTime] [int] NULL,
	[FrmNote] [nvarchar](200) NULL,
	[ToCompany] [nvarchar](50) NULL,
	[ToName] [nvarchar](50) NULL,
	[ToLocation] [nvarchar](10) NULL,
	[ToZipcode] [char](5) NULL,
	[ToADDR] [nvarchar](150) NULL,
	[ToPhone] [varchar](30) NULL,
	[Oldin] [datetime] NULL,
	[ChgdConfirm] [datetime] NULL,
	[ConfirmNote] [nvarchar](200) NULL,
	[OldOut] [datetime] NULL,
	[NewIn] [datetime] NULL,
	[NewOut] [datetime] NULL,
	[NewoutShipperID] [int] NULL,
	[NewoutshpCode] [nvarchar](50) NULL,
	[Status] [int] NULL,
	[StatusNote] [nvarchar](200) NULL,
	[StatusUser] [nvarchar](50) NULL,
	[StatusDate] [datetime] NULL,
	[Note] [nvarchar](200) NULL,
	[CreateDate] [datetime] NULL,
	[CreateUser] [nvarchar](50) NULL,
	[Updated] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
 CONSTRAINT [PK_chgd_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
