USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[salesordergroupTemp]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[salesordergroupTemp](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Vaccunt] [varchar](16) NOT NULL,
	[PriceSum] [int] NOT NULL,
	[OrderNum] [int] NOT NULL CONSTRAINT [DF_salesordergroupTemp_salesordergroupTemp_ordernum]  DEFAULT ((0)),
	[Note] [nvarchar](100) NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Updated] [int] NOT NULL,
	[UpdateUser] [nvarchar](50) NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_salesordergroupTemp_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
