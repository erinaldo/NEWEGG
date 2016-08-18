--dbo.訂單相關優惠活動SQL語法.sql
SELECT TOP 1000 [PromotionGiftBasicID]
      ,[SalesOrderItemCode]
      ,[PromotionGiftIntervalID]
      ,[DiscountAmount]
      ,[UsedStatus]
      ,[ApportionedAmount]
      ,[SAIn]
      ,[SAOut]
      ,[CreateDate]
      ,[UpdateDate]
      ,[UpdateUser]
  FROM [TWSQLDB_PRD].[dbo].[PromotionGiftRecords]order by CreateDate desc


select *from   [TWSQLDB_PRD].[dbo].[PromotionGiftBasic]where ID='124'
select coupons from   [TWBACKENDDB_PRD].[dbo].Process  where ID='LBS160226000003'
select *   FROM [TWSQLDB_PRD].[dbo].[coupon] where ordcode like '%LBS150123000007%' 

select coupons,CartID from   [TWBACKENDDB_PRD].[dbo].Process  where ID='LBS150123000007'


select * FROM [TWSQLDB_PRD].[dbo].[event] where ID='201'