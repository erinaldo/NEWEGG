USE [TWSQLDB]
GO
/****** Object:  Table [dbo].[HiLifeF04Tail]    Script Date: 2016/08/18 14:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HiLifeF04Tail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HeadId] [int] NOT NULL,
	[Rdfmt] [char](1) NOT NULL,
	[Rdcnt] [varchar](8) NOT NULL,
	[Fil2] [nvarchar](21) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_F04Tail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[HiLifeF04Tail] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
