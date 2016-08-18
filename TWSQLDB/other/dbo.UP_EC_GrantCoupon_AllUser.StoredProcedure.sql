USE [TWSQLDB]
GO
/****** Object:  StoredProcedure [dbo].[UP_EC_GrantCoupon_AllUser]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UP_EC_GrantCoupon_AllUser]
       @dboutput varchar(100) OUTPUT 
AS  --先處理全部會員
DECLARE @Couponmax int;
SET @Couponmax = 0;
DECLARE @EventId int;
SET @EventId = 0;
DECLARE @Items nvarchar(max);
SET @Items = '';
DECLARE @Categories nvarchar(max);
SET @Categories = '';
DECLARE @CouponValidStart datetime;
DECLARE @CouponValidEnd datetime;
DECLARE @AccountId int;
SET @AccountId=0;
DECLARE @Value int;
SET @Value = 0;
DECLARE @Couponsum int;
Set @Couponsum = 0;
DECLARE @Couponmarketnumber nvarchar(20);
Set @Couponmarketnumber = '';
DECLARE @CouponActiveType int;
Set @CouponActiveType = 0;
DECLARE @CouponLimit int;
set @CouponLimit = 0;
DECLARE @ValidType int;
Set @ValidType = 0;
DECLARE @CouponUsedStatus int;
Set @CouponUsedStatus = 0;

DECLARE @i int;
SET @i = 0;

/*
GrantStatus: 0:NotGrant, 1:WaitingForGranting, 2:Granted, 3:BreakGrantedByUser, 4:BreakGrantedByOther, 5:Granting
CouponAction: 0:NotActive, 1:SystemAutoActive, 2:MemberActive
FilterFileUsage: 0:DoNotUser, 1:Use
*/
--完整的coupon number由year(1) + event(5) + coupon(7)組成
DECLARE @number_year char(1)
DECLARE @number_event char(5);
DECLARE @number_coupon char(7);

--處理發送全user coupon : event.filterfileusage = 0
DECLARE @cur_event CURSOR 
SET @cur_event = Cursor for
( SELECT 
          id 
         ,couponmax 
         ,char(datepart(YEAR, createdate )-1948) 
		 ,items
         ,couponcategories 
         ,couponvalidstart 
         ,couponvalidend 
		 ,value
		 ,couponsum
		 ,couponmarketnumber
		 ,couponactivetype
		 ,limit
      FROM event WITH (NOLOCK) 
      WHERE 
          GETDATE() BETWEEN event.datestart 
          AND event.dateend 
          AND event.grantstatus=1  --設定待發放
		  AND event.couponactivetype != 2 --不可為兌碼啟動
          AND event.filterfileusage=0 
   
   )

DECLARE @tmp_tableAccount TABLE 
(	
     ID int	
);

INSERT INTO @tmp_tableAccount 
SELECT 
    ID  
FROM account WITH (NOLOCK) 

OPEN @cur_event

FETCH NEXT FROM @cur_event INTO @EventId
                              ,@Couponmax
                              ,@number_year
							  ,@Items
                              ,@Categories
                              ,@CouponValidStart
                              ,@CouponValidEnd
							  ,@Value
							  ,@Couponsum
							  ,@Couponmarketnumber
							  ,@Couponactivetype
							  ,@CouponLimit;
WHILE (@@FETCH_STATUS=0)
begin
	--設定Event為發送中的狀態
	UPDATE [event] 
    SET grantstatus=5
       ,grantstart=GETDATE()
       ,updated = updated +1
	   where id=@EventId
	--set @number_event = dbo.fn_EC_PadLeft('0',5,@EventId);
	set @number_event = REPLACE(str(@EventId, 5), SPACE(1), '0');
	set @i = 0;
--判斷目前發送的數量是否在規定的範圍裡, 數量符合才可以執行, @Couponmax為0或是null表示不限制
if(@Couponmax is null or (@Couponmax is not null and (@Couponmax = 0 or @Couponsum < @Couponmax)))
begin
	DECLARE cur_account CURSOR 
    FOR (SELECT 
             ID 
         FROM @tmp_tableAccount
      )
	open cur_account;
	FETCH NEXT FROM cur_account INTO @AccountId;
	WHILE (@@FETCH_STATUS = 0)
	begin
        if(@Couponmax is null or (@Couponmax is not null and (@Couponmax = 0 or @i < @Couponmax)))
		begin
			set @i = @i + 1;
			--set @number_coupon = dbo.fn_EC_PadLeft('0', 7, @i);
			set @number_coupon = REPLACE(str(@i, 7), SPACE(1), '0');
			Set @ValidType = 0;
			Set @CouponUsedStatus = 0;
			if(@CouponActiveType = 1)
				begin
					Set @ValidType = 1
					Set @CouponUsedStatus = 1;		
				end
			insert into coupon (number, eventid, accountid, items, categories, validstart, validend, usestatus, value, createuser, createdate, marketnumber, activetype, validtype, limit)
			values(@number_year+@number_event+@number_coupon, @EventId, @AccountId, @Items, @Categories, @CouponValidStart, @CouponValidEnd, @CouponUsedStatus, @Value, 'sys_sync', GETDATE(), @Couponmarketnumber, @CouponActiveType, @ValidType, @CouponLimit);

			fetch next from cur_account into @AccountId;
		End
	    Else
        Begin
		    break;
        End
	end	--cur_account
	close cur_account
	deallocate cur_account
end
--設定發送完成
	update [event] set couponsum=couponsum + @i, grantstatus=2, grantend=GETDATE(), updated = updated +1 where id=@EventId
	FETCH NEXT FROM @cur_event INTO @EventId
                                  ,@Couponmax
                                  ,@number_year
								  ,@Items
                                  ,@Categories
                                  ,@CouponValidStart
                                  ,@CouponValidEnd
								  ,@Value
								  ,@Couponsum
								  ,@Couponmarketnumber
								  ,@Couponactivetype
								  ,@CouponLimit;
END --cur_event
close @cur_event;



--處理附檔名單 & 自訂條件: event.filterfileusage = 1 or event.filterfileusage = 2
set @i = 0;
set @cur_event = cursor for
(
SELECT 
          id 
         ,couponmax 
         ,char(datepart(YEAR, createdate )-1948) 
		 ,items
         ,couponcategories 
         ,couponvalidstart 
         ,couponvalidend
		 ,value
		 ,couponsum
		 ,couponmarketnumber
		 ,couponactivetype
		 ,limit
      FROM event WITH (NOLOCK) 
      WHERE 
          GETDATE() BETWEEN event.datestart 
          AND event.dateend 
		AND event.grantstatus=1  --設定待發放
		  AND event.couponactivetype != 2 --不可為兌碼啟動
          AND (event.filterfileusage=1 or event.filterfileusage=2)
)
open @cur_event

FETCH NEXT FROM @cur_event INTO @EventId
                              ,@Couponmax
                              ,@number_year
							  ,@Items
                              ,@Categories
                              ,@CouponValidStart
                              ,@CouponValidEnd
							  ,@Value
							  ,@Couponsum
							  ,@Couponmarketnumber
							  ,@Couponactivetype
							  ,@CouponLimit;
WHILE (@@FETCH_STATUS=0)
begin
	--設定Event為發送中的狀態
	UPDATE [event]
    SET grantstatus=5
       ,grantstart=GETDATE()
       ,updated = updated +1
       where id=@EventId

		set @i = @Couponsum;
		--set @number_event = dbo.fn_EC_PadLeft('0',5,@EventId);
		set @number_event = REPLACE(str(@EventId, 5), SPACE(1), '0');

delete @tmp_tableAccount;
--判斷發送的數是是否在規範裡
if(@Couponmax is null or(@Couponmax is not null and (@Couponmax = 0 or @Couponsum < @Couponmax)))
begin
INSERT INTO @tmp_tableAccount  SELECT account_id as ID FROM eventtempimport WITH (NOLOCK) where event_id=@EventId

	DECLARE cur_account CURSOR 
    FOR (SELECT 
             ID 
         FROM @tmp_tableAccount
      )
	open cur_account;
	FETCH NEXT FROM cur_account INTO @AccountId;
	WHILE (@@FETCH_STATUS = 0)
	begin
	if(@Couponmax is null or (@Couponmax is not null and(@Couponmax=0 or @i < @Couponmax)))
		begin
			set @i = @i + 1;
			--set @number_coupon = dbo.fn_EC_PadLeft('0', 7, @i);
			set @number_coupon = REPLACE(str(@i, 7), SPACE(1), '0');
			Set @ValidType = 0;
			Set @CouponUsedStatus = 0;
			if(@CouponActiveType = 1)
				begin
					Set @ValidType = 1
					Set @CouponUsedStatus = 1;
				end
			insert into coupon  (number, eventid, accountid, items, categories, validstart, validend, usestatus, value, createuser, createdate, marketnumber, activetype, validtype, limit)
			values(@number_year+@number_event+@number_coupon, @EventId, @AccountId, @Items, @Categories, @CouponValidStart, @CouponValidEnd, @CouponUsedStatus, @Value, 'sys_sync', GETDATE(), @Couponmarketnumber, @CouponActiveType, @ValidType, @CouponLimit);
			update eventtempimport set usageflag=1 where account_id=@AccountId and event_id=@EventId;
		
			fetch next from cur_account into @AccountId;
		end
	else
		begin
			break;
		end
	end	--cur_account
	close cur_account
	deallocate cur_account
	--刪除eventfileimport的記錄
	delete eventtempimport where event_id=@EventId and usageflag = 1;
end
--更改狀態為發送完成
	update [event] set couponsum=couponsum + @i, grantstatus=2, grantend=GETDATE(), updated = updated +1 where id=@EventId
	FETCH NEXT FROM @cur_event INTO @EventId
                                  ,@Couponmax
                                  ,@number_year
								  ,@Items
                                  ,@Categories
                                  ,@CouponValidStart
                                  ,@CouponValidEnd
								  ,@Value
								  ,@Couponsum
								  ,@Couponmarketnumber
								  ,@Couponactivetype
								  ,@CouponLimit;
end
close @cur_event


deallocate @cur_event


SELECT  @dboutput=CONVERT(varchar(100),GETDATE())

GO
