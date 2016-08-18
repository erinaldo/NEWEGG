USE [TWSQLDB]
GO
/****** Object:  View [dbo].[ViewTracksActive]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ViewTracksActive]
AS
SELECT     dbo.track.ACCID, dbo.track.Status AS TrackStatus, dbo.track.CreateDate, dbo.trackitem.Status, dbo.item.ID AS ItemID, dbo.item.ProductID AS ItemProductID, 
                      dbo.item.Name AS ItemName, dbo.item.PriceCash AS ItemPriceCash, dbo.item.Layout AS ItemLayout, dbo.item.PhotoName AS ItemPhotoName, 
                      dbo.itemlist.ID AS ItemListID, dbo.itemlist.ItemlistID AS ItemListItemListID, dbo.itemlist.Name AS ItemListName, dbo.itemlist.Price AS ItemListPrice, 
                      dbo.itemlistgroup.Type, dbo.item.SaleType AS ItemSaleType, dbo.item.PayType AS ItemPayType, dbo.item.DelvType AS ItemDelvType, 
                      dbo.item.DelvDate AS ItemDelvDate, dbo.item.DateStart AS ItemDateStart, dbo.item.DateEnd AS ItemDateEnd, dbo.item.DateDel AS ItemDateDel, 
                      dbo.item.Class AS ItemClass, dbo.itemlist.ItemlistProductID, dbo.itemlist.ItemlistOrder, dbo.item.ShowOrder AS ItemShowOrder, dbo.itemlistgroup.ItemlistgroupOrder, 
                      dbo.fn_EC_GetSellingQty(dbo.item.Qty, dbo.item.QtyReg, J.Qty, J.QtyReg, dbo.item.QtyLimit) AS ItemSellingQty, K.SellerID AS ItemSellerID, 
                      K.Length AS ItemProductLength, K.Width AS ItemProductWidth, K.Height AS ItemProductHeight, K.Weight AS ItemProductWeight, K.TradeTax AS ItemTradeTax, 
                      dbo.fn_EC_GetSellingQty(dbo.itemlist.Qty, dbo.itemlist.QtyReg, H.Qty, H.QtyReg, dbo.itemlist.QtyLimit) AS ItemListSellingQty, L.SellerID AS ItemListsellerid, 
                      L.Length AS ItemListProductLength, L.Width AS ItemListProductWidth, L.Height AS ItemListProductHeight, L.Weight AS ItemListProductWeight, 
                      L.TradeTax AS ItemListTradeTax
FROM         dbo.track WITH (nolock) INNER JOIN
                      dbo.item WITH (nolock) ON dbo.item.ID = dbo.track.ItemID INNER JOIN
                      dbo.itemstock AS J WITH (nolock) ON J.ProductID = dbo.item.ProductID INNER JOIN
                      dbo.product AS K WITH (nolock) ON K.ID = J.ProductID LEFT OUTER JOIN
                      dbo.itemlist WITH (nolock) ON dbo.itemlist.ItemID = dbo.item.ID AND dbo.itemlist.Status = 0 LEFT OUTER JOIN
                      dbo.trackitem WITH (nolock) ON dbo.trackitem.TrackID = dbo.track.ID AND dbo.trackitem.ItemlistID = dbo.itemlist.ID LEFT OUTER JOIN
                      dbo.itemlistgroup WITH (nolock) ON dbo.itemlistgroup.ID = dbo.itemlist.ItemlistGroupID LEFT OUTER JOIN
                      dbo.itemstock AS H WITH (nolock) ON dbo.itemlist.ItemlistProductID = H.ProductID LEFT OUTER JOIN
                      dbo.product AS L WITH (nolock) ON L.ID = H.ProductID
WHERE     (DATEDIFF(d, dbo.track.CreateDate, GETDATE()) < 30) AND (dbo.item.Status IN (0, 60))

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[50] 4[19] 2[20] 3) )"
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
         Begin Table = "track"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 125
               Right = 198
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "item"
            Begin Extent = 
               Top = 8
               Left = 782
               Bottom = 128
               Right = 947
            End
            DisplayFlags = 280
            TopColumn = 16
         End
         Begin Table = "J"
            Begin Extent = 
               Top = 126
               Left = 38
               Bottom = 245
               Right = 198
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "K"
            Begin Extent = 
               Top = 126
               Left = 236
               Bottom = 245
               Right = 432
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "itemlist"
            Begin Extent = 
               Top = 246
               Left = 38
               Bottom = 365
               Right = 210
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "trackitem"
            Begin Extent = 
               Top = 246
               Left = 248
               Bottom = 365
               Right = 408
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "itemlistgroup"
            Begin Extent = 
               Top = 366
               Left = 38
               Bottom = 485
               Right = 218
            End
            DisplayFlags = 280
       ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ViewTracksActive'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'     TopColumn = 0
         End
         Begin Table = "H"
            Begin Extent = 
               Top = 366
               Left = 256
               Bottom = 485
               Right = 416
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "L"
            Begin Extent = 
               Top = 486
               Left = 38
               Bottom = 605
               Right = 234
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
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 1740
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ViewTracksActive'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ViewTracksActive'
GO
