USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[answr]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[answr](
	[Code] [varchar](50) NOT NULL,
	[PrblmCode] [varchar](50) NULL,
	[Cont] [nvarchar](1000) NULL,
	[Note] [nvarchar](1000) NULL,
	[Date] [datetime] NULL,
	[AnswrCode] [varchar](50) NULL,
	[ProcessUser] [nvarchar](50) NULL,
	[ToUser] [nvarchar](50) NULL,
	[Status] [int] NULL,
	[CreateDate] [datetime] NULL CONSTRAINT [DF_answr_answr_createdate]  DEFAULT (getdate()),
	[Updated] [int] NULL CONSTRAINT [DF_answr_answr_updated]  DEFAULT ((0)),
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](50) NULL,
 CONSTRAINT [PK_answr_code] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
