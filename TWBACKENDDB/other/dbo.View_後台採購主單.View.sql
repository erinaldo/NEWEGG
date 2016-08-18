USE [TWBACKENDDB]
GO
/****** Object:  View [dbo].[View_後台採購主單]    Script Date: 2016/08/18 13:04:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[View_後台採購主單]
AS
SELECT     Code AS 採購主單編號, SalesorderCode AS 訂單編號, Name AS 客戶名稱, AccountID AS 客戶編號, TelDay AS 日間電話, TelNight AS 夜間電話, Mobile AS 手機, Email, 
                      PayType AS 付款方式, RecvName AS 收件人名稱, RecvTelDay AS 收件人日間電話, RecvTelNight AS 收件人夜間電話, RecvMobile AS 收件人手機, 
                      DELIVType AS 配送方式, DELIVData AS 配送日期, DELIVLOC AS 配送國家, DELIVZip AS 配送郵遞區號, DELIVADDR AS 配送地址, DELIVNO AS [Tracking no1], 
                      ForwardNO AS [Tracking no2], InventoryStatus AS 出貨狀態, ACTCode AS 公司統編, Status AS 狀態, DelvStatus AS 配送狀態, DelvStatusdate AS 配送狀態日期, 
                      Forwarder AS [Tracking no2貨運業者], ASNNumber AS Wms回傳進貨號碼, InventoryStatusDate AS 存貨日期, InventoryStatusUser AS 存貨更新人, 
                      WareHouse AS 倉別代碼, RecvENGName AS 收件人英文名, DelivENGADDR AS 配送英文地址
FROM         dbo.purchaseorder

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[20] 4[41] 2[20] 3) )"
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
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "purchaseorder"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 312
               Right = 379
            End
            DisplayFlags = 280
            TopColumn = 68
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
         Column = 1755
         Alias = 3420
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_後台採購主單'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_後台採購主單'
GO
