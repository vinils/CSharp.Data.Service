USE [DataContext]
GO

/****** Object:  Table [dbo].[LimitStringDenormalized]    Script Date: 8/11/2019 3:27:40 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LimitStringDenormalized](
	[GroupId] [uniqueidentifier] NOT NULL,
	[CollectionDate] [datetime] NOT NULL,
	[Expected] [nvarchar](max) NULL,
	[Color] [int] NULL,
 CONSTRAINT [PK_dbo.LimitStringDenormalized] PRIMARY KEY CLUSTERED 
(
	[GroupId] ASC,
	[CollectionDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
