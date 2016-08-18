USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[prh]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[prh](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CartID] [int] NOT NULL,
	[OrderCode] [varchar](50) NOT NULL,
	[PrhUser] [nvarchar](50) NOT NULL,
	[USRIDNO] [varchar](50) NOT NULL,
	[USRMobile] [varchar](30) NOT NULL,
	[RECV] [nvarchar](50) NOT NULL,
	[RECVPhone] [varchar](30) NOT NULL,
	[RECVPhone2] [varchar](30) NOT NULL,
	[RECVMobile] [varchar](30) NOT NULL,
	[RECVEmail] [varchar](256) NOT NULL,
	[RECVLOC] [nvarchar](10) NOT NULL,
	[RECVZip] [char](5) NOT NULL,
	[RECVADDR] [nvarchar](150) NOT NULL,
	[RECVNote] [nvarchar](200) NOT NULL,
	[Note] [nvarchar](200) NOT NULL,
	[DELVType] [int] NOT NULL,
	[DELVDateEXP] [datetime] NOT NULL,
	[DELVDateLast] [datetime] NOT NULL,
	[CreditReply] [datetime] NOT NULL,
	[OutUser] [nvarchar](50) NOT NULL,
	[OutDate] [datetime] NOT NULL,
	[DELVShipperID] [int] NOT NULL,
	[DELVSHPNO] [varchar](50) NOT NULL,
	[SaleUpdate] [int] NOT NULL,
	[ServerName] [varchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[Updated] [int] NOT NULL,
	[UpdateUser] [nvarchar](50) NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_prh_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_cartid]  DEFAULT ((0)) FOR [CartID]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_ordercode]  DEFAULT ('') FOR [OrderCode]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_user]  DEFAULT ('') FOR [PrhUser]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_usridno]  DEFAULT ('') FOR [USRIDNO]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_usrmobile]  DEFAULT ('') FOR [USRMobile]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_recv]  DEFAULT ('') FOR [RECV]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_Table_1_prh_recvphoneprh_recvphoneprh_recvphone]  DEFAULT ('') FOR [RECVPhone]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_recvmobile]  DEFAULT ('') FOR [RECVMobile]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_recvemail]  DEFAULT ('') FOR [RECVEmail]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_recvaddr]  DEFAULT ('') FOR [RECVADDR]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_recvnote]  DEFAULT ('') FOR [RECVNote]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_note]  DEFAULT ('') FOR [Note]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_delvtype]  DEFAULT ((0)) FOR [DELVType]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_delvdateexp]  DEFAULT ('1900/1/1') FOR [DELVDateEXP]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_delvdatelast]  DEFAULT ('1900/1/1') FOR [DELVDateLast]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_date]  DEFAULT ('1900/1/1') FOR [CreditReply]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_outuser]  DEFAULT ('') FOR [OutUser]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_outdate]  DEFAULT ('1900/1/1') FOR [OutDate]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_delvshipperid]  DEFAULT ((0)) FOR [DELVShipperID]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_delvshpno]  DEFAULT ('') FOR [DELVSHPNO]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_saleupdate]  DEFAULT ((0)) FOR [SaleUpdate]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_servername]  DEFAULT ('') FOR [ServerName]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_createdate]  DEFAULT ('1900/1/1') FOR [CreateDate]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_createuser]  DEFAULT ('') FOR [CreateUser]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_updated]  DEFAULT ((0)) FOR [Updated]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_updateusrer]  DEFAULT ('') FOR [UpdateUser]
GO
ALTER TABLE [dbo].[prh] ADD  CONSTRAINT [DF_prh_prh_updatedateprh_updatedateprh_updatedateprh_updatedateprh_updatedate]  DEFAULT ('1900/1/1') FOR [UpdateDate]
GO
