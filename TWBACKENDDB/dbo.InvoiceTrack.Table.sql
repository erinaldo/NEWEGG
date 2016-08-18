USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[InvoiceTrack]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InvoiceTrack](
	[SN] [int] IDENTITY(1,1) NOT NULL,
	[Year] [int] NOT NULL,
	[BeginMonth] [int] NOT NULL,
	[EndMonth] [int] NOT NULL,
	[InvoiceTrackName] [char](2) NOT NULL,
	[BeginNumber] [int] NOT NULL,
	[EndNumber] [int] NOT NULL,
	[CurrentNumber] [int] NOT NULL CONSTRAINT [DF_InvoiceTrack_CurrentNumber]  DEFAULT ((0)),
 CONSTRAINT [PK_InvoiceTrack] PRIMARY KEY CLUSTERED 
(
	[SN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
