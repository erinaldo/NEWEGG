USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[checkoutV2]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[checkoutV2](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](15) NULL,
	[SoCode] [varchar](15) NULL,
	[SoitemCode] [varchar](15) NULL,
	[CheckOutUser] [nvarchar](50) NULL,
	[FinalDate] [datetime] NULL,
	[Status] [int] NULL,
	[WhID] [int] NULL,
	[WareHouse] [int] NULL,
	[Type] [int] NULL,
	[Cause] [int] NULL,
	[VouhCode] [varchar](50) NULL,
	[EndUser] [nvarchar](50) NULL,
	[Note] [nvarchar](200) NULL,
	[CreateUser] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[Updated] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[DelivType] [int] NULL,
	[StckStatus] [int] NULL,
	[RefSONumber] [int] NOT NULL CONSTRAINT [DF_checkoutV2_checkoutV2_RefSONumber]  DEFAULT ((0)),
	[StckstatusDate] [datetime] NULL,
	[StckstatusUser] [nvarchar](50) NULL,
	[SellerID] [int] NULL,
 CONSTRAINT [PK_checkoutV2_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
