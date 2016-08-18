USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[sbcategory]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[sbcategory](
	[SN] [int] IDENTITY(1,1) NOT NULL,
	[NodeID] [int] NOT NULL,
	[StoreID] [int] NOT NULL,
	[CategoryID] [int] NOT NULL,
	[CategoryType] [int] NOT NULL,
	[Description] [varchar](150) NOT NULL,
	[Showall] [int] NOT NULL,
	[Updated] [int] NOT NULL,
	[ShowOrder] [int] NOT NULL,
	[CreateUser] [nvarchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateUser] [nvarchar](50) NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_subcategory_sn] PRIMARY KEY CLUSTERED 
(
	[SN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[sbcategory] ADD  CONSTRAINT [DF_sbcategory_sbcategory_updated]  DEFAULT ((0)) FOR [Updated]
GO
ALTER TABLE [dbo].[sbcategory] ADD  CONSTRAINT [DF_sbcategory_sbcategory_createdate]  DEFAULT (getdate()) FOR [CreateDate]
GO
