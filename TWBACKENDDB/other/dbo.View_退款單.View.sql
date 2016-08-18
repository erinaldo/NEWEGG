USE [TWBACKENDDB]
GO
/****** Object:  View [dbo].[View_退款單]    Script Date: 2016/08/18 13:04:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[View_退款單]
AS
SELECT     ID AS 退款單序號, Code AS 退款單號, Date AS 退款產生日期, PayDate AS 付款日期, Amount AS 退款金額, FinalDate AS 退款完成日, Status AS 退款狀態, 
                      Cause AS 退款原因代碼, BankID AS 退款銀行編號, BankName AS 退款銀行編號_名稱, SubBankName AS 退款分行名稱, AccountName AS 退款人名稱, 
                      AccountNO AS 退款人退款帳號, CauseNote AS 退款原因, ApplyDate AS 同意退款日期, ProcessID AS 訂單子單編號, RetgoodID AS 退貨單編號, 
                      InvoiceResult AS 發票狀態, InvoiceNO AS 發票號碼, InvoiceDate AS 發票日期, InvoicePrice AS 發票金額, CreateDate AS 產生日期, UpdateUser AS 最後更新人, 
                      CartID AS 訂單主單編號, Note AS 修改紀錄, OnRefundDate AS 退款處理日期, AbnRefundDate AS 退款異常日期, CancelRefundDate AS 退款取消日期, 
                      UpdateNote AS 退款詳細更新說明, ABNRefundReason AS 退款異常原因, CreateUser AS 退款單建立人
FROM         dbo.refund2c

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = -5226
         Left = 0
      End
      Begin Tables = 
         Begin Table = "refund2c"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 309
               Right = 319
            End
            DisplayFlags = 280
            TopColumn = 39
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1800
         Alias = 1935
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_退款單'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_退款單'
GO
