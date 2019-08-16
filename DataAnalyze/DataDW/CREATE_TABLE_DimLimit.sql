USE [DataDW]
GO

/****** Object:  Table [dbo].[DimLimit]    Script Date: 13/04/2019 14:55:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DimLimit](
	[Id] [uniqueidentifier] NOT NULL,
	[FullName] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Min] [decimal](18, 2) NULL,
	[Max] [decimal](18, 2) NULL,
	[Priority] [int] NOT NULL,
	[GroupId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_DimLimit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


