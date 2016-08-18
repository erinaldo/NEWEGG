USE [TWSQLDB]
GO
/****** Object:  StoredProcedure [dbo].[UP_EC_GrantCoupon_Dynamic]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UP_EC_GrantCoupon_Dynamic]
	-- Add the parameters for the stored procedure here
	@EventId int,
	@UserAccountId nvarchar(50)

	--處理的狀態, 0:未處理, 1:活動已截止, 2: Coupon發送已達上限 3:User已領過, 4:發送成功, 5:發送失敗
	--@ReturnCouponMessage int output
AS
DECLARE @Couponmax int;
SET @Couponmax = 0;
DECLARE @Items nvarchar(max);
SET @Items = '';
DECLARE @Categories nvarchar(max);
SET @Categories = '';
DECLARE @CouponValidStart datetime;
DECLARE @CouponValidEnd datetime;
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
Set @ValidType = 2; --靠User啟動
DECLARE @CouponUsedStatus int;
Set @CouponUsedStatus = 0;
DECLARE @Couponactiveusagedays int;
Set @Couponactiveusagedays = 0;
DECLARE @Couponreget int;
Set @Couponreget = 1; --單一活動中, 單一User最多可領取的張數

DECLARE @i int;
SET @i = 0;

--可處理的列數
DECLARE @ProcessCount int;
Set @ProcessCount = 0;	

declare @ReturnCouponMessage int;
set @ReturnCouponMessage = 0;



/*
GrantStatus: 0:NotGrant, 1:WaitingForGranting, 2:Granted, 3:BreakGrantedByUser, 4:BreakGrantedByOther, 5:Granting
CouponAction: 0:NotActive, 1:SystemAutoActive, 2:MemberActive
FilterFileUsage: 0:DoNotUser, 1:Use
*/
--完整的coupon number由year(1) + event(5) + coupon(7)組成
DECLARE @number_year char(1)
DECLARE @number_event char(5);
DECLARE @number_coupon char(7);


--取得符合條件的Event
Select @ProcessCount = count(*)  From event With(NOLOCK)
where
GETDATE() BETWEEN event.datestart AND event.dateend 
		  AND event.id = @EventId --活動ID
          AND event.grantstatus=1  --設定待發放
		  AND event.couponactivetype = 2 --設定兌碼啟動
          AND event.filterfileusage=0 -- 設定為全會員
if(@ProcessCount <= 0)
begin
SELECT  @ReturnCouponMessage=1; --活動已截止或找不到此活動
end
else
begin
	--確認User是否領過
	Set @ProcessCount = 0;

select  @EventId=a.eventid, @UserAccountId=a.accountid, @Couponreget=b.couponreget, @ProcessCount= count(*) from coupon  a with(nolock)right join event b with(nolock) on a.eventid=b.id
where a.eventid=@EventId and a.accountid=@UserAccountId
group by a.eventid, a.accountid, b.couponreget



	Select @ProcessCount=count(*) From coupon With(NOLOCK)
	Where
		coupon.eventid = @EventId
		And coupon.accountid = @UserAccountId
	if(@ProcessCount > 0 and @Couponreget > 0 and @ProcessCount >=@Couponreget)
	begin
		select @ReturnCouponMessage = 3; --User已領過
	end
	else
	begin

	-----------------------------------開始執行發送-----------------------------------
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
		 ,couponactiveusagedays
      FROM event WITH (NOLOCK) 
      WHERE 
          GETDATE() BETWEEN event.datestart 
          AND event.dateend 
		  AND event.id = @EventId --活動ID
          AND event.grantstatus=1  --設定待發放
		  AND event.couponactivetype = 2 --設定兌碼啟動
          AND event.filterfileusage=0 -- 設定為全會員
   
   )

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
							  ,@CouponLimit
							  ,@Couponactiveusagedays;
WHILE (@@FETCH_STATUS=0)
begin
	set @i = @Couponsum;
	set @number_event = dbo.fn_EC_PadLeft('0'
       ,5
       ,@EventId
 );
--判斷目前發送的數量是否在規定的範圍裡, 數量符合才可以執行, @Couponmax為0或是null表示不限制
if(@Couponmax is null or (@Couponmax is not null and (@Couponmax = 0 or @Couponsum < @Couponmax)))
begin
		set @i = @i + 1;
		set @number_coupon = dbo.fn_EC_PadLeft('0', 7, @i);
		Set @ValidType = 2; --靠User啟動
		--------判斷Coupon的有效期間
		if(@Couponactiveusagedays is not null and @Couponactiveusagedays != 0)
		begin
			if(GETDATE() >= @CouponValidStart)
			begin
				Set @CouponValidStart = Getdate();
			end
		
			if((DATEADD (DAY , @Couponactiveusagedays , @CouponValidStart )) < @CouponValidEnd)
			begin
				select @CouponValidEnd =DATEADD (DAY , @Couponactiveusagedays , @CouponValidStart )
			end
		end
		--------end of 判斷Coupon的有效期間
		Set @CouponUsedStatus = 1;
		--------開始發送
		BEGIN TRANSACTION grantCoupon
			insert into coupon (number, eventid, accountid, items, categories, validstart, validend, usestatus, value, createuser, createdate, marketnumber, activetype, validtype, limit)
			values(@number_year+@number_event+@number_coupon, @EventId, cast(@UserAccountId as int), @Items, @Categories, @CouponValidStart, @CouponValidEnd, @CouponUsedStatus, @Value, 'sys_sync', GETDATE(), @Couponmarketnumber, @CouponActiveType, @ValidType, @CouponLimit);
			update event set event.couponsum = event.couponsum +1 where id=@EventId;
		if(@@ERROR = 0)
		begin
			select @CouponSum = CouponSum, @CouponMax = CouponMax from event where id=@EventId;
			if(@Couponmax is not null and  @Couponmax != 0 and @Couponsum > @Couponmax)
			begin
				ROLLBACK TRAN grantCoupon
				select @ReturnCouponMessage = 2; --Coupon發送已達上限
			end
			else
			begin
				COMMIT TRANSACTION grantCoupon;
				select @ReturnCouponMessage = 4;  --發送成功
			end
		end
		else
		begin
			ROLLBACK TRAN grantCoupon
			select @ReturnCouponMessage = 5; --發送失敗
		end
		break; --僅發送一張Coupon即跳出
		--------發送結束
end
else
begin
	select @ReturnCouponMessage = 2; --Coupon發送已達上限
end --end of判斷目前發送的數量
--設定發送完成
	
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
								  ,@CouponLimit
								  ,@Couponactiveusagedays
END --cur_event
close @cur_event; --關閉指標
deallocate @cur_event --釋放指標
-----------------------------------完成執行發送-----------------------------------

	end -- end of 確認User是否領過
end -- end of 取得符合條件的Event

SELECT  @ReturnCouponMessage	--回傳結果
GO
