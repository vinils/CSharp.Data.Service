use [DataDW]
delete from FactData
delete from FactDataLimit
delete from DimLimit
delete from DimGroup
--delete from [DataDW]..DimDate
go

------------------------ GROUP ---------------------------
    WITH Groups AS (
-- This is end of the recursion: Select items with no parent
        SELECT 
			id, 
			ParentId, 
			Name, 
			Initials, 
			Name FullName,
			MeasureUnit, 
			0 Level, 
			Name + '#$##$#$##0' FullNameWithSeq
        FROM [DataContextContext-20180321055800]..[Group]
        WHERE ParentId is null
    UNION ALL
-- This is the recursive part: It joins to CTE
        SELECT 
			t.id, 
			t.ParentId, 
			t.Name,
			t.Initials,
			c.FullName + '\' + t.Name,
			t.MeasureUnit, 
			c.Level + 1 as Level, 
			c.FullNameWithSeq + '\' + t.Name + '#$##$#$##' + CONVERT(VARCHAR(128), c.Level + 1)
        FROM [DataContextContext-20180321055800]..[Group] t
        INNER JOIN Groups c ON t.ParentId = c.id
    )
    SELECT * 
	into ##Groups2
--drop table #Groups2
	FROM Groups
    ORDER BY FullNameWithSeq;

select 
	id as id,
	ParentId,
	FullName,
	MeasureUnit,
	Name,
	Initials,
	SUBSTRING(value, CHARINDEX('#$##$#$##', value) + 9 , 1) as SubLevel,
	SUBSTRING(value, 0 , CHARINDEX('#$##$#$##', value)) as SubName
into ##Groups3
--drop table #Groups3
from  ##Groups2 cross apply string_split(fullNameWithSeq, '\')

insert into [DataDW]..[DimGroup]
select
SUBSTRING(FullName, 0, 400) AS FullName,
[0] as level0,
[1] as level1,
[2] as level2,
[3] as level3,
[4] as level4,
[5] as level5,
[6] as level6,
[7] as level7,
[8] as level8,
[9] as level9,
Id,
ParentId
from ##Groups3
pivot
(
	max(SubName)
	for SubLevel in ([0],[1],[2],[3],[4],[5],[6],[7],[8],[9])) as p

----------------------------------------------------------
---------------- DimLimit --------------------------------

insert into [DataDW]..[DimLimit]
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
from [DataContextContext-20180321055800]..LimitDecimal l
left join [DataDW]..DimGroup g
on l.GroupId = g.id

insert into [DataDW]..[DimLimit] (Id, FullName, Name, Description, Min, Max, Priority) values
('92DBC513-4FBD-4FB7-AC64-B5AAAD07C77D', null, 'Sem Limite', 'Sem Limite', null, null, 0)

insert into [DataDW]..[DimLimit]
select
NEWID() as Id,
FullName,
'Above the higher limit' as Name,
CONCAT('Above the higher limit ( >= ', Max(Max), ' )') as Description,
Max(Max) as Min,
null as Max,
Priority,
GroupId
from [DataDW]..DimLimit
where GroupId is not null
group by
FullName,
GroupId,
Priority

----------------------------------------------------------
---------------------- FactData --------------------------

insert into [DataDW]..[FactData]
select 
--top 1
SUBSTRING(FullName, 0, 400) as FullName, 
e.CollectionDate, 
g.Name, 
g.Initials, 
e.DecimalValue,
g.MeasureUnit,
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
from [DataContextContext-20180321055800]..[Data] e
inner join ##Groups2 g 
on e.GroupId = g.Id
left join [DataContextContext-20180321055800]..LimitDecimalDenormalized ld 
on e.GroupId = ld.GroupId and e.CollectionDate = ld.CollectionDate
left join [DataContextContext-20180321055800]..LimitStringDenormalized ls 
on e.GroupId = ls.GroupId and e.CollectionDate = ls.CollectionDate

----------------------------------------------------------
---------------- FactDataLimit ---------------------------

insert into [DataDW]..[FactDataLimit]
select 
ISNULL(l.Id, '92DBC513-4FBD-4FB7-AC64-B5AAAD07C77D') as LimitId,
--l2.Description,
e.*
from [DataDW]..FactData e
left join [DataDW]..DimLimit l
on e.FullName = l.FullName and ((l.Min is not null and e.DecimalValue >= l.Min and e.DecimalValue < l.Max) or (l.Min is null and e.DecimalValue < l.Max))

----------------------------------------------------------
