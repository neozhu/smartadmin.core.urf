USE [smartdb]
GO

/****** Object:  Table [dbo].[Sequence] 序列种子表   Script Date: 2020/7/30 8:57:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Sequence](
	[prefix] [nvarchar](10) NOT NULL,
	[seed] [int] NULL,
 CONSTRAINT [PK_Sequence] PRIMARY KEY CLUSTERED 
(
	[prefix] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Sequence] ADD  CONSTRAINT [DF_Sequence_seed]  DEFAULT ((0)) FOR [seed]
GO