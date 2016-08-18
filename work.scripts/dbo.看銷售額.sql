--dbo.看銷售額
use TWSQLDB

select SUBSTRING( a.YYYYMMDD,1,6) as '年月', 
	SUM(PriceSum) as '總銷售額', 
	(SUM(PriceSum)/100)*12 as '預估毛利' 
	from (
		select convert(char, CreateDate, 112) as YYYYMMDD, PriceSum  
		from dbo.salesordergroup
) as a
where SUBSTRING( a.YYYYMMDD,1,4)>='2011'
group by SUBSTRING( a.YYYYMMDD,1,6)
order by SUBSTRING( a.YYYYMMDD,1,6)

