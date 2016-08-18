--dbo.�ݾP���B
use TWSQLDB

select SUBSTRING( a.YYYYMMDD,1,6) as '�~��', 
	SUM(PriceSum) as '�`�P���B', 
	(SUM(PriceSum)/100)*12 as '�w����Q' 
	from (
		select convert(char, CreateDate, 112) as YYYYMMDD, PriceSum  
		from dbo.salesordergroup
) as a
where SUBSTRING( a.YYYYMMDD,1,4)>='2011'
group by SUBSTRING( a.YYYYMMDD,1,6)
order by SUBSTRING( a.YYYYMMDD,1,6)

