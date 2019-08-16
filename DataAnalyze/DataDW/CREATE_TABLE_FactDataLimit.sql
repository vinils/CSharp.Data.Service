USE [DataDW]
GO

/****** Object:  Table [dbo].[FactDataLimit]    Script Date: 13/04/2019 14:56:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FactDataLimit](
	[LimitId] [uniqueidentifier] NOT NULL,
	[FullName] [nvarchar](255) NOT NULL,
	[CollectionDate] [datetime] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Initials] [nvarchar](max) NULL,
	[DecimalValue] [decimal](18, 2) NULL,
	[MeasureUnit] [nvarchar](max) NULL,
	[StringValue] [nvarchar](max) NULL,
	[LimitName] [nvarchar](max) NULL,
	[LimitDescription] [nvarchar](max) NULL,
	[LimitColor] [int] NULL,
	[Discriminator] [nvarchar](128) NOT NULL,
	[DateKey] [int] NOT NULL,
 CONSTRAINT [PK_FactDataLimit] PRIMARY KEY CLUSTERED 
(
	[LimitId] ASC,
	[FullName] ASC,
	[CollectionDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[FactDataLimit]  WITH CHECK ADD  CONSTRAINT [FK_FactDataLimit_DimDate] FOREIGN KEY([DateKey])
REFERENCES [dbo].[DimDate] ([DateKey])
GO

ALTER TABLE [dbo].[FactDataLimit] CHECK CONSTRAINT [FK_FactDataLimit_DimDate]
GO

ALTER TABLE [dbo].[FactDataLimit]  WITH CHECK ADD  CONSTRAINT [FK_FactDataLimit_DimGroup] FOREIGN KEY([FullName])
REFERENCES [dbo].[DimGroup] ([FullName])
GO

ALTER TABLE [dbo].[FactDataLimit] CHECK CONSTRAINT [FK_FactDataLimit_DimGroup]
GO

ALTER TABLE [dbo].[FactDataLimit]  WITH CHECK ADD  CONSTRAINT [FK_FactDataLimit_DimLimit] FOREIGN KEY([LimitId])
REFERENCES [dbo].[DimLimit] ([Id])
GO

ALTER TABLE [dbo].[FactDataLimit] CHECK CONSTRAINT [FK_FactDataLimit_DimLimit]
GO


