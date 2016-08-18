USE [TWSQLDB]
GO
/****** Object:  View [dbo].[View_賣場基本資料]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[View_賣場基本資料]
AS
SELECT     dbo.item.ID AS 賣場編號, dbo.item.Name AS 商品名稱, dbo.item.DescTW AS 中文描述, dbo.item.ItemDesc AS 英文描述, dbo.item.Sdesc AS 特色標題, 
                      dbo.item.Spechead AS 商品簡述, dbo.item.DelvType AS 配送方式, dbo.item.DelvDate AS 到貨資訊, dbo.item.ProductID AS 商品編號, 
                      dbo.item.CategoryID AS 館價ID, dbo.item.Model AS 型號, dbo.item.SellerID AS 商家ID, dbo.item.DateStart AS 賣場開始日期, 
                      dbo.item.DateEnd AS 賣場結束日期, dbo.itemdisplayprice.DisplayShipping AS 運費, dbo.itemdisplayprice.DisplayTax AS 稅金, 
                      dbo.itemdisplayprice.ItemCost AS 原幣成本, dbo.itemdisplayprice.ItemCostTW AS 台幣成本, dbo.itemdisplayprice.DisplayPrice AS 顯示售價, 
                      dbo.item.MarketPrice AS 建議售價, dbo.itemdisplayprice.ItemProfitPercent AS 毛利率, dbo.item.Qty AS 限售數量, dbo.item.ShowOrder AS 是否顯示, 
                      dbo.item.Status AS 上下狀態, dbo.item.ManufactureID AS 製造商ID, dbo.item.ShipType AS 運送類型, dbo.item.IsNew AS 新品否
FROM         dbo.item WITH (nolock) INNER JOIN
                      dbo.itemdisplayprice WITH (nolock) ON dbo.item.ID = dbo.itemdisplayprice.ItemID

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[17] 4[44] 2[20] 3) )"
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
         Begin Table = "item"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 341
               Right = 203
            End
            DisplayFlags = 280
            TopColumn = 50
         End
         Begin Table = "itemdisplayprice"
            Begin Extent = 
               Top = 6
               Left = 241
               Bottom = 341
               Right = 415
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 28
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 2790
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_賣場基本資料'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_賣場基本資料'
GO
