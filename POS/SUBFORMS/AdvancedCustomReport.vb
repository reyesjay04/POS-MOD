Imports MySql.Data.MySqlClient
Imports PdfSharp.Drawing
Imports PdfSharp.Pdf

Public Class AdvancedCustomReport
    Private Sub AdvancedCustomReport_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        MDIFORM.newMDIchildReports.Enabled = True
    End Sub
    Private Sub AdvancedCustomReport_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TopMost = True
        SelectDisctinctDaily()
        LoadCouponTypes()
        With DataGridViewCustomReport
            .Font = New Font("tahoma", 10)
            .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        End With
        ToolStripComboBoxTaxType.SelectedIndex = 0
        ToolStripComboBox1.SelectedIndex = 0
    End Sub
    Private Sub SelectDisctinctDaily()
        Try
            Dim ConnectionLocal As MySqlConnection = LocalhostConn()
            Dim sql = "Select DISTINCT transaction_type FROM loc_daily_transaction ORDER BY transaction_type ASC"
            Dim cmd As MySqlCommand = New MySqlCommand(sql, ConnectionLocal)
            Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
            Dim dt As DataTable = New DataTable
            da.Fill(dt)

            ToolStripComboBoxTransactionType.Items.Clear()
            ToolStripComboBoxTransactionType.Items.Add("All")
            ToolStripComboBoxTransactionType.Items.Add("All(Cash)")
            ToolStripComboBoxTransactionType.Items.Add("All(Others)")

            For i As Integer = 0 To dt.Rows.Count - 1 Step +1
                ToolStripComboBoxTransactionType.Items.Add(dt(i)(0))
            Next

            ToolStripComboBoxTransactionType.SelectedIndex = 0
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "ADVCustRep/SelectDisctinctDaily(): " & ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub LoadCouponTypes()
        Try
            Dim ConnectionLocal As MySqlConnection = LocalhostConn()
            Dim sql = "Select Couponname_ FROM tbcoupon ORDER BY type ASC"
            Dim cmd As MySqlCommand = New MySqlCommand(sql, ConnectionLocal)
            Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
            Dim dt As DataTable = New DataTable
            da.Fill(dt)
            ToolStripComboBoxDiscType.Items.Clear()
            ToolStripComboBoxDiscType.Items.Add("All")
            For i As Integer = 0 To dt.Rows.Count - 1 Step +1
                ToolStripComboBoxDiscType.Items.Add(dt(i)(0))
            Next
            ToolStripComboBoxDiscType.SelectedIndex = 0
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "ADVCustRep/LoadCouponTypes(): " & ex.ToString, "Critical")
        End Try
    End Sub

    Property CustomReportLessVat As Double = 0
    Property CustomReportVat As Double = 0
    Property CustomReportdt As DataTable
    Property GrossSales As Double = 0
    Property VatableSales As Double = 0
    Property LessVat As Double = 0
    Property AddVat As Double = 0
    Property VatExempt As Double = 0
    Property TotalDisc As Double = 0
    Property NetSales As Double = 0
    Property GrossRefund As Double = 0
    Property VatPercentage As Double = 0

    Private Sub CustomReport(TaxType, TransactionType, DiscountType)
        Try
            Dim WhereExtension As String = " GROUP BY LD.transaction_number ORDER BY LD.created_at ASC"
            Dim ActiveQuery As String = ""
            Dim FieldsNormal As String = "LD.transaction_number As transaction_number, LD.grosssales as grosssales, LD.vatablesales as vatablesales, LD.vatpercentage as vatpercentage, LD.lessvat as lessvat, LD.vatexemptsales as vatexemptsales, SUM(LC.coupon_total) as totaldiscount, LD.transaction_type as transaction_type, LD.amountdue as amountdue, SUM(LC.gc_value) as gc_value , LD.created_at as dateCreated, LD.vatpercentage as AddVat, LD.Active as Status"
            Dim LeftJointNormal As String = " LEFT JOIN loc_coupon_data LC ON LD.transaction_number = LC.transaction_number "
            Dim AddWhere As String = " LC.coupon_type = 'Fix-1'"

            If ToolStripComboBox1.Text = "All" Then
                ActiveQuery = ""
            ElseIf ToolStripComboBox1.Text = "Complete" Then
                'WhereExtension = ""
                If TransactionType = "All" Then
                    If DiscountType = "All" Then
                        ActiveQuery = " AND LD.active IN (1,3) "
                    End If
                ElseIf TransactionType = "All(Cash)" Then
                    If DiscountType = "All" Then
                        ActiveQuery = " AND LD.active IN (1,3) "
                    End If
                ElseIf TransactionType = "All(Others)" Then
                    If DiscountType = "All" Then
                        ActiveQuery = " AND LD.active IN (1,3) "
                    End If
                Else
                    If DiscountType = "All" Then
                        ActiveQuery = " AND LD.active IN (1,3) "
                    End If
                End If
            Else
                If TransactionType = "All" Then
                    If DiscountType = "All" Then
                        ActiveQuery = " AND LD.active = 2"
                    End If
                ElseIf TransactionType = "All(Cash)" Then
                    If DiscountType = "All" Then
                        ActiveQuery = " AND LD.active = 2"
                    End If
                ElseIf TransactionType = "All(Others)" Then
                    If DiscountType = "All" Then
                        ActiveQuery = " AND LD.active = 2"
                    End If
                Else
                    If DiscountType = "All" Then
                        ActiveQuery = " AND LD.active = 2"
                    End If
                End If
            End If

            Dim ConnectionLocal As MySqlConnection = LocalhostConn()
            Dim cmd As MySqlCommand
            Dim da As MySqlDataAdapter
            CustomReportdt = New DataTable
            Dim sql As String = ""


            If TaxType = "All" Then
                If TransactionType = "All" Then
                    If DiscountType = "All" Then
                        sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' " & ActiveQuery & " " & WhereExtension
                    Else
                        sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE date(LD.created_at) >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND date(LD.created_at) <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' " & ActiveQuery & " AND LD.discount_type = '" & DiscountType & "'" & WhereExtension
                    End If
                ElseIf TransactionType = "All(Cash)" Then
                    If DiscountType = "All" Then
                        sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' " & ActiveQuery & " AND LD.transaction_type IN('Walk-In','Registered')" & WhereExtension
                    Else
                        sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE date(LD.created_at) >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND date(LD.created_at) <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' " & ActiveQuery & " AND LD.transaction_type IN('Walk-In','Registered') AND LD.discount_type = '" & DiscountType & "'" & WhereExtension
                    End If
                ElseIf TransactionType = "All(Others)" Then
                    If DiscountType = "All" Then
                        sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' " & ActiveQuery & " AND LD.transaction_type NOT IN('Walk-In','Registered')" & WhereExtension
                    Else
                        sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE date(LD.created_at) >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND date(LD.created_at) <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' " & ActiveQuery & " AND LD.transaction_type NOT IN('Walk-In','Registered') AND LD.discount_type = '" & DiscountType & "'" & WhereExtension
                    End If
                Else
                    If DiscountType = "All" Then
                        sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' " & ActiveQuery & " AND LD.transaction_type = '" & TransactionType & "' " & WhereExtension
                    Else
                        sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE date(LD.created_at) >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND date(LD.created_at) <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' " & ActiveQuery & " AND LD.transaction_type = '" & TransactionType & "' AND LD.discount_type = '" & DiscountType & "'" & WhereExtension
                    End If
                End If
            Else
                If TaxType = "VAT" Then
                    If TransactionType = "All" Then
                        If DiscountType = "All" Or DiscountType = "N/A" Then
                            sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.discount_type = 'N/A' AND LD.zeroratedsales = 0 AND LD.active = 1 " & WhereExtension
                        End If
                    ElseIf TransactionType = "All(Cash)" Then
                        If DiscountType = "All" Or DiscountType = "N/A" Then
                            sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.discount_type = 'N/A' AND LD.zeroratedsales = 0 AND LD.active = 1 AND LD.transaction_type IN('Walk-In','Registered')" & WhereExtension
                        End If
                    ElseIf TransactionType = "All(Others)" Then
                        If DiscountType = "All" Or DiscountType = "N/A" Then
                            sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.discount_type = 'N/A' AND LD.zeroratedsales = 0 AND LD.active = 1 AND LD.transaction_type NOT IN('Walk-In','Registered')" & WhereExtension
                        End If
                    Else
                        If DiscountType = "All" Or DiscountType = "N/A" Then
                            sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.discount_type = 'N/A' AND LD.zeroratedsales = 0 AND LD.active = 1 AND LD.transaction_type = '" & TransactionType & "' " & WhereExtension
                        End If
                    End If
                ElseIf TaxType = "NONVAT" Then
                    Dim Types As String = "'Senior Discount 20%','PWD Discount 20%','Sports Discount 20%'"
                    If TransactionType = "All" Then
                        If DiscountType = "All" Or DiscountType = "Percentage(w/o vat)" Then
                            sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.discount_type IN (" & Types & ") AND LD.active = 1" & WhereExtension
                        End If
                    ElseIf TransactionType = "All(Cash)" Then
                        If DiscountType = "All" Or DiscountType = "Percentage(w/o vat)" Then
                            sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.discount_type IN (" & Types & ") AND LD.active = 1 AND LD.transaction_type IN('Walk-In','Registered')" & WhereExtension
                        End If
                    ElseIf TransactionType = "All(Others)" Then
                        If DiscountType = "All" Or DiscountType = "Percentage(w/o vat)" Then
                            sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.discount_type IN (" & Types & ") AND LD.active = 1 AND LD.transaction_type NOT IN('Walk-In','Registered')" & WhereExtension
                        End If
                    Else
                        If DiscountType = "All" Or DiscountType = "Percentage(w/o vat)" Then
                            sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.discount_type IN (" & Types & ") AND LD.transaction_type = '" & TransactionType & "' AND LD.active = 1" & WhereExtension
                        End If
                    End If
                ElseIf TaxType = "ZERO RATED" Then
                    If TransactionType = "All" Then
                        If DiscountType = "All" Then
                            sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.zeroratedsales > 0 " & WhereExtension
                        Else
                            sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.zeroratedsales > 0 AND LD.discount_type = '" & DiscountType & "'" & WhereExtension
                        End If
                    ElseIf TransactionType = "All(Cash)" Then
                        If DiscountType = "All" Then
                            sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.zeroratedsales > 0 AND LD.transaction_type IN('Walk-In','Registered')" & WhereExtension
                        Else
                            sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.zeroratedsales > 0 AND LD.transaction_type IN('Walk-In','Registered') AND LD.discount_type = '" & DiscountType & "'" & WhereExtension
                        End If
                    ElseIf TransactionType = "All(Others)" Then
                        If DiscountType = "All" Then
                            sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.zeroratedsales > 0 AND LD.transaction_type NOT IN('Walk-In','Registered')" & WhereExtension
                        Else
                            sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.zeroratedsales > 0 AND LD.transaction_type NOT IN('Walk-In','Registered') AND LD.discount_type = '" & DiscountType & "'" & WhereExtension
                        End If
                    Else
                        If DiscountType = "All" Then
                            sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.zeroratedsales > 0 AND LD.transaction_type = '" & TransactionType & "' AND LD.active = 1" & WhereExtension
                        Else
                            sql = "SELECT " & FieldsNormal & " FROM loc_daily_transaction LD " & LeftJointNormal & " WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.zeroratedsales > 0 AND LD.transaction_type = '" & TransactionType & "' AND LD.active = 1 AND LD.discount_type = '" & DiscountType & "'" & WhereExtension
                        End If
                    End If
                End If
            End If
            GrossSales = 0
            VatableSales = 0
            LessVat = 0
            AddVat = 0
            VatExempt = 0
            TotalDisc = 0
            NetSales = 0
            GrossRefund = 0
            VatPercentage = 0

            If sql <> "" Then
                cmd = New MySqlCommand(sql, ConnectionLocal)
                da = New MySqlDataAdapter(cmd)
                da.Fill(CustomReportdt)
                grossRefund = 0
                For Each row As DataRow In CustomReportdt.Rows

                    Dim GCVal As String = ""
                    If row("gc_value").ToString = "" Then
                        GCVal = "0"
                    Else
                        GCVal = row("gc_value")
                    End If

                    Dim DiscVal As String = ""
                    If row("totaldiscount").ToString = "" Then
                        DiscVal = "0"
                    Else
                        DiscVal = row("totaldiscount")

                    End If
                    Dim trxType = ""

                    Dim GrossSalesDgv As Double = 0
                    Dim VatableSalesDgv As Double = 0
                    Dim VatPercentageDgv As Double = 0
                    Dim LessVatDgv As Double = 0
                    Dim VatExemptDgv As Double = 0
                    Dim NetSalesDgv As Double = 0
                    Dim AddVatDgv As Double = 0

                    If row("transaction_type") = "Complimentary Expenses" Then
                        trxType = "Complimentary"
                        GCVal = "0.00"
                        DiscVal = "0.00"
                    Else
                        Dim Status = row("Status")
                        trxType = row("transaction_type")
                        GrossSalesDgv = row("grosssales")
                        VatableSalesDgv = row("vatablesales")
                        VatPercentageDgv = row("vatpercentage")
                        LessVatDgv = row("lessvat")
                        VatExemptDgv = row("vatexemptsales")
                        NetSalesDgv = row("amountdue")
                        AddVatDgv = row("AddVat")
                        If Status <> 2 Then
                            GrossSales += GrossSalesDgv
                            VatableSales += VatableSalesDgv
                            VatPercentage += VatPercentageDgv
                            LessVat += LessVatDgv
                            VatExempt += VatExemptDgv
                            NetSales += NetSalesDgv
                            AddVat += AddVatDgv
                            TotalDisc += Val(DiscVal)
                        End If
                    End If

                    If row("Status") = 2 Then
                        GrossRefund += GrossSalesDgv
                    End If

                    DataGridViewCustomReport.Rows.Add(row("transaction_number"), GrossSalesDgv, VatableSalesDgv, VatPercentageDgv, LessVatDgv, VatExemptDgv, GCVal, DiscVal, trxType, NetSalesDgv, row("dateCreated"), AddVatDgv)
                Next
            End If

            Dim sql1 As String = ""
            Dim cmd1 As MySqlCommand

            Dim list As List(Of String) = New List(Of String)

            For i As Integer = 0 To DataGridViewCustomReport.Rows.Count - 1 Step +1
                list.Add(DataGridViewCustomReport.Rows(i).Cells(1).Value)
            Next

            Dim result As List(Of String) = list.Distinct().ToList

            CustomReportVat = 0
            CustomReportLessVat = 0

            ' Display result.
            For Each element As String In result
                If ProductName = "All" Then
                    If TaxType = "All" Then
                        If TransactionType = "All" Then
                            If DiscountType = "All" Then
                                sql1 = "SELECT vatablesales, lessvat, vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND transaction_number = '" & element & "'"
                            Else
                                sql1 = "SELECT vatablesales, lessvat, vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND transaction_number = '" & element & "' AND discount_type = '" & DiscountType & "'"
                            End If
                        ElseIf TransactionType = "All(Cash)" Then
                            If DiscountType = "All" Then
                                sql1 = "SELECT vatablesales, lessvat, vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND transaction_number = '" & element & "' AND transaction_type IN('Walk-In','Registered')"
                            Else
                                sql1 = "SELECT vatablesales, lessvat, vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND transaction_number = '" & element & "' AND transaction_type IN('Walk-In','Registered') AND discount_type = '" & DiscountType & "'"
                            End If
                        ElseIf TransactionType = "All(Others)" Then
                            If DiscountType = "All" Then
                                sql1 = "SELECT vatablesales, lessvat, vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND transaction_number = '" & element & "' AND transaction_type NOT IN('Walk-In','Registered')" & WhereExtension
                            Else
                                sql1 = "SELECT vatablesales, lessvat, vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND transaction_number = '" & element & "' AND transaction_type NOT IN('Walk-In','Registered') AND discount_type = '" & DiscountType & "'"
                            End If
                        Else
                            If DiscountType = "All" Then
                                sql1 = "SELECT vatablesales, lessvat, vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND transaction_type = '" & TransactionType & "' AND transaction_number = '" & element & "'" & WhereExtension
                            Else
                                sql1 = "SELECT vatablesales, lessvat, vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND transaction_type = '" & TransactionType & "' AND transaction_number = '" & element & "' AND discount_type = '" & DiscountType & "'"
                            End If
                        End If
                    Else
                        If TaxType = "VAT" Then
                            If TransactionType = "All" Then
                                If DiscountType = "All" Or DiscountType = "N/A" Then
                                    sql1 = "SELECT vatablesales, lessvat, LD.vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND discount_type = 'N/A' AND zeroratedsales = 0 AND transaction_number = '" & element & "'"
                                End If
                            ElseIf TransactionType = "All(Cash)" Then
                                If DiscountType = "All" Or DiscountType = "N/A" Then
                                    sql1 = "SELECT vatablesales, lessvat, LD.vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND discount_type = 'N/A' AND zeroratedsales = 0 AND transaction_number = '" & element & "' AND transaction_type IN('Walk-In','Registered')"
                                End If
                            ElseIf TransactionType = "All(Others)" Then
                                If DiscountType = "All" Or DiscountType = "N/A" Then
                                    sql1 = "SELECT vatablesales, lessvat, LD.vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND discount_type = 'N/A' AND zeroratedsales = 0 AND transaction_number = '" & element & "' AND transaction_type NOT IN('Walk-In','Registered')"
                                End If
                            Else
                                If DiscountType = "All" Or DiscountType = "N/A" Then
                                    sql1 = "SELECT vatablesales, lessvat, LD.vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND discount_type = 'N/A' AND zeroratedsales = 0 AND transaction_type = '" & TransactionType & "' AND transaction_number = '" & element & "'"
                                End If
                            End If
                        ElseIf TaxType = "NONVAT" Then
                            Dim Types As String = "'Senior Discount 20%','PWD Discount 20%','Sports Discount 20%'"
                            If TransactionType = "All" Then
                                If DiscountType = "All" Or DiscountType = "Percentage(w/o vat)" Then
                                    sql1 = "SELECT vatablesales, lessvat, LD.vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND discount_type IN (" & Types & ") AND transaction_number = '" & element & "'"
                                End If
                            ElseIf TransactionType = "All(Cash)" Then
                                If DiscountType = "All" Or DiscountType = "Percentage(w/o vat)" Then
                                    sql1 = "SELECT vatablesales, lessvat, LD.vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND discount_type IN (" & Types & ") AND transaction_number = '" & element & "' AND transaction_type IN('Walk-In','Registered')"
                                End If
                            ElseIf TransactionType = "All(Others)" Then
                                If DiscountType = "All" Or DiscountType = "Percentage(w/o vat)" Then
                                    sql1 = "SELECT vatablesales, lessvat, LD.vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND discount_type IN (" & Types & ") AND transaction_number = '" & element & "' AND transaction_type NOT IN('Walk-In','Registered')"
                                End If
                            Else
                                If DiscountType = "All" Or DiscountType = "Percentage(w/o vat)" Then
                                    sql1 = "SELECT vatablesales, lessvat, LD.vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND discount_type IN (" & Types & ") AND transaction_type = '" & TransactionType & "' AND transaction_number = '" & element & "'"
                                End If
                            End If
                        ElseIf TaxType = "ZERO RATED" Then
                            If TransactionType = "All" Then
                                If DiscountType = "All" Then
                                    sql1 = "SELECT vatablesales, lessvat, LD.vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND zeroratedsales > 0 AND transaction_number = '" & element & "'"
                                Else
                                    sql1 = "SELECT vatablesales, lessvat, LD.vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND zeroratedsales > 0 AND transaction_number = '" & element & "' AND discount_type = '" & DiscountType & "'"
                                End If
                            ElseIf TransactionType = "All(Cash)" Then
                                If DiscountType = "All" Then
                                    sql1 = "SELECT vatablesales, lessvat, LD.vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND zeroratedsales > 0 AND transaction_number = '" & element & "' AND transaction_type IN('Walk-In','Registered')"
                                Else
                                    sql1 = "SELECT vatablesales, lessvat, LD.vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND zeroratedsales > 0 AND transaction_number = '" & element & "' AND transaction_type IN('Walk-In','Registered') AND discount_type = '" & DiscountType & "'"
                                End If
                            ElseIf TransactionType = "All(Others)" Then
                                If DiscountType = "All" Then
                                    sql1 = "SELECT vatablesales, lessvat, LD.vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND zeroratedsales > 0 AND transaction_number = '" & element & "' AND transaction_type NOT IN('Walk-In','Registered')"
                                Else
                                    sql1 = "SELECT vatablesales, lessvat, LD.vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND zeroratedsales > 0 AND transaction_number = '" & element & "' AND transaction_type NOT IN('Walk-In','Registered') AND discount_type = '" & DiscountType & "'"
                                End If
                            Else
                                If DiscountType = "All" Then
                                    sql1 = "SELECT vatablesales, lessvat, LD.vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND zeroratedsales > 0 AND transaction_type = '" & TransactionType & "' AND transaction_number = '" & element & "'"
                                Else
                                    sql1 = "SELECT vatablesales, lessvat, LD.vatpercentage FROM loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND zeroratedsales > 0 AND transaction_type = '" & TransactionType & "' AND transaction_number = '" & element & "' AND discount_type = '" & DiscountType & "'"
                                End If
                            End If
                        End If
                    End If
                Else
                    If TaxType = "All" Then
                        If TransactionType = "All" Then
                            If DiscountType = "All" Then
                                sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "'"
                            Else
                                sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "' AND discount_type = '" & DiscountType & "'"
                            End If
                        ElseIf TransactionType = "All(Cash)" Then
                            If DiscountType = "All" Then
                                sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "' AND LD.transaction_type IN('Walk-In','Registered')"
                            Else
                                sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "' AND LD.transaction_type IN('Walk-In','Registered') AND discount_type = '" & DiscountType & "'"
                            End If
                        ElseIf TransactionType = "All(Others)" Then
                            If DiscountType = "All" Then
                                sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "' AND LD.transaction_type NOT IN('Walk-In','Registered')"
                            Else
                                sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "' AND LD.transaction_type NOT IN('Walk-In','Registered') AND discount_type = '" & DiscountType & "'"
                            End If
                        Else
                            If DiscountType = "All" Then
                                sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.transaction_type = '" & TransactionType & "' AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "'"
                            Else
                                sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.transaction_type = '" & TransactionType & "' AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "' AND discount_type = '" & DiscountType & "'"
                            End If
                        End If
                    Else
                        If TaxType = "VAT" Then
                            If TransactionType = "All" Then
                                If DiscountType = "All" Or DiscountType = "N/A" Then
                                    sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.discount_type = 'N/A' AND zeroratedsales = 0 AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "'"
                                End If
                            ElseIf TransactionType = "All(Cash)" Then
                                If DiscountType = "All" Or DiscountType = "N/A" Then
                                    sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.discount_type = 'N/A' AND zeroratedsales = 0 AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "' AND LD.transaction_type IN('Walk-In','Registered')"
                                End If
                            ElseIf TransactionType = "All(Others)" Then
                                If DiscountType = "All" Or DiscountType = "N/A" Then
                                    sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.discount_type = 'N/A' AND zeroratedsales = 0 AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "' AND LD.transaction_type NOT IN('Walk-In','Registered')"
                                End If
                            Else
                                If DiscountType = "All" Or DiscountType = "N/A" Then
                                    sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.discount_type = 'N/A' AND zeroratedsales = 0 AND LD.transaction_type = '" & TransactionType & "' AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "'"
                                End If
                            End If
                        ElseIf TaxType = "NONVAT" Then
                            Dim Types As String = "'Senior Discount 20%','PWD Discount 20%','Sports Discount 20%'"
                            If TransactionType = "All" Then
                                If DiscountType = "All" Or DiscountType = "Percentage(w/o vat)" Then
                                    sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.discount_type IN (" & Types & ") AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "'"
                                End If
                            ElseIf TransactionType = "All(Cash)" Then
                                If DiscountType = "All" Or DiscountType = "Percentage(w/o vat)" Then
                                    sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.discount_type IN (" & Types & ") AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "' AND LD.transaction_type IN('Walk-In','Registered')"
                                End If
                            ElseIf TransactionType = "All(Others)" Then
                                If DiscountType = "All" Or DiscountType = "Percentage(w/o vat)" Then
                                    sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.discount_type IN (" & Types & ") AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "' AND LD.transaction_type NOT IN('Walk-In','Registered')"
                                End If
                            Else
                                If DiscountType = "All" Or DiscountType = "Percentage(w/o vat)" Then
                                    sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.discount_type IN (" & Types & ") AND LD.transaction_type = '" & TransactionType & "' AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "'"
                                End If
                            End If
                        ElseIf TaxType = "ZERO RATED" Then
                            If TransactionType = "All" Then
                                If DiscountType = "All" Then
                                    sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.zeroratedsales > 0 AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "'"
                                Else
                                    sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.zeroratedsales > 0 AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "' AND LD.discount_type = '" & DiscountType & "'"
                                End If
                            ElseIf TransactionType = "All(Cash)" Then
                                If DiscountType = "All" Then
                                    sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.zeroratedsales > 0 AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "' AND LD.transaction_type IN('Walk-In','Registered')"
                                Else
                                    sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.zeroratedsales > 0 AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "' AND LD.transaction_type IN('Walk-In','Registered') AND LD.discount_type = '" & DiscountType & "'"
                                End If
                            ElseIf TransactionType = "All(Others)" Then
                                If DiscountType = "All" Then
                                    sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.zeroratedsales > 0 AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "' AND LD.transaction_type NOT IN('Walk-In','Registered')"
                                Else
                                    sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.zeroratedsales > 0 AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "' AND LD.transaction_type NOT IN('Walk-In','Registered') AND LD.discount_type = '" & DiscountType & "'"
                                End If
                            Else
                                If DiscountType = "All" Then
                                    sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.zeroratedsales > 0 AND LD.transaction_type = '" & TransactionType & "' AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "'"
                                Else
                                    sql1 = "SELECT LD.vatablesales, LD.lessvat, LD.vatpercentage FROM loc_daily_transaction LD INNER JOIN loc_daily_transaction_details LDT ON LD.transaction_number = LDT.transaction_number WHERE LD.zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND LD.zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "' AND LD.zeroratedsales > 0 AND LD.transaction_type = '" & TransactionType & "' AND LD.transaction_number = '" & element & "' AND LDT.product_name = '" & ProductName & "' AND LD.discount_type = '" & DiscountType & "'"
                                End If
                            End If
                        End If
                    End If
                End If
                cmd1 = New MySqlCommand(sql1, ConnectionLocal)
                Using reader As MySqlDataReader = cmd1.ExecuteReader
                    If reader.HasRows Then
                        While reader.Read
                            CustomReportVat += reader("vatablesales")
                            CustomReportLessVat += reader("lessvat")

                        End While
                    End If
                End Using
            Next
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "ADVCustRep/Customreport(): " & ex.ToString, "Critical")
        End Try
    End Sub
    Dim TotalDiscountCustomReports As Double = 0
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            TotalDiscountCustomReports = 0
            DataGridViewCustomReport.Rows.Clear()
            CustomReport(ToolStripComboBoxTaxType.Text, ToolStripComboBoxTransactionType.Text, ToolStripComboBoxDiscType.Text)
            ToolStripStatusLabel2.Text = DataGridViewCustomReport.Rows.Count
            TotalDiscountCustomReports = sum("totaldiscount", "loc_daily_transaction WHERE zreading >= '" & Format(DateTimePicker17.Value, "yyyy-MM-dd") & "' AND  zreading <= '" & Format(DateTimePicker18.Value, "yyyy-MM-dd") & "'")
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "ADVCustRep/Button1: " & ex.ToString, "Critical")
        End Try
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            Dim document As PdfDocument = New PdfDocument
            document.Info.Title = "Created with PDFsharp"
            Dim page As PdfPage = document.Pages.Add
            Dim gfx As XGraphics = XGraphics.FromPdfPage(page)
            Dim font As XFont = New XFont("Verdana", 7, XFontStyle.Regular)
            Dim font1 As XFont = New XFont("Verdana", 7, XFontStyle.Bold)


            Dim list As List(Of String) = New List(Of String)

            For i As Integer = 0 To DataGridViewCustomReport.Rows.Count - 1 Step +1
                If Not list.Contains(DataGridViewCustomReport.Rows(i).Cells(1).Value) Then
                    list.Add(DataGridViewCustomReport.Rows(i).Cells(1).Value)
                End If
            Next

            Dim result As List(Of String) = list.Distinct().ToList
            Dim TotalNetSales As Double = 0
            Dim ConnectionLocal As MySqlConnection = LocalhostConn()

            For Each element As String In result
                'Console.WriteLine(element.ToString)
                Dim Query = "SELECT amountdue FROM loc_daily_transaction WHERE transaction_number = '" & element & "'"
                Dim Cmd As MySqlCommand = New MySqlCommand(Query, ConnectionLocal)
                Using reader As MySqlDataReader = Cmd.ExecuteReader
                    If reader.HasRows Then
                        While reader.Read
                            TotalNetSales += reader("amountdue")
                        End While
                    End If
                End Using
            Next

            If DataGridViewCustomReport.Rows.Count > 0 Then
                ' Create a new PDF document
                Dim NextPage As Integer = DataGridViewCustomReport.Rows.Count
                Dim PageRows As Integer = 50
                Dim TotalRowsPerPage As Integer = NextPage / PageRows

                If NextPage <= 50 Then
                    TotalRowsPerPage = 1
                Else
                    TotalRowsPerPage += 1
                End If
                Dim Kahitano As Integer = 1
                Dim GetDgvRowCount As Integer = 0

                For a = 1 To TotalRowsPerPage

                    If a <> Kahitano Then
                        page = document.AddPage
                        gfx = XGraphics.FromPdfPage(page)
                        gfx.DrawString("Date From - To: " & DateTimePicker17.Value.ToString & " | " & DateTimePicker18.Value.ToString, font, XBrushes.Black, 50, 50)
                        gfx.DrawString("Product Name: N/A", font, XBrushes.Black, 50, 61)
                        gfx.DrawString("Tax Type: " & ToolStripComboBoxTaxType.Text, font, XBrushes.Black, 50, 72)
                        gfx.DrawString("Transaction Type: " & ToolStripComboBoxTransactionType.Text, font, XBrushes.Black, 50, 83)
                        gfx.DrawString("Discount Type: " & ToolStripComboBoxDiscType.Text, font, XBrushes.Black, 50, 94)

                        gfx.DrawString("Trx. #", font1, XBrushes.Black, 50, 103 + 10)
                        gfx.DrawString("Gross Sales", font1, XBrushes.Black, 85, 103 + 10)
                        gfx.DrawString("Vatable Sales", font1, XBrushes.Black, 135, 103 + 10)
                        gfx.DrawString("12% Vat", font1, XBrushes.Black, 195, 103 + 10)
                        gfx.DrawString("Less Vat", font1, XBrushes.Black, 235, 103 + 10)
                        gfx.DrawString("Vat Exempt Sales", font1, XBrushes.Black, 280, 103 + 10)
                        gfx.DrawString("Discount", font1, XBrushes.Black, 360, 103 + 10)
                        gfx.DrawString("Trx. Type", font1, XBrushes.Black, 400, 103 + 10)
                        gfx.DrawString("Net Sales", font1, XBrushes.Black, 450, 103 + 10)
                        gfx.DrawString("Date/Time", font1, XBrushes.Black, 500, 103 + 10)

                        Dim RowCount As Integer = 10
                        Dim CountPage As Integer = 0
                        With DataGridViewCustomReport

                            For i As Integer = GetDgvRowCount To .Rows.Count - 1 Step +1
                                If CountPage < PageRows Then
                                    gfx.DrawString(.Rows(i).Cells(0).Value, font, XBrushes.Black, 50, 123 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(1).Value, font, XBrushes.Black, 85, 123 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(2).Value, font, XBrushes.Black, 135, 123 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(3).Value, font, XBrushes.Black, 195, 123 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(4).Value, font, XBrushes.Black, 235, 123 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(5).Value, font, XBrushes.Black, 280, 123 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(7).Value, font, XBrushes.Black, 360, 123 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(8).Value, font, XBrushes.Black, 400, 123 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(9).Value, font, XBrushes.Black, 450, 123 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(10).Value, font, XBrushes.Black, 500, 123 + RowCount)
                                    RowCount += 10
                                    CountPage += 1
                                    GetDgvRowCount += 1
                                Else
                                    Exit For
                                End If

                            Next
                        End With

                        gfx.DrawString("Total Items: " & ToolStripStatusLabel2.Text, font, XBrushes.Black, 50, 133 + RowCount)
                        gfx.DrawString("Total Gross Sales: " & GrossSales, font, XBrushes.Black, 50, 143 + RowCount)
                        gfx.DrawString("Total Discount: " & TotalDisc, font, XBrushes.Black, 50, 153 + RowCount)
                        gfx.DrawString("Net Sales: " & NetSales, font, XBrushes.Black, 50, 163 + RowCount)
                        gfx.DrawString("Vatable Sales: " & VatableSales, font, XBrushes.Black, 50, 173 + RowCount)
                        gfx.DrawString("Less Vat: " & LessVat, font, XBrushes.Black, 50, 183 + RowCount)
                        gfx.DrawString("Add Vat: " & AddVat, font, XBrushes.Black, 50, 193 + RowCount)
                        gfx.DrawString("Vat Exempt Sales: " & VatExempt, font, XBrushes.Black, 50, 203 + RowCount)
                        gfx.DrawString("Refunded/Cancel: " & NUMBERFORMAT(grossRefund), font, XBrushes.Black, 50, 213 + RowCount)
                        gfx.DrawString("Date Generated: " & FullDate24HR(), font, XBrushes.Black, 50, 223 + RowCount)


                        Kahitano += 1
                    Else
                        gfx.DrawString("Date From - To: " & DateTimePicker17.Value.ToString & " | " & DateTimePicker18.Value.ToString, font, XBrushes.Black, 50, 50)
                        gfx.DrawString("Product Name: N/A", font, XBrushes.Black, 50, 61)
                        gfx.DrawString("Tax Type: " & ToolStripComboBoxTaxType.Text, font, XBrushes.Black, 50, 72)
                        gfx.DrawString("Transaction Type: " & ToolStripComboBoxTransactionType.Text, font, XBrushes.Black, 50, 83)
                        gfx.DrawString("Discount Type: " & ToolStripComboBoxDiscType.Text, font, XBrushes.Black, 50, 94)

                        gfx.DrawString("Trx. #", font1, XBrushes.Black, 50, 103 + 10)
                        gfx.DrawString("Gross Sales", font1, XBrushes.Black, 85, 103 + 10)
                        gfx.DrawString("Vatable Sales", font1, XBrushes.Black, 135, 103 + 10)
                        gfx.DrawString("12% Vat", font1, XBrushes.Black, 195, 103 + 10)
                        gfx.DrawString("Less Vat", font1, XBrushes.Black, 235, 103 + 10)
                        gfx.DrawString("Vat Exempt Sales", font1, XBrushes.Black, 280, 103 + 10)
                        gfx.DrawString("Discount", font1, XBrushes.Black, 360, 103 + 10)
                        gfx.DrawString("TRN. Type", font1, XBrushes.Black, 400, 103 + 10)
                        gfx.DrawString("Net Sales", font1, XBrushes.Black, 450, 103 + 10)
                        gfx.DrawString("Date/Time", font1, XBrushes.Black, 500, 103 + 10)



                        Dim RowCount As Integer = 10
                        Dim CountPage As Integer = 0
                        With DataGridViewCustomReport

                            For i As Integer = 0 To .Rows.Count - 1 Step +1

                                If i < PageRows Then
                                    gfx.DrawString(.Rows(i).Cells(0).Value, font, XBrushes.Black, 50, 123 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(1).Value, font, XBrushes.Black, 85, 123 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(2).Value, font, XBrushes.Black, 135, 123 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(3).Value, font, XBrushes.Black, 195, 123 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(4).Value, font, XBrushes.Black, 235, 123 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(5).Value, font, XBrushes.Black, 280, 123 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(7).Value, font, XBrushes.Black, 360, 123 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(8).Value, font, XBrushes.Black, 400, 123 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(9).Value, font, XBrushes.Black, 450, 123 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(10).Value, font, XBrushes.Black, 500, 123 + RowCount)

                                    RowCount += 10

                                    CountPage += 1
                                    GetDgvRowCount += 1
                                Else
                                    Exit For
                                End If
                            Next
                        End With

                        gfx.DrawString("Total Items: " & ToolStripStatusLabel2.Text, font, XBrushes.Black, 50, 133 + RowCount)
                        gfx.DrawString("Total Gross Sales: " & GrossSales, font, XBrushes.Black, 50, 143 + RowCount)
                        gfx.DrawString("Total Discount: " & TotalDisc, font, XBrushes.Black, 50, 153 + RowCount)
                        gfx.DrawString("Net Sales: " & NetSales, font, XBrushes.Black, 50, 163 + RowCount)
                        gfx.DrawString("Vatable Sales: " & VatableSales, font, XBrushes.Black, 50, 173 + RowCount)
                        gfx.DrawString("Less Vat: " & LessVat, font, XBrushes.Black, 50, 183 + RowCount)
                        gfx.DrawString("Add Vat: " & AddVat, font, XBrushes.Black, 50, 193 + RowCount)
                        gfx.DrawString("Vat Exempt Sales: " & VatExempt, font, XBrushes.Black, 50, 203 + RowCount)
                        gfx.DrawString("Refunded/Cancel: " & NUMBERFORMAT(GrossRefund), font, XBrushes.Black, 50, 213 + RowCount)
                        gfx.DrawString("Date Generated: " & FullDate24HR(), font, XBrushes.Black, 50, 223 + RowCount)

                    End If
                Next
                AuditTrail.LogToAuditTrail("Report", "Reports/Advanced Custom Report: Generated Report, " & ClientCrewID, "Normal")

                Dim filename = My.Computer.FileSystem.SpecialDirectories.Desktop & "\Advanced-Custom-Report-" & FullDateFormatForSaving() & ".pdf"
                document.Save(filename)

                ' ...and start a viewer.
                Process.Start(filename)

            End If
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "ADVCustRep/Button2: " & ex.ToString, "Critical")
        End Try
    End Sub
End Class