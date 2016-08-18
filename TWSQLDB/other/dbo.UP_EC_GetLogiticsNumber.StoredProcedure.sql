USE [TWSQLDB]
GO
/****** Object:  StoredProcedure [dbo].[UP_EC_GetLogiticsNumber]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--SP_UP_EC_GetLogiticsNumber=======================================
CREATE PROCEDURE [dbo].[UP_EC_GetLogiticsNumber]
	@LogiticsCode char(1)
As
	Declare @Number int;
	Set @Number = 0;

	--回傳結果
	Select @Number = TodayNumber from GenerateNumber where LogiticsCode = @LogiticsCode
	if(@Number is null)
	begin
		Set @Number = 0;
	end
	else
	begin
		Set @Number = @Number + 1;
		Update GenerateNumber Set TodayNumber = TodayNumber + 1 where LogiticsCode = @LogiticsCode
	end

select @Number
GO
