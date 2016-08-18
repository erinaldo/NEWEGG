USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[CCCCode]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CCCCode](
	[貨品分類號列] [nvarchar](255) NULL,
	[中文貨名] [nvarchar](255) NULL,
	[英文貨名] [nvarchar](255) NULL,
	[第一欄稅率] [nvarchar](255) NULL,
	[第二欄稅率] [nvarchar](255) NULL,
	[第三欄稅率] [nvarchar](255) NULL,
	[統計數量單位] [nvarchar](255) NULL,
	[統計重量單位] [nvarchar](255) NULL,
	[稽徵規定] [nvarchar](255) NULL,
	[輸入規定] [nvarchar](255) NULL,
	[輸出規定] [nvarchar](255) NULL
) ON [PRIMARY]

GO
