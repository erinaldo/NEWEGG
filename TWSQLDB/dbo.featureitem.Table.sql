USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[featureitem]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[featureitem](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Type] [int] NOT NULL CONSTRAINT [DF_featureitem_featureitem_type]  DEFAULT ((0)),
	[TypeName] [nvarchar](30) NOT NULL,
	[ItemID] [int] NOT NULL,
	[FeatureitemOrder] [int] NOT NULL,
	[DateStart] [datetime] NULL,
	[DateEnd] [datetime] NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_featureitem_featureitem_createdate]  DEFAULT (getdate()),
	[Updated] [int] NOT NULL CONSTRAINT [DF_featureitem_featureitem_updated]  DEFAULT ((0)),
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_featureitem_id] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
