USE [DataContext]
GO

/****** Object:  Table [dbo].[LimitDecimal]    Script Date: 8/11/2019 3:26:04 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LimitDecimal](
	[GroupId] [uniqueidentifier] NOT NULL,
	[Priority] [int] NOT NULL,
	[Max] [decimal](18, 2) NOT NULL,
	[Min] [decimal](18, 2) NULL,
	[Name] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.LimitDecimal] PRIMARY KEY CLUSTERED 
(
	[GroupId] ASC,
	[Priority] ASC,
	[Max] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
