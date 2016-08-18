USE [TWBACKENDDB]
GO
/****** Object:  Table [dbo].[PackageInfo]    Script Date: 2016/08/18 14:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PackageInfo](
	[Index] [int] IDENTITY(1,1) NOT NULL,
	[ShipId] [int] NOT NULL,
	[Carton] [int] NOT NULL,
	[Length] [decimal](6, 3) NULL,
	[Width] [decimal](6, 3) NULL,
	[Height] [decimal](6, 3) NULL,
	[GrossWeight] [decimal](6, 3) NULL,
 CONSTRAINT [PK_dbo.PackageInfo] PRIMARY KEY CLUSTERED 
(
	[Index] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
