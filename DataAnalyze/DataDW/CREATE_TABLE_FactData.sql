USE [DataDW]
GO

/****** Object:  Table [dbo].[FactData]    Script Date: 13/04/2019 14:55:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FactData](
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
 CONSTRAINT [PK_FactData] PRIMARY KEY CLUSTERED 
(
	[FullName] ASC,
	[CollectionDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[FactData]  WITH CHECK ADD  CONSTRAINT [FK_FactData_DimDate] FOREIGN KEY([DateKey])
REFERENCES [dbo].[DimDate] ([DateKey])
GO

ALTER TABLE [dbo].[FactData] CHECK CONSTRAINT [FK_FactData_DimDate]
GO

ALTER TABLE [dbo].[FactData]  WITH CHECK ADD  CONSTRAINT [FK_FactData_DimGroup] FOREIGN KEY([FullName])
REFERENCES [dbo].[DimGroup] ([FullName])
GO

ALTER TABLE [dbo].[FactData] CHECK CONSTRAINT [FK_FactData_DimGroup]
GO


