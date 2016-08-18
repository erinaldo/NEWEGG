USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[prhitm]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[prhitm](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PRHID] [int] NOT NULL,
	[ProcessID] [int] NOT NULL,
	[OrderitmCode] [varchar](15) NOT NULL,
	[CatsubID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
	[Attribs] [nvarchar](50) NOT NULL,
	[Parent] [int] NOT NULL,
	[SPNO] [varchar](50) NOT NULL,
	[Qty] [int] NOT NULL,
	[Cost] [decimal](10, 2) NOT NULL,
	[QtyProccncl] [int] NOT NULL,
	[QtyRetGood] [int] NOT NULL,
	[ScmEnabled] [int] NOT NULL,
	[ShipperID] [int] NOT NULL,
	[SPID] [int] NOT NULL,
	[SPName] [nvarchar](50) NOT NULL,
	[Status] [int] NOT NULL,
	[CnclDate] [datetime] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[Updated] [int] NOT NULL,
	[UpdateUser] [nvarchar](50) NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_prhitm_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_prhitm_prhitm_prhid]  DEFAULT ((0)) FOR [PRHID]
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_prhitm_prhitm_processid]  DEFAULT ((0)) FOR [ProcessID]
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_Table_1_prhitm_orderitmcode
prhitm_orderitmcode]  DEFAULT ('') FOR [OrderitmCode]
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_prhitm_prhitm_catsubid]  DEFAULT ((0)) FOR [CatsubID]
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_prhitm_prhitm_productid]  DEFAULT ((0)) FOR [ProductID]
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_prhitm_prhitm_title]  DEFAULT ('') FOR [Title]
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_prhitm_prhitm_attribs]  DEFAULT ('') FOR [Attribs]
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_prhitm_prhitm_parent]  DEFAULT ((0)) FOR [Parent]
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_prhitm_prhitm_spno]  DEFAULT ('') FOR [SPNO]
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_Table_1_prhitm_qtyprhitm_qty]  DEFAULT ((0)) FOR [Qty]
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_prhitm_prhitm_cost]  DEFAULT ((0)) FOR [Cost]
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_prhitm_prhitm_qtyproccncl]  DEFAULT ((0)) FOR [QtyProccncl]
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_prhitm_prhitm_qtyretgood]  DEFAULT ((0)) FOR [QtyRetGood]
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_prhitm_prhitm_scmenabled]  DEFAULT ((0)) FOR [ScmEnabled]
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_prhitm_prhitm_shipperid]  DEFAULT ((0)) FOR [ShipperID]
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_prhitm_prhitm_spid]  DEFAULT ((0)) FOR [SPID]
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_prhitm_prhitm_spname]  DEFAULT ('') FOR [SPName]
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_prhitm_prhitm_status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_prhitm_prhitm_cncldate]  DEFAULT ('1900/1/1') FOR [CnclDate]
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_prhitm_prhitm_createdate]  DEFAULT ('1900/1/1') FOR [CreateDate]
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_prhitm_prhitm_createuser]  DEFAULT ('') FOR [CreateUser]
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_prhitm_prhitm_updated]  DEFAULT ((0)) FOR [Updated]
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_prhitm_prhitm_updateusrer]  DEFAULT ('') FOR [UpdateUser]
GO
ALTER TABLE [dbo].[prhitm] ADD  CONSTRAINT [DF_prhitm_prhitm_updatedate]  DEFAULT ('1900/1/1') FOR [UpdateDate]
GO
