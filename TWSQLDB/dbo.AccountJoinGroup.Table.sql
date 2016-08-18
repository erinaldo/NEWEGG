USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[AccountJoinGroup]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AccountJoinGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Introduction_time] [int] NOT NULL,
	[New_Account] [nvarchar](50) NOT NULL,
	[Old_Account] [nvarchar](50) NOT NULL,
	[SO_Number] [varchar](15) NULL,
	[RegisterSuccess] [bit] NULL,
	[CreateDate] [datetime] NULL,
	[UpdateDate] [datetime] NULL,
	[OldAccountGetCouponDate] [datetime] NULL,
	[NewAccountGetCouponDate] [datetime] NULL,
	[Status] [int] NULL,
	[ReceivedMail] [bit] NULL,
 CONSTRAINT [PK_MouseAccount] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
