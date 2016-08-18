USE [TWBACKENDDB]
GO
/****** Object:  View [dbo].[View_退貨單]    Script Date: 2016/08/18 13:04:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[View_退貨單]
AS
SELECT     ID AS 退貨單序號, Code AS 退貨單號, SupplierID AS SellerID, ProductID AS 商品序號, RetgoodType AS 退貨配達方式, Date AS 建檔日期, RetgoodUser AS 建立人, 
                      FinalDate AS 退貨完成日, Status AS 退貨狀態, Price AS 退款金額, Qty AS 退貨數量, Cause AS 退貨原因, CauseNote AS 退貨原因描述, 
                      StockOutItemID AS 出庫子單序號, BankName AS 銀行代碼_名稱, BankBranch AS 銀行分行名稱, AccountNO AS 退款帳號, AccountName AS 退款帳號名稱, 
                      ProcessID AS 定單子單編號, FrmName AS 退貨人名稱, FrmLocation AS 退貨人城市, FrmADDR AS 退貨人地址, FrmZipcode AS 退貨人郵遞區號, 
                      FrmPhone AS 退貨人聯絡電話, FrmMobile AS 退貨人聯絡手機, FrmEmail AS 退貨人Email, ShpCode AS 退貨託運編號, UpdatedDate AS 最後更新日期, 
                      UpdatedUser AS 最後更新人, OnReturnDate AS 退貨開始處理日期, FinReturnDate AS 退貨結案日期, ABNReturndate AS 退貨異常日期, 
                      CancelReturnDate AS 退貨取消日期, Note AS 退貨詳細步驟, Declined AS 是否拒收, ASNNumber AS ASN編號, SendStatus AS 是否上傳WMS, 
                      SendDate AS 上傳WMS日期, ProductStatus AS 商品狀態, ToSAP AS 是否進SAP, CartID AS 訂購主單編號, ChangeSalesOrderCode AS 換單編號
FROM         dbo.retgood

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[20] 2[21] 3) )"
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
         Begin Table = "retgood"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 341
               Right = 242
            End
            DisplayFlags = 280
            TopColumn = 69
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
         Alias = 3255
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_退貨單'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_退貨單'
GO
