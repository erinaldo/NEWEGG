USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[scmdelvfail]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[scmdelvfail](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SupplierID] [int] NOT NULL,
	[ProcessID] [int] NOT NULL,
	[PurhitemID] [int] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[Cause] [int] NOT NULL,
	[CauseNote] [nvarchar](200) NOT NULL,
	[Status] [int] NOT NULL,
	[StatusNote] [nvarchar](50) NOT NULL,
	[StatusDate] [datetime] NOT NULL,
	[PHR2csNote] [nvarchar](50) NOT NULL,
	[PHR2csDate] [datetime] NOT NULL,
	[PHRUser] [nvarchar](50) NOT NULL,
	[PRBLMCode] [varchar](15) NOT NULL,
	[OldReceiver] [nvarchar](50) NOT NULL,
	[OldLOC] [nvarchar](10) NOT NULL,
	[OldZip] [char](5) NOT NULL,
	[OldADDR] [nvarchar](150) NOT NULL,
	[OldPhone] [varchar](30) NOT NULL,
	[OldMobile] [varchar](30) NOT NULL,
	[OldCartNote] [nvarchar](200) NOT NULL,
	[NewReceiver] [nvarchar](50) NOT NULL,
	[NewLOC] [nvarchar](10) NOT NULL,
	[NewZip] [char](5) NOT NULL,
	[NewADDR] [nvarchar](150) NOT NULL,
	[NewPhone] [varchar](30) NOT NULL,
	[NewMobile] [varchar](30) NOT NULL,
	[NewCartNote] [nvarchar](200) NOT NULL,
	[SupplierNote] [nvarchar](200) NOT NULL,
	[Note] [nvarchar](200) NOT NULL,
	[SCMDELVFailUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Updated] [int] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[UpdateUser] [nvarchar](50) NOT NULL,
	[SprspDate] [datetime] NOT NULL,
 CONSTRAINT [PK_scmdelvfail_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_supplierid]  DEFAULT ((0)) FOR [SupplierID]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_processid]  DEFAULT ((0)) FOR [ProcessID]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_purhitemid]  DEFAULT ((0)) FOR [PurhitemID]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_createdby]  DEFAULT ((0)) FOR [CreatedBy]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_cause]  DEFAULT ((0)) FOR [Cause]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_causenote]  DEFAULT ('') FOR [CauseNote]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_statusnote]  DEFAULT ('') FOR [StatusNote]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_statusdate]  DEFAULT ('1900/1/1') FOR [StatusDate]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_phr2csnote]  DEFAULT ('') FOR [PHR2csNote]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_phr2csdate]  DEFAULT ('1900/1/1') FOR [PHR2csDate]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_phruser]  DEFAULT ('') FOR [PHRUser]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_prblmcode]  DEFAULT ('') FOR [PRBLMCode]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_oldreceiver]  DEFAULT ('') FOR [OldReceiver]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_oldloc]  DEFAULT ((0)) FOR [OldLOC]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_oldzip]  DEFAULT ('') FOR [OldZip]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_oldaddr]  DEFAULT ('') FOR [OldADDR]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_oldphone]  DEFAULT ('') FOR [OldPhone]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_oldmobile]  DEFAULT ('') FOR [OldMobile]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_oldcartnote]  DEFAULT ('') FOR [OldCartNote]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_newreceiver]  DEFAULT ('') FOR [NewReceiver]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_newloc]  DEFAULT ((0)) FOR [NewLOC]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_newzip]  DEFAULT ('') FOR [NewZip]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_newaddr]  DEFAULT ('') FOR [NewADDR]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_newphone]  DEFAULT ('') FOR [NewPhone]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_newmobile]  DEFAULT ('') FOR [NewMobile]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_newcartnote]  DEFAULT ('') FOR [NewCartNote]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_suppliernote]  DEFAULT ('') FOR [SupplierNote]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_note]  DEFAULT ('') FOR [Note]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_user]  DEFAULT ('') FOR [SCMDELVFailUser]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_createdate]  DEFAULT ('1900/1/1') FOR [CreateDate]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_updated]  DEFAULT ((0)) FOR [Updated]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_updateddate]  DEFAULT ('1900/1/1') FOR [UpdateDate]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_updateuser]  DEFAULT ('') FOR [UpdateUser]
GO
ALTER TABLE [dbo].[scmdelvfail] ADD  CONSTRAINT [DF_scmdelvfail_scmdelvfail_sprspdate]  DEFAULT ('1900/1/1') FOR [SprspDate]
GO
