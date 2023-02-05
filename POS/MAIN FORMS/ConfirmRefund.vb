﻿Imports System.Drawing.Printing
Imports System.Text
Imports System.Xml
Imports MySql.Data.MySqlClient

Public Class ConfirmRefund

    Property TRANSACTIONNUMBER As String
    Property REFUNDTOTAL As Double = 0

    Private WithEvents printdoc As PrintDocument = New PrintDocument
    Private PrintPreviewDialog As New PrintPreviewDialog

    Private Sub ConfirmRefund_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        SettingsForm.Enabled = True
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles ButtonSubmit.Click
        Try
            If CheckColumnIfExist("user_code", "loc_users WHERE user_code = '" & Trim(TextBox1.Text) & "' AND user_level IN ('Manager', 'Admin', 'Head Crew')") Then
                Button3.PerformClick()
            Else
                MessageBox.Show("User code does not exist", "NOTICE", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "Confirm Refund/ButtonSubmit: " & ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub DisableCouponTotal(TRANSACTIONNUMBER)
        Try
            Dim ConnectionLocal As MySqlConnection = LocalhostConn()
            Dim Query As String = "UPDATE loc_coupon_data SET status = 0 WHERE transaction_number = " & TRANSACTIONNUMBER
            Dim Command As MySqlCommand = New MySqlCommand(Query, ConnectionLocal)
            Command.ExecuteNonQuery()
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "Confirm Refund/DisableCouponTotal(): " & ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub PrintDocument_PrintPage(sender As Object, e As PrintPageEventArgs) Handles printdoc.PrintPage
        Try

            Dim FontDefault As Font
            Dim AddLine As Integer = 20
            Dim CategorySpacing As Integer = 20
            If My.Settings.PrintSize = "57mm" Then
                FontDefault = New Font("Tahoma", 6)
            Else
                FontDefault = New Font("Tahoma", 7)
            End If

            If My.Settings.PrintSize = "80mm" Then
                CategorySpacing = 50
            End If

            ReceiptHeaderOne(sender, e, True, "", False, True)
            ReceiptBody(sender, e, True, TRANSACTIONNUMBER, False)

            If SettingsForm.DataGridViewITEMRETURN1.SelectedRows(0).Cells(2).Value > 0 Then
                ReceiptBodyFooter(sender, e, True, TRANSACTIONNUMBER, True, True)
            Else
                ReceiptBodyFooter(sender, e, True, TRANSACTIONNUMBER, True, False)
            End If

            ReceiptFooterOne(sender, e, True, False)

        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "Confirm Refund/printdoc(): " & ex.ToString, "Critical")
        End Try
    End Sub

    Private Sub ButtonKeyboard_Click(sender As Object, e As EventArgs) Handles ButtonKeyboard.Click
        ShowKeyboard()
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim isSuccess As Boolean = True
        Dim XMLName As String = ""
        Try

            Dim ConnectionLocal As MySqlConnection = LocalhostConn()
            Dim Query As String = "SELECT * FROM loc_daily_transaction_details WHERE transaction_number = '" & TRANSACTIONNUMBER & "'"
            Dim Command As MySqlCommand = New MySqlCommand(Query, ConnectionLocal)
            Dim Da As MySqlDataAdapter = New MySqlDataAdapter(Command)
            Dim Dt As DataTable = New DataTable
            Da.Fill(Dt)

            Dim TotalLines As Integer = 0
            Dim BodyLine As Integer = 560
            Dim CountHeaderLine As Integer = count("id", "loc_receipt WHERE type = 'Header' AND status = 'Y'")
            Dim ProductLine As Integer = 0
            Dim CountFooterLine As Integer = count("id", "loc_receipt WHERE type = 'Footer' AND status = 'Y'")

            CountHeaderLine *= 10
            CountFooterLine *= 10

            For i As Integer = 0 To Dt.Rows.Count - 1 Step +1
                ProductLine += 10
                If Dt(i)(17) > 0 Then
                    ProductLine += 10
                End If
            Next

            TotalLines = CountHeaderLine + ProductLine + CountFooterLine + BodyLine
            printdoc.DefaultPageSettings.PaperSize = New PaperSize("Custom", ReturnPrintSize(), TotalLines)

            XMLName = "R" & TRANSACTIONNUMBER & FullDateFormatForSaving().ToString & ".xml"
            XML_Writer = New XmlTextWriter(XML_Path & XMLName, Encoding.UTF8)
            XML_Writer.WriteStartDocument(True)
            XML_Writer.Formatting = Formatting.Indented
            XML_Writer.Indentation = 2
            XML_Writer.WriteStartElement("Table")

            If S_Print_Returns = "YES" Then
                printdoc.Print()
            Else
                PrintPreviewDialog.Document = printdoc
                PrintPreviewDialog.ShowDialog()
            End If

            XML_Writer.WriteEndElement()
            XML_Writer.WriteEndDocument()
            XML_Writer.Close()



            AuditTrail.LogToAuditTrail("Refund", "Refund: " & TRANSACTIONNUMBER, "Normal")
            Dim UserCodeID = returnselect("user_id", "loc_users WHERE user_code = '" & Trim(TextBox1.Text) & "'")
            SettingsForm.INSERTRETURNS(TRANSACTIONNUMBER)
            AuditTrail.LogToAuditTrail("Refund", "TRN. ID: " & TRANSACTIONNUMBER & ", Code: " & UserCodeID & ", Total: " & REFUNDTOTAL, "Normal")



            UpdateInventory(True)
            DisableCouponTotal(TRANSACTIONNUMBER)
            Close()

        Catch ex As Exception
            isSuccess = False
            AuditTrail.LogToAuditTrail("System", "Confirm Refund/Button3: " & ex.ToString, "Critical")
        Finally
            If isSuccess Then
                SaveXMLInfo(XMLName)
            End If
        End Try
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Close()
    End Sub

    Private Sub ConfirmRefund_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadProducts(TRANSACTIONNUMBER)
    End Sub

    Private Sub LoadProducts(TransactionNumber)
        Try
            Dim ConnectionLocal As MySqlConnection = LocalhostConn()
            Dim Query As String = "SELECT product_id, quantity FROM loc_daily_transaction_details WHERE transaction_number = '" & TransactionNumber & "'"
            Dim Command As MySqlCommand = New MySqlCommand(Query, ConnectionLocal)
            Dim Da As MySqlDataAdapter = New MySqlDataAdapter(Command)
            Dim dt As DataTable = New DataTable
            Da.Fill(dt)
            For i As Integer = 0 To dt.Rows.Count - 1 Step +1
                Query = "SELECT product_name, formula_id, half_batch FROM loc_admin_products WHERE product_id = " & dt(i)(0) & ""
                Command = New MySqlCommand(Query, ConnectionLocal)
                Using reader As MySqlDataReader = Command.ExecuteReader
                    If reader.HasRows Then
                        While reader.Read
                            DataGridView1.Rows.Add(reader("product_name"), reader("formula_id"), dt(i)(1), reader("half_batch"))
                        End While
                    End If
                End Using
            Next
            Dim IncrementID As Integer = 1
            For i As Integer = 0 To DataGridView1.Rows.Count - 1 Step +1
                Dim FormulaID As String = DataGridView1.Rows(i).Cells(1).Value
                Dim ProductName As String = DataGridView1.Rows(i).Cells(0).Value
                Dim HalfBatch As Integer = DataGridView1.Rows(i).Cells(3).Value
                Dim words As String() = FormulaID.Split(New Char() {","c})
                Dim word As String
                For Each word In words
                    Dim Quantity As Integer = DataGridView1.Rows(i).Cells(2).Value
                    Dim TotalServings As Double = 0
                    Dim TotalUnitCost As Double = 0
                    Query = "SELECT serving_value, unit_cost, origin FROM loc_product_formula WHERE formula_id = " & word & ""
                    Command = New MySqlCommand(Query, ConnectionLocal)
                    Using reader As MySqlDataReader = Command.ExecuteReader
                        If reader.HasRows Then
                            While reader.Read
                                TotalServings = Quantity * reader("serving_value")
                                TotalUnitCost = Quantity * reader("unit_cost")
                                DataGridViewInv.Rows.Add(TotalServings, word, Quantity, IncrementID, ProductName, reader("serving_value"), TotalUnitCost, reader("unit_cost"), "", reader("origin"), HalfBatch)
                            End While
                        End If
                    End Using

                Next
                IncrementID += 1

            Next
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "Confirm Refund/LoadProducts(): " & ex.ToString, "Critical")
        End Try
    End Sub

    Private Sub TextBox1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox1.KeyPress
        Try
            If InStr(DisallowedCharacters, e.KeyChar) > 0 Then
                e.Handled = True
            End If
        Catch ex As Exception

        End Try
    End Sub
End Class