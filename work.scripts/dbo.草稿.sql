use TWSQLDB_PRD

declare @name varchar(20) =N'§Ú¬O¯ó½Z';

select top 1 * from ItemSketch
--where Name=@name;
where ID='34673'

select  top 1 * from itemtemp
--where Name=@name;
where ID='551254'
order by CreateDate desc 

select top 1 * from producttemp
--where Name=@name;
where ID='157037'
order by CreateDate desc 

select top 1 * from item
--where Name=@name;
--order by CreateDate desc 
where ID='489825'


select top 1 * from product
--where Name=@name;
where ID='502896'
--order by CreateDate desc 



