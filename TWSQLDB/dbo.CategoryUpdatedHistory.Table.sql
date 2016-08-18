USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[CategoryUpdatedHistory]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CategoryUpdatedHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[UpdatedType] [int] NOT NULL,
	[BeforeUpdatedData] [nvarchar](2048) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_CategoryUpdatedHistory_CreateDate]  DEFAULT (getdate()),
	[CreateUser] [varchar](20) NOT NULL,
 CONSTRAINT [PK_CategoryUpdatedHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0:Unknown, 1:Add_Category, 2:Add_Manager, 7:UpdatedBasicData, 8:UpdatedManager' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'CategoryUpdatedHistory', @level2type=N'COLUMN',@level2name=N'UpdatedType'
GO
