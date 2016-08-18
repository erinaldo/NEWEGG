USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[salesordergroup]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[salesordergroup](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Vaccunt] [varchar](16) NOT NULL CONSTRAINT [DF_ordergroup_ordergroup_vaccunt]  DEFAULT (''),
	[PriceSum] [int] NOT NULL CONSTRAINT [DF_ordergroup_ordergroup_pricesum]  DEFAULT ((0)),
	[OrderNum] [int] NOT NULL CONSTRAINT [DF_salesordergroup_salesordergroup_ordernum]  DEFAULT ((0)),
	[Note] [nvarchar](100) NOT NULL CONSTRAINT [DF_ordergroup_ordergroup_note]  DEFAULT (''),
	[CreateUser] [nvarchar](50) NOT NULL CONSTRAINT [DF_ordergroup_ordergroup_sysuser]  DEFAULT (''),
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_ordergroup_ordergroup_sysdate]  DEFAULT (getdate()),
	[Updated] [int] NOT NULL CONSTRAINT [DF_ordergroup_ordergroup_updated]  DEFAULT ((0)),
	[UpdateUser] [nvarchar](50) NOT NULL CONSTRAINT [DF_ordergroup_ordergroup_updateduser]  DEFAULT (''),
	[UpdateDate] [datetime] NOT NULL CONSTRAINT [DF_ordergroup_ordergroup_updateddate]  DEFAULT ('1900/01/01'),
 CONSTRAINT [PK_salesordergroup_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
