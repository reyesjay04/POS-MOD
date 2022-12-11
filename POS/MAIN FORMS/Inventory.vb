﻿Option Explicit On
Imports MySql.Data.MySqlClient
Imports System.Drawing.Printing
Public Class Inventory
    Private Shared _instance As Inventory
    Public ReadOnly Property Instance As Inventory
        Get
            Return _instance
        End Get
    End Property
    Dim boolinventory As Boolean = False
    Dim prodid As String
    Dim tbl As String
    Dim flds As String

    Property InventoryList As New List(Of InventoryCls)

    Private Sub Inventory_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _instance = Me
        Try
            TabControl1.TabPages(0).Text = "Stock Inventory"
            TabControl1.TabPages(1).Text = "Critical Stock"
            TabControl1.TabPages(2).Text = "Fast Moving Stock"
            TabControl1.TabPages(3).Text = "Stock Adjustment"
            TabControl1.TabPages(4).Text = "Stock in (Receiving) Entry"
            TabControl2.TabPages(0).Text = "Product Ingredients(Server)"
            TabControl2.TabPages(1).Text = "Product Ingredients(Local)"
            TabControl5.TabPages(0).Text = "Approved"
            TabControl5.TabPages(1).Text = "Waiting for approval"

            With DataGridViewINVVIEW
                .Rows.Clear()
                .RowHeadersVisible = False
                .AllowUserToAddRows = False
                .AllowUserToDeleteRows = False
                .AllowUserToOrderColumns = False
                .AllowUserToResizeColumns = False
                .AllowUserToResizeRows = False
                .Font = New Font("tahoma", 10)
                .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
                .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
                .SelectionMode = DataGridViewSelectionMode.FullRowSelect
                .Columns(0).Width = 150
                .Columns(7).Width = 170
            End With

            GetInventory() 'Class Type

            loadcriticalstocks()
            loadstockadjustmentreport(False)
            loadfastmovingstock()
            loadstockentry(False)
            FillDatagridZreadInv(False)
            loadinventorycustom()
            loadinventorycustomdisapp()

            If ClientRole = "Crew" Then
                TabControl1.TabPages.Remove(TabControl1.TabPages(3))
                ButtonResetInventory.Enabled = False
                ButtonResetInventory.Visible = False
                GroupBoxStockIN.Visible = False
                GroupBoxStockAdjustment.Visible = False
            End If

            If ClientRole = "Head Crew" Then
                TabControl1.TabPages.Remove(TabControl1.TabPages(3))
                ButtonResetInventory.Enabled = False
                ButtonResetInventory.Visible = False
                GroupBoxStockAdjustment.Visible = False
            End If
            LabelDate.Text = "Starting inventory of [ " & Format(DateTimePickerZXreading.Value, "MMMM dd, yyyy") & " ]"
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "Inventory/Load: " & ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub dgv_EmployeeTraining_RowPostPaint(sender As Object, e As DataGridViewRowPostPaintEventArgs) Handles DataGridViewINVVIEW.RowPostPaint
        If e.RowIndex < Me.DataGridViewINVVIEW.RowCount - 1 Then
            Try
                Dim dgvRow As DataGridViewRow = Me.DataGridViewINVVIEW.Rows(e.RowIndex)
                If dgvRow.Cells("colPrimary").Value <= dgvRow.Cells("colLimit").Value Then
                    dgvRow.DefaultCellStyle.BackColor = Color.FromArgb(255, 213, 128)
                End If
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        End If
    End Sub

    Public Sub GetInventory()
        Try
            DataGridViewINVVIEW.Rows.Clear()
            InventoryList = ModInventory.loadinventory()
            For Each row As InventoryCls In InventoryList
                With row
                    DataGridViewINVVIEW.Rows.Add(.Ingredientname, .SKU, .PrimaryValue, .UnitOfMeasure, .Servings, If(.Status = 1, "Active", "In-active"), .Limit, .DateCreated)
                End With
            Next
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "Inventory/GetInventory(): " & ex.ToString, "Critical")
        End Try
    End Sub
    Sub loadinventorycustom()
        Try
            fields = "I.product_ingredients as Ingredients, CONCAT_WS(' ', ROUND(I.stock_primary,0), F.primary_unit) as PrimaryValue , CONCAT_WS(' ', I.stock_secondary, F.secondary_unit) as UOM , ROUND(I.stock_no_of_servings,0) as NoofServings, I.stock_status, I.critical_limit, I.date_modified"
            GLOBAL_SELECT_ALL_FUNCTION_WHERE(table:="loc_pos_inventory I INNER JOIN loc_product_formula F ON F.formula_id = I.inventory_id ", datagrid:=DataGridViewCustomInvApproved, fields:=fields, where:="F.origin = 'Local' AND I.stock_status = 1 AND I.store_id = " & ClientStoreID)
            With DataGridViewCustomInvApproved
                .Columns(3).HeaderCell.Value = "No. of Servings"
                .Columns(4).HeaderCell.Value = "Status"
                .Columns(5).HeaderCell.Value = "Critical Limit"
                .Columns(6).HeaderCell.Value = "Date Modified"
            End With
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "Inventory/loadinventorycustom(): " & ex.ToString, "Critical")
        End Try
    End Sub
    Sub loadinventorycustomdisapp()
        Try
            fields = "I.product_ingredients as Ingredients, CONCAT_WS(' ', ROUND(I.stock_primary,0), F.primary_unit) as PrimaryValue , CONCAT_WS(' ', I.stock_secondary, F.secondary_unit) as UOM , ROUND(I.stock_no_of_servings,0) as NoofServings, I.stock_status, I.critical_limit, I.date_modified"
            GLOBAL_SELECT_ALL_FUNCTION_WHERE(table:="loc_pos_inventory I INNER JOIN loc_product_formula F ON F.formula_id = I.formula_id ", datagrid:=DataGridViewCustomDisapp, fields:=fields, where:="F.origin = 'Local' AND I.stock_status = 0 AND I.store_id = " & ClientStoreID)
            With DataGridViewCustomDisapp
                .Columns(3).HeaderCell.Value = "No. of Servings"
                .Columns(4).HeaderCell.Value = "Status"
                .Columns(5).HeaderCell.Value = "Critical Limit"
                .Columns(6).HeaderCell.Value = "Date Modified"
            End With
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "Inventory/loadinventorycustomdisapp(): " & ex.ToString, "Critical")
        End Try
    End Sub
    Public Sub loadcriticalstocks()
        Try
            fields = "`product_ingredients`, ROUND(`stock_primary`, 0),ROUND(`stock_secondary`, 0) , `critical_limit`, `date_modified`"
            GLOBAL_SELECT_ALL_FUNCTION_WHERE(table:="loc_pos_inventory", datagrid:=DataGridViewCriticalStocks, fields:=fields, where:=" stock_status = 1 AND critical_limit >= stock_primary AND store_id = " & ClientStoreID & "  ORDER BY product_ingredients ASC")
            With DataGridViewCriticalStocks
                .Columns(0).HeaderCell.Value = "Product Name"
                .Columns(1).HeaderCell.Value = "Primary Value"
                .Columns(2).HeaderCell.Value = "Secondary Value"
                .Columns(3).HeaderCell.Value = "Critical Limit"
                .Columns(4).HeaderCell.Value = "Date Modified"
            End With
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "Inventory/loadcriticalstocks(): " & ex.ToString, "Critical")
        End Try
    End Sub
    Sub loadfastmovingstock()
        Try
            Dim fields As String = "formula_id, SUM(stock_primary) as StockP"
            Dim FastMovingDatatable = AsDatatable("loc_fm_stock GROUP by formula_id ORDER BY SUM(stock_primary) DESC", fields, DataGridViewFASTMOVING)
            For Each row As DataRow In FastMovingDatatable.Rows
                Dim RetFormula = GLOBAL_SELECT_FUNCTION_RETURN("loc_product_formula", "product_ingredients", "formula_id ='" & row("formula_id") & "'", "product_ingredients")

                DataGridViewFASTMOVING.Rows.Add(RetFormula, row("StockP"))
            Next
            With DataGridViewFASTMOVING
                .Columns(0).HeaderCell.Value = "Product Ingredients"
                .Columns(1).HeaderCell.Value = "Total Stock Quantity"
            End With
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "Inventory/loadfastmovingstock(): " & ex.ToString, "Critical")
        End Try
    End Sub

    Dim DataTableInventory As New DataTable
    Dim DataTableFormula As New DataTable
    Dim inv
    Public Sub loadstockentry(bool As Boolean)
        Try
            Dim Fields = "`created_at`, `group_name`, `severity`, `crew_id`, `description`"
            Dim Table = "loc_audit_trail"
            Dim WhereVal As String = ""
            Dim LoadStockEntry
            If bool = False Then
                WhereVal = " WHERE severity = 'Normal' AND description LIKE '%Stock Entry:%' AND date(created_at) = CURRENT_DATE() "
                LoadStockEntry = AsDatatable(Table & WhereVal, Fields, DataGridViewSTOCKENTRY)
            Else
                WhereVal = " WHERE severity = 'Normal' AND description LIKE '%Stock Entry:%' AND date(created_at) BETWEEN '" & Format(DateTimePicker4.Value, "yyyy-MM-dd") & "' AND '" & Format(DateTimePicker3.Value, "yyyy-MM-dd") & "'"
                LoadStockEntry = AsDatatable(Table & WhereVal, Fields, DataGridViewSTOCKENTRY)
            End If

            With DataGridViewSTOCKENTRY
                For Each row As DataRow In LoadStockEntry.Rows
                    Dim CrewID = returnfullname(row("crew_id"))
                    DataGridViewSTOCKENTRY.Rows.Add(CrewID, row("group_name"), row("severity"), row("description"), row("created_at"))
                Next
                .Columns(0).HeaderText = "Service Crew"
                .Columns(1).HeaderText = "Group Name"
                .Columns(2).HeaderText = "Severity"
                .Columns(3).HeaderText = "Description"
                .Columns(4).HeaderText = "Date and Time"
                .Columns(0).Width = 120
                .Columns(1).Width = 120
                .Columns(2).Width = 120
                .Columns(4).Width = 170
            End With
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "Inventory/loadstockentry(): " & ex.ToString, "Critical")
        End Try
    End Sub
    Dim inventoryid
    Sub loadstockadjustmentreport(searchdate As Boolean)
        Try
            Dim StockAdjustmentReport As DataTable = New DataTable
            Dim Fields = "`created_at`, `group_name`, `severity`, `crew_id`, `description`"
            Dim Table = "loc_audit_trail"
            Dim Where = ""
            If searchdate = False Then
                Where = " WHERE severity = 'Normal' AND date(created_at) = CURRENT_DATE() AND description LIKE '%Stock Adjustment:%'"
                StockAdjustmentReport = AsDatatable(Table & Where, Fields, DataGridViewStockAdjustment)
            Else
                Where = " WHERE severity = 'Normal' AND description LIKE '%Stock Adjustment:%' AND date(created_at) BETWEEN '" & Format(DateTimePicker1.Value, "yyyy-MM-dd") & "' AND '" & Format(DateTimePicker2.Value, "yyyy-MM-dd") & "'"
                StockAdjustmentReport = AsDatatable(Table & Where, Fields, DataGridViewStockAdjustment)
            End If
            With DataGridViewStockAdjustment
                .Columns(0).HeaderText = "Service Crew"
                .Columns(1).HeaderText = "Group Name"
                .Columns(2).HeaderText = "Severity"
                .Columns(3).HeaderText = "Description"
                .Columns(4).HeaderText = "Date and Time"
                .Columns(0).Width = 120
                .Columns(1).Width = 120
                .Columns(2).Width = 120
                .Columns(4).Width = 170
                For Each row As DataRow In StockAdjustmentReport.Rows
                    Dim CrewID = returnfullname(row("crew_id"))
                    DataGridViewStockAdjustment.Rows.Add(CrewID, row("group_name"), row("severity"), row("description"), row("created_at"))
                Next
            End With
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "Inventory/loadstockadjustmentreport(): " & ex.ToString, "Critical")
        End Try
    End Sub
    Dim totalqty As Integer
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles ButtonSearchStockAdj.Click
        Try
            If DateTimePicker1.Value.Date > DateTimePicker2.Value.Date Then

            Else
                loadstockadjustmentreport(True)
            End If
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "Inventory/ButtonSearchStockAdj: " & ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub ButtonResetInventory_Click(sender As Object, e As EventArgs) Handles ButtonResetInventory.Click
        Try
            Dim msg = MessageBox.Show("Are you sure you want to reset the inventory ? Press Yes to continue or No to cancel", "NOTICE", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
            If msg = DialogResult.Yes Then
                Dim sql = "UPDATE loc_pos_inventory SET stock_primary = 0, stock_secondary = 0,  stock_no_of_servings = 0"
                Dim cmd As MySqlCommand = New MySqlCommand(sql, LocalhostConn)
                cmd.ExecuteNonQuery()
                GetInventory()
                AuditTrail.LogToAuditTrail("Reset", "Inventory Reset ", "Normal")
            End If
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "Inventory/ButtonResetInventory: " & ex.ToString, "Critical")
        End Try
    End Sub
    Private WithEvents printdoc As PrintDocument = New PrintDocument
    Private PrintPreviewDialog1 As New PrintPreviewDialog

    Private Sub ButtonPrintCurInv_Click(sender As Object, e As EventArgs) Handles ButtonPrintCurInv.Click
        Dim b = 0
        Try

            b = 0
            For i As Integer = 0 To DataGridViewINVVIEW.Rows.Count - 1 Step +1
                b += 10
            Next

            EJournal.UseWriter = False
            EJournal.PosWriter = False

            printdoc.DefaultPageSettings.PaperSize = New PaperSize("Custom", ReturnPrintSize(), 200 + b)
            PrintPreviewDialog1.Document = printdoc
            PrintPreviewDialog1.ShowDialog()
            AuditTrail.LogToAuditTrail("Report", "Inventory: Generated Report, " & ClientCrewID, "Normal")

        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "Inventory/ButtonPrintCurInv: " & ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub pdoc_PrintPage(sender As Object, e As Printing.PrintPageEventArgs) Handles printdoc.PrintPage
        Try
            Dim FontDefault As New Font("Tahoma", 5)
            Dim FontDefaultBold As New Font("Tahoma", 5, FontStyle.Bold)
            Dim FontDefaultLine As New Font("Tahoma", 5)
            Dim BodySpacing As Integer = 0
            If My.Settings.PrintSize = "57mm" Then
                FontDefaultLine = New Font("Tahoma", 6)
                FontDefault = New Font("Tahoma", 5)
                FontDefaultBold = New Font("Tahoma", 5, FontStyle.Bold)
            Else

                FontDefault = New Font("Tahoma", 6)
                FontDefaultBold = New Font("Tahoma", 6, FontStyle.Bold)
                FontDefaultLine = New Font("Tahoma", 7)
                BodySpacing = 20
            End If
            RECEIPTLINECOUNT = 0
            ReceiptHeaderOne(sender, e, False, "", False, False)
            RECEIPTLINECOUNT += 10
            SimpleTextDisplay(sender, e, "INGREDIENTS", FontDefaultBold, 0, RECEIPTLINECOUNT)
            SimpleTextDisplay(sender, e, "PRIMARY", FontDefaultBold, 70 + BodySpacing, RECEIPTLINECOUNT)
            SimpleTextDisplay(sender, e, "SERVINGS", FontDefaultBold, 140 + BodySpacing, RECEIPTLINECOUNT)
            RECEIPTLINECOUNT += 10

            For i As Integer = 0 To DataGridViewINVVIEW.Rows.Count - 1 Step +1
                SimpleTextDisplay(sender, e, DataGridViewINVVIEW.Rows(i).Cells(1).Value, FontDefault, 0, RECEIPTLINECOUNT)
                SimpleTextDisplay(sender, e, DataGridViewINVVIEW.Rows(i).Cells(2).Value, FontDefault, 70 + BodySpacing, RECEIPTLINECOUNT)
                SimpleTextDisplay(sender, e, DataGridViewINVVIEW.Rows(i).Cells(3).Value, FontDefault, 140 + BodySpacing, RECEIPTLINECOUNT)
                RECEIPTLINECOUNT += 10
            Next

            RECEIPTLINECOUNT += 10
            PrintStars(sender, e, FontDefault, RECEIPTLINECOUNT)

            RECEIPTLINECOUNT += 30
            CenterTextDisplay(sender, e, Format(Now(), "yyyy-MM-dd HH:mm:ss"), FontDefault, RECEIPTLINECOUNT)
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "Inventory/printdoc: " & ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub ButtonKeyboard_Click(sender As Object, e As EventArgs) Handles ButtonKeyboard.Click
        ShowKeyboard()
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles ButtonStockAdjustment.Click
        Enabled = False
        StockAdjustment.Show()
    End Sub

    Private Sub Inventory_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Application.OpenForms().OfType(Of PanelReasonCat).Any Then
            PanelReasonCat.Close()
        End If
        If Application.OpenForms().OfType(Of StockAdjustment).Any Then
            StockAdjustment.Close()
        End If
        If Application.OpenForms().OfType(Of NewStockEntry).Any Then
            NewStockEntry.Close()
        End If
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles ButtonStockIN.Click
        NewStockEntry.Show()
        Enabled = False
    End Sub

    Private Sub ButtonSearchDailyTransaction_Click(sender As Object, e As EventArgs) Handles ButtonSearchDailyTransaction.Click
        If Not DateTimePicker4.Value.Date > DateTimePicker3.Value.Date Then
            loadstockentry(True)
        End If
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
        Try

            If TabControl1.SelectedIndex = 0 Then
                GetInventory()
            End If

        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "Inventory/TabControl1: " & ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub FillDatagridZreadInv(searchdate As Boolean)
        Try
            table = "loc_zread_inventory I INNER JOIN loc_product_formula F ON F.server_formula_id = I.server_inventory_id "
            fields = "I.product_ingredients as Ingredients, i.sku , CONCAT_WS(' ', ROUND(I.stock_primary,0), F.primary_unit) as PrimaryValue , CONCAT_WS(' ', ROUND(I.stock_secondary,0), F.secondary_unit) as UOM , ROUND(I.stock_no_of_servings,0) as NoofServings, I.zreading"
            If searchdate = False Then
                where = "zreading = '" & Format(Now(), "yyyy-MM-dd") & "' AND I.stock_status = 1 AND I.store_id = " & ClientStoreID & " ORDER BY I.product_ingredients ASC"
                GLOBAL_SELECT_ALL_FUNCTION_WHERE(table:=table, datagrid:=DataGridViewZreadInvData, fields:=fields, where:=where)
            Else
                where = "zreading = '" & Format(DateTimePickerZXreading.Value, "yyyy-MM-dd") & "' AND I.stock_status = 1 AND I.store_id = " & ClientStoreID & " ORDER BY I.product_ingredients ASC"
                GLOBAL_SELECT_ALL_FUNCTION_WHERE(table:=table, datagrid:=DataGridViewZreadInvData, fields:=fields, where:=where)
            End If
            With DataGridViewZreadInvData
                .Columns(0).HeaderText = "Ingredients"
                .Columns(1).HeaderText = "SKU"
                .Columns(2).HeaderText = "Primary Value"
                .Columns(3).HeaderText = "UOM"
                .Columns(4).HeaderText = "No. of Servings"
                .Columns(5).HeaderText = "Zreading Date"
            End With
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "Inventory/FillDatagridZreadInv(): " & ex.ToString, "Critical")
        End Try
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles ButtonSearchStarting.Click
        LabelDate.Text = "Starting inventory of [ " & Format(DateTimePickerZXreading.Value, "MMMM dd, yyyy") & " ]"
        FillDatagridZreadInv(True)
    End Sub

End Class