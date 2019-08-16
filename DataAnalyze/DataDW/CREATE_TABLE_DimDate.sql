USE [DataDW]
GO

/****** Object:  Table [dbo].[DimDate]    Script Date: 13/04/2019 14:54:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DimDate](
	[DateKey] [int] NOT NULL,
	[FullDateAlternateKey] [date] NOT NULL,
	[DayNumberOfWeek] [smallint] NOT NULL,
	[EnglishDayNameOfWeek] [char](3) NOT NULL,
	[DayNumberOfMonth] [smallint] NOT NULL,
	[DayNumberOfYear] [smallint] NOT NULL,
	[WeekNumberOfYear] [smallint] NOT NULL,
	[EnglishMonthName] [char](3) NOT NULL,
	[MonthNumberOfYear] [smallint] NOT NULL,
	[CalendarQuarter] [smallint] NOT NULL,
	[CalendarYear] [smallint] NOT NULL,
	[CalendarSemester] [smallint] NOT NULL,
 CONSTRAINT [PK_DimDate] PRIMARY KEY CLUSTERED 
(
	[DateKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


