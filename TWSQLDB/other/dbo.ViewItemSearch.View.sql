USE [TWSQLDB]
GO
/****** Object:  View [dbo].[ViewItemSearch]    Script Date: 2016/08/18 13:11:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ViewItemSearch]
AS
SELECT     I.ID, I.Name AS ItemName, I.Sdesc, I.SaleType, I.PayType, I.DateStart, I.DateEnd, I.Pricesgst, 
                      CAST(CASE WHEN IDP.DisplayPrice > 0 THEN IDP.DisplayPrice ELSE I.PriceCash END AS Decimal(12, 2)) AS PriceCash, dbo.fn_EC_GetSellingQty(I.Qty, I.QtyReg, 
                      J.Qty, J.QtyReg, I.QtyLimit) AS SellingQty, I.QtyReg, I.PhotoName, I.ShowOrder, C.ID AS CategoryID, C.Layer, C.Title, M.ID AS ManufactureID, 
                      M.Name AS ManufactureName, P.Model, I.UpdateDate, I.Spechead, I.HtmlName, P.Name AS ProductName, P.SellerProductID, I.SellerID, S.Name AS SellerName, 
                      S.CountryID, I.PriceGlobalship
FROM         dbo.item AS I WITH (nolock) INNER JOIN
                      dbo.category AS C WITH (nolock) ON I.CategoryID = C.ID INNER JOIN
                      dbo.product AS P WITH (nolock) ON P.ID = I.ProductID INNER JOIN
                      dbo.itemstock AS J WITH (nolock) ON J.ProductID = I.ProductID LEFT OUTER JOIN
                      dbo.manufacture AS M WITH (nolock) ON M.ID = P.ManufactureID INNER JOIN
                      dbo.seller AS S WITH (nolock) ON I.SellerID = S.ID LEFT OUTER JOIN
                          (SELECT     s1.ID, s1.ItemID, s1.PriceType, s1.MinNumber, s1.MaxNumber, s1.StartDate, s1.EndDate, s1.DisplayPrice, s1.DisplayTax, s1.DisplayShipping, 
                                                   s1.ItemCost, s1.ItemCostTW, s1.ItemProfitPercent, s1.CreateDate, s1.CreateUser, s1.Updated, s1.UpdateDate, s1.UpdateUser
                            FROM          dbo.itemdisplayprice AS s1 WITH (nolock) LEFT OUTER JOIN
                                                   dbo.itemdisplayprice AS s2 WITH (nolock) ON s1.ItemID = s2.ItemID AND s1.ID > s2.ID AND s2.PriceType = 1
                            WHERE      (s2.ItemID IS NULL) AND (s1.PriceType = 1) AND (s1.MinNumber = 1) AND (s1.MaxNumber = 1)) AS IDP ON I.ID = IDP.ItemID
WHERE     (GETDATE() BETWEEN I.DateStart AND I.DateEnd) AND (I.Status = 0) AND (I.ShowOrder >= 0) AND (IDP.PriceType = 1 OR
                      IDP.PriceType IS NULL) AND (IDP.MinNumber = 1 OR
                      IDP.MinNumber IS NULL) AND (IDP.MaxNumber = 1 OR
                      IDP.MaxNumber IS NULL) OR
                      (GETDATE() < I.DateEnd) AND (I.Status = 60) AND (I.ShowOrder >= 0) AND (IDP.PriceType = 1 OR
                      IDP.PriceType IS NULL) AND (IDP.MinNumber = 1 OR
                      IDP.MinNumber IS NULL) AND (IDP.MaxNumber = 1 OR
                      IDP.MaxNumber IS NULL)

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[20] 2[23] 3) )"
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
         Begin Table = "I"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 194
            End
            DisplayFlags = 280
            TopColumn = 58
         End
         Begin Table = "C"
            Begin Extent = 
               Top = 6
               Left = 232
               Bottom = 114
               Right = 407
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "P"
            Begin Extent = 
               Top = 114
               Left = 38
               Bottom = 222
               Right = 220
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "J"
            Begin Extent = 
               Top = 114
               Left = 258
               Bottom = 222
               Right = 409
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "M"
            Begin Extent = 
               Top = 222
               Left = 38
               Bottom = 330
               Right = 189
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "S"
            Begin Extent = 
               Top = 6
               Left = 445
               Bottom = 114
               Right = 598
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "IDP"
            Begin Extent = 
               Top = 6
               Left = 636
               Bottom = 114
               Right = 801
            End
            DisplayFlags = 280
            TopColumn = 0
         End
 ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ViewItemSearch'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'     End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 29
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
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ViewItemSearch'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ViewItemSearch'
GO
