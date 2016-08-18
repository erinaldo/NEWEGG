USE [TWSQLDB]
GO
/****** Object:  StoredProcedure [dbo].[UP_EC_GrantCoupon_AllUser_BAK]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UP_EC_GrantCoupon_AllUser_BAK]
       @dboutput varchar(100) OUTPUT 
AS  --先處理全部會員
--處理發送全user coupon : event.filterfileusage = 0
DECLARE @Couponmax int;
SET @Couponmax = 0;
DECLARE @EventId int;
SET @EventId = 0;
DECLARE @Categories nvarchar(50);
SET @Categories = '';
DECLARE @CouponValidStart datetime;
DECLARE @CouponValidEnd datetime;
DECLARE @AccountId int;
SET @AccountId=0;
DECLARE @Value int;
SET @Value = 0;

DECLARE @i int;
SET @i = 0;


--完整的coupon number由year(1) + event(5) + coupon(7)組成
DECLARE @number_year char(1)
DECLARE @number_event char(5);
DECLARE @number_coupon char(7);


DECLARE @cur_event CURSOR 
SET @cur_event = Cursor for
( SELECT 
          id 
         ,couponmax 
         ,char(datepart(YEAR, createdate )-1948) 
         ,couponcategories 
         ,couponvalidstart 
         ,couponvalidend 
		 ,value
      FROM event WITH (NOLOCK) 
      WHERE 
          GETDATE() BETWEEN event.datestart 
          AND event.dateend 
          AND event.grantstatus=1 
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
                              ,@Categories
                              ,@CouponValidStart
                              ,@CouponValidEnd
							  ,@Value;
WHILE (@@FETCH_STATUS=0)
begin
	--設定Event為發送中的狀態
	UPDATE [event] 
    SET grantstatus=4
       ,grantstart=GETDATE()
       ,updatedate=GETDATE()
       ,updated = updated +1
       ,updateuser='sys_sync' where id=@EventId
	set @i = @Couponmax;
	set @number_event = dbo.fn_EC_PadLeft('0'
       ,5
       ,@EventId
 );
	DECLARE cur_account CURSOR 
    FOR (SELECT 
             ID 
         FROM @tmp_tableAccount
      )
	open cur_account;
	FETCH NEXT FROM cur_account INTO @AccountId;
	WHILE (@@FETCH_STATUS = 0)
	begin
		set @i = @i + 1;
		set @number_coupon = dbo.fn_EC_PadLeft('0', 7, @i);
		insert into coupon (number, eventid, accountid, categories, validstart, validend, usestatus, value, createuser, createdate)
		values(@number_year+@number_event+@number_coupon, @EventId, @AccountId, @Categories, @CouponValidStart, @CouponValidEnd, 0, @Value, 'sys_sync', GETDATE());

		fetch next from cur_account into @AccountId;
	end	--cur_account
	close cur_account
	deallocate cur_account
	update [event] set couponmax=@i, grantstatus=2, grantend=GETDATE(), updatedate=GETDATE(), updated = updated +1, updateuser='sys_sync' where id=@EventId
	FETCH NEXT FROM @cur_event INTO @EventId
                                  ,@Couponmax
                                  ,@number_year
                                  ,@Categories
                                  ,@CouponValidStart
                                  ,@CouponValidEnd
								  ,@Value;
END --cur_event
close @cur_event;



--處理附檔名單 & 自訂條件: event.filterfileusage = 2 or event.filterfileusage = 3
set @cur_event = cursor for
(
SELECT 
          id 
         ,couponmax 
         ,char(datepart(YEAR, createdate )-1948) 
         ,couponcategories 
         ,couponvalidstart 
         ,couponvalidend
		 ,value
      FROM event WITH (NOLOCK) 
      WHERE 
          GETDATE() BETWEEN event.datestart 
          AND event.dateend 
          AND event.grantstatus=1 
          AND (event.filterfileusage=2 or event.filterfileusage=3)
)
open @cur_event

FETCH NEXT FROM @cur_event INTO @EventId
                              ,@Couponmax
                              ,@number_year
                              ,@Categories
                              ,@CouponValidStart
                              ,@CouponValidEnd
							  ,@Value;
WHILE (@@FETCH_STATUS=0)
begin
	--設定Event為發送中的狀態
	UPDATE [event] 
    SET grantstatus=4
       ,grantstart=GETDATE()
       ,updatedate=GETDATE()
       ,updated = updated +1
       ,updateuser='sys_sync' where id=@EventId

		set @i = @Couponmax;
		set @number_event = dbo.fn_EC_PadLeft('0',5,@EventId);

delete @tmp_tableAccount;
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
		set @i = @i + 1;
		set @number_coupon = dbo.fn_EC_PadLeft('0', 7, @i);
		insert into coupon (number, eventid, accountid, categories, validstart, validend, usestatus, value, createuser, createdate)
		values(@number_year+@number_event+@number_coupon, @EventId, @AccountId, @Categories, @CouponValidStart, @CouponValidEnd, 0, @Value, 'sys_sync', GETDATE());
		update eventtempimport set usageflag=1 where account_id=@AccountId and event_id=@EventId;

		fetch next from cur_account into @AccountId;
	end	--cur_account
	close cur_account
	deallocate cur_account
	--刪除eventfileimport的記錄
	delete eventtempimport where event_id=@EventId and usageflag = 1;

	update [event] set couponmax=@i, grantstatus=2, grantend=GETDATE(), updatedate=GETDATE(), updated = updated +1, updateuser='sys_sync' where id=@EventId
	FETCH NEXT FROM @cur_event INTO @EventId
                                  ,@Couponmax
                                  ,@number_year
                                  ,@Categories
                                  ,@CouponValidStart
                                  ,@CouponValidEnd
								  ,@Value;
end
close @cur_event


deallocate @cur_event


SELECT  @dboutput=CONVERT(varchar(100),GETDATE())

GO
