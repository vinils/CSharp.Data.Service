USE [DataDW]
-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_Load]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	---------------- FactDataLimit ---------------------------

	insert into FactData
	select 
	SUBSTRING(FullName, 0, 400) as FullName, 
	e.CollectionDate, 
	gD.Name, 
	gD.Initials, 
	e.DecimalValue,
	gD.MeasureUnit,
	e.StringValue, 
	case Discriminator 
		when 'DataDecimal' then ld.Name
		when 'DataString' then ls.Expected
	end as LimitName,
	case Discriminator 
		when 'DataString' then ls.Expected
		when 'DataDecimal' then 
		case
			when Max is null and Min is null then null
			when Max is null and Min is not null then 
				ld.Name + ' ( ' +
				case MinType 
					when 1 then '> '
					when 2 then '>= '
				End + 
				CONVERT(VARCHAR(50), Min) + ' )'
			when Max is not null and Min is null then 
				ld.Name + ' ( ' +
				case MaxType
					when 1 then '< '
					when 2 then '<= '
				End +
				CONVERT(VARCHAR(50), Max) + ' )'
			when Max is not null and Min is not null then 
				ld.Name + ' ( ' +
				case MinType 
					when 1 then '> '
					when 2 then '>= '
				End + 
				CONVERT(VARCHAR(50), Min) + ' e ' + 
				case MaxType
					when 1 then '< '
					when 2 then '<= '
				End +
				CONVERT(VARCHAR(50), Max) + ' )' 
		end
	end as LimitDescription,
	case Discriminator 
		when 'DataDecimal' then ld.Color
		when 'DataString' then ls.Color
	end as LimitColor,
	e.Discriminator, 
	CONVERT(char(4), YEAR(e.CollectionDate)) + FORMAT(e.CollectionDate,'MM') + FORMAT(e.CollectionDate,'dd') as DateKey
	from [DataContext]..Data e
	inner join [DataDW]..[DimGroup] g 
	on e.GroupId = g.Id
	inner join [DataContext]..[Group] gD
	on gD.Id = g.Id and gD.ParentId = g.ParentId
	left join [DataContext]..LimitDecimalDenormalized ld 
	on e.GroupId = ld.GroupId and e.CollectionDate = ld.CollectionDate
	left join [DataContext]..LimitStringDenormalized ls 
	on e.GroupId = ls.GroupId and e.CollectionDate = ls.CollectionDate
	where SUBSTRING(FullName, 0, 400) + CONVERT(VARCHAR, e.CollectionDate, 120) not in (select fD.FullName + CONVERT(VARCHAR, fD.CollectionDate, 120) from [DataDW]..[FactData] fD)

	----------------------------------------------------------
	---------------- FactDataLimit ---------------------------

	insert into [DataDW]..FactDataLimit
	select 
	ISNULL(l.Id, '92DBC513-4FBD-4FB7-AC64-B5AAAD07C77D') as LimitId,
	--l2.Description,
	e.*
	from [DataDW]..FactData e
	left join [DataDW]..DimLimit l
	on e.FullName = l.FullName and ((l.Min is not null and e.DecimalValue >= l.Min and e.DecimalValue < l.Max) or (l.Min is null and e.DecimalValue < l.Max))
	where convert(nvarchar(50), ISNULL(l.Id, '92DBC513-4FBD-4FB7-AC64-B5AAAD07C77D')) + SUBSTRING(e.FullName, 0, 400) + CONVERT(VARCHAR, e.CollectionDate, 120) not in (select convert(nvarchar(50), fd.LimitId) + fD.FullName + CONVERT(VARCHAR, fD.CollectionDate, 120) from [DataDW]..[FactDataLimit] fD)

	----------------------------------------------------------
	---------------------- DimLimi ---------------------------

	insert into DimLimit
	select 
	NEWID() as Id,
	g.FullName,
	l.Name,
	case
		when min is null then CONCAT(l.Name, ' ( < ', Max, ' )')
		when min is not null then CONCAT(l.Name, ' ( >= ', Min, ' e < ', Max, ' )')
	end as Description,
	--'>=' as MinType,
	Min,
	--'<' as MaxType,
	Max,
	Priority,
	GroupId
	from [DataContext]..LimitDecimal l
	left join [DataDW]..DimGroup g
	on l.GroupId = g.id
	where convert(nvarchar(50), l.GroupId) + convert(nvarchar(50), l.[Max]) + convert(nvarchar(50), l.[Priority]) not in (select convert(nvarchar(50), l.GroupId) + convert(nvarchar(50), l.[Max]) + convert(nvarchar(50), l.[Priority]) from [DataDW]..DimLimit)

	----------------------------------------------------------

END
GO
