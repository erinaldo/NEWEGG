USE [TWBACKENDDB]
GO
/****** Object:  StoredProcedure [dbo].[UP_EC_Refund2cGetNumber]    Script Date: 2016/08/18 13:04:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE PROCEDURE [dbo].[UP_EC_Refund2cGetNumber]
AS
Declare @get_Refund2ccode nvarchar(15)
Declare @run_repeat int
set @run_repeat=0
Declare @dboutput nvarchar(50)
set @dboutput=''




	set @get_Refund2ccode=dbo.fn_EC_GetRefund2cAutoSN('LBT')
select @get_Refund2ccode as Refund2cID

				/*if(@@ERROR<>0)
					begin
						set @run_repeat=@run_repeat+1
						
						
						CLOSE mycursor
						DEALLOCATE mycursor
						ROLLBACK TRANSACTION
						set @dboutput='getsalesorderitemAutoSN is error.'
						return 
					end
				set @run_repeat=0*/







GO
