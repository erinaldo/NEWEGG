USE [TWBACKENDDB]
GO
/****** Object:  View [dbo].[View_後台購物車主單]    Script Date: 2016/08/18 13:04:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[View_後台購物車主單]
AS
SELECT     ID AS 訂單編號, Price AS 金額, Qty AS 數量, Zipcode AS 收件人郵遞區號, ADDR AS 收件人地址, Phone AS 訂購人日間電話, Phone2 AS 訂購人夜間電話, 
                      Mobile AS 訂購人手機, Email AS 訂購人EMail, Receiver AS 訂購人, IinvoiceTitle AS 發票抬頭, InvoiceNO AS 發票號碼, StoreID AS 賣場代號, PayType AS 付款方式, 
                      UsrIP AS 購買人IP, Username AS 購買人姓名, CardPhone AS 付款人日間電話, CardPhone2 AS 付款人夜間電話, CardMobile AS 付款人手機, ShipType AS 送貨方式, 
                      ShipPrice AS 運費, Status AS 訂單狀態, Location AS 收件人所在縣市, HpType AS 分期付款期數, InvoLocation AS 發票縣市, InvoZipcode AS 發票郵遞區號, 
                      InvoADDR AS 發票地址, InvoReceiver AS 發票收件人, UsrLOC AS 購買人縣市, UserZipcode AS 購買人郵遞區號, UsrADDR AS 購買人地址, CMPName AS 公司名稱, 
                      CMPLOC AS 公司縣市, CMOZipcode AS 公司郵遞區號, CMPADDR AS 公司地址, CntName AS 聯絡人姓名, CnRelation AS 聯絡人關係, CntTelCMP AS 聯絡人公司電話, 
                      CntTelHome AS 聯絡人家電話, CntMobile AS 聯絡人手機, StID AS 店配門市編號, StName AS 店配門市名稱, StPhone AS 店配門市電話, StADDR AS 店配門市地址, 
                      StatusDate AS 狀態修改日期, CreateDate AS 實際建檔日期, RecvMobile AS 收件人行動電話, ActCode AS 公司統編, DelvStatus AS 出貨狀態, 
                      DelvStatusDate AS 配達時間, PayTypeID AS 付款金流
FROM         dbo.cart

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[20] 4[66] 2[3] 3) )"
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
         Begin Table = "cart"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 468
               Right = 283
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
         Alias = 3360
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_後台購物車主單'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_後台購物車主單'
GO
