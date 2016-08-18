use TWSQLDB_PRD

select count(ShowOrder) as showorder_count, ShowOrder as showorder_value from dbo.item
--where ShowOrder=0
group by ShowOrder
