USE [DataDW]
GO

/****** Object:  Table [dbo].[DimGroup]    Script Date: 13/04/2019 14:54:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DimGroup](
	[FullName] [nvarchar](255) NOT NULL,
	[level0] [nvarchar](max) NULL,
	[level1] [nvarchar](max) NULL,
	[level2] [nvarchar](max) NULL,
	[level3] [nvarchar](max) NULL,
	[level4] [nvarchar](max) NULL,
	[level5] [nvarchar](max) NULL,
	[level6] [nvarchar](max) NULL,
	[level7] [nvarchar](max) NULL,
	[level8] [nvarchar](max) NULL,
	[level9] [nvarchar](max) NULL,
	[Id] [uniqueidentifier] NULL,
	[ParentId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_DimGroup] PRIMARY KEY CLUSTERED 
(
	[FullName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


