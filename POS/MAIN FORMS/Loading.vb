﻿Imports System.Net
Imports System.Threading
Imports MySql.Data.MySqlClient
Public Class Loading
    Inherits Form
    Dim RowsReturned As Integer
    Dim thread As Thread
    Dim IfItsIstDayOfTheMonth As Boolean
    Dim IfInternetIsAvailable As Boolean
    Dim IfNeedsToReset As Boolean = False
    Dim if1stdayofthemonth
    Property ListOfScripts As New List(Of ScriptRunnerCls)
    Private Sub Loadme()
        Try
            LabelVersion.Text = My.Settings.Version
            LabelFOOTER.Text = My.Settings.Footer
            CheckForIllegalCrossThreadCalls = False

            CreateXmlPath()

            BackgroundWorker1.WorkerSupportsCancellation = True
            BackgroundWorker1.WorkerReportsProgress = True
            BackgroundWorker1.RunWorkerAsync()

        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
        End Try
    End Sub
    Private Shared _instance As Loading
    Public ReadOnly Property Instance As Loading
        Get
            Return _instance
        End Get
    End Property

    Private Sub Loading_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            _instance = Me

            ChangeProgBarColor(ProgressBar1, ProgressBarColor.Yellow)
            Loadme()
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
        End Try
    End Sub
    Dim threadList As List(Of Thread) = New List(Of Thread)

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Try
            For i = 0 To 10
                BackgroundWorker1.ReportProgress(i)
                Thread.Sleep(50)
                If i = 5 Then
                    Label1.Text = "Checking local connection..."
                    thread = New Thread(AddressOf LoadLocalConnection)
                    thread.Start()
                    threadList.Add(thread)
                End If
                If i = 10 Then
                    Label1.Text = "Checking cloud connection..."
                    thread = New Thread(AddressOf LoadCloudConnString)
                    thread.Start()
                    threadList.Add(thread)
                End If
            Next
            For Each t In threadList
                t.Join()
            Next
            For i = 10 To 100
                BackgroundWorker1.ReportProgress(i)
                Thread.Sleep(50)
                If i = 10 Then
                    If ValidLocalConnection Then

                        Label1.Text = "Getting information..."
                        IfConnectionIsConfigured = True
                        ValidDatabaseLocalConnection = True
                        thread = New Thread(AddressOf LoadMasterList)
                        thread.Start()
                        threadList.Add(thread)
                        For Each t In threadList
                            t.Join()
                        Next
                        thread = New Thread(Sub() ListOfScripts = FillScript())
                        thread.Start()
                        threadList.Add(thread)
                        For Each t In threadList
                            t.Join()
                        Next
                    Else
                        IfConnectionIsConfigured = False
                        Label1.Text = "Please Setup Connection in Configuration Manager..."
                    End If
                End If
                'If i = 20 Then
                '    If ValidLocalConnection Then
                '        thread = New Thread(AddressOf AutoMaticResetPOS)
                '        thread.Start()
                '        threadList.Add(thread)
                '        For Each t In threadList
                '            t.Join()
                '        Next
                '    End If
                'End If
                If i = 30 Then
                    If ValidDatabaseLocalConnection Then
                        Label1.Text = "Checking for updates..."
                    End If
                End If
                If i = 40 Then
                    If CheckForInternetConnection() Then
                        IfInternetIsAvailable = True
                        Label1.Text = "Connecting to cloud server..."
                        If ValidDatabaseLocalConnection Then
                            thread = New Thread(AddressOf ServerCloudCon)
                            thread.Start()
                            threadList.Add(thread)
                            For Each t In threadList
                                t.Join()
                            Next
                        End If
                        If ValidCloudConnection Then
                            thread = New Thread(AddressOf RunScript)
                            thread.Start()
                            threadList.Add(thread)

                            For Each t In threadList
                                t.Join()
                            Next
                        End If
                    Else
                        IfInternetIsAvailable = False
                        Label1.Text = "No Internet Connection..."
                    End If
                End If
                If My.Settings.Auto_Update Then
                    If i = 50 Then
                        If CheckForInternetConnection() Then
                            Label1.Text = "Checking for updates"
                            If ValidDatabaseLocalConnection Then
                                If ValidCloudConnection Then
                                    thread = New Thread(AddressOf CheckPriceChanges)
                                    thread.Start()
                                    threadList.Add(thread)

                                    For Each t In threadList
                                        t.Join()
                                    Next

                                    thread = New Thread(AddressOf SetDatatablesToNew)
                                    thread.Start()
                                    threadList.Add(thread)

                                    For Each t In threadList
                                        t.Join()
                                    Next

                                    thread = New Thread(Sub() GetCategoryUpdate(0))
                                    thread.Start()
                                    threadList.Add(thread)

                                    thread = New Thread(Sub() GetFormulaUpdate(0))
                                    thread.Start()
                                    threadList.Add(thread)

                                    thread = New Thread(Sub() GetInventoryUpdate(0))
                                    thread.Start()
                                    threadList.Add(thread)

                                    thread = New Thread(Sub() GetCouponsUpdate(0))
                                    thread.Start()
                                    threadList.Add(thread)

                                    thread = New Thread(Sub() GetPartnersUpdate(0))
                                    thread.Start()
                                    threadList.Add(thread)

                                    thread = New Thread(AddressOf CouponApproval)
                                    thread.Start()
                                    threadList.Add(thread)

                                    thread = New Thread(AddressOf CustomProductApproval)
                                    thread.Start()
                                    threadList.Add(thread)

                                    thread = New Thread(Sub() GetProductUpdates(0))
                                    thread.Start()
                                    threadList.Add(thread)

                                    thread = New Thread(AddressOf PromptMessage)
                                    thread.Start()
                                    threadList.Add(thread)

                                    For Each t In threadList
                                        t.Join()
                                    Next
                                End If
                            End If
                        Else
                            IfInternetIsAvailable = False
                            Label1.Text = "No Internet Connection..."
                        End If
                    End If
                End If

                If i = 60 Then
                    If ValidDatabaseLocalConnection Then
                        thread = New Thread(AddressOf LoadSettings)
                        thread.Start()
                        threadList.Add(thread)
                        For Each t In threadList
                            t.Join()
                            If BackgroundWorker1.CancellationPending Then
                                e.Cancel = True
                            End If
                        Next
                    End If
                End If

                If i = 70 Then
                    If ValidDatabaseLocalConnection Then
                        thread = New Thread(AddressOf LoadOldGrandtotal)
                        thread.Start()
                        threadList.Add(thread)
                    End If
                End If
                If i = 80 Then
                    If IfConnectionIsConfigured Then
                        If AutoInventoryReset Then
                            If CheckIfNeedToReset() Then
                                IfNeedsToReset = True
                            Else
                                IfNeedsToReset = False
                            End If
                        End If
                    End If
                End If
                If i = 90 Then
                    If ValidLocalConnection Then
                        Label1.Text = "Loading..."
                        thread = New Thread(Sub() AuditTrail.LogToAuditTrail("Start/Close", "Opening/Closing of Application details", "Normal"))
                        thread.Start()
                        threadList.Add(thread)
                        For Each t In threadList
                            t.Join()
                        Next
                    End If
                End If
            Next
            For Each t In threadList
                t.Join()
                If BackgroundWorker1.CancellationPending = True Then
                    e.Cancel = True
                End If
            Next

        Catch ex As Exception

            If ValidLocalConnection Then
                AuditTrail.LogToAuditTrail("System", "Loading: Update not successful, " & ex.ToString, "Critical")
                AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
            End If
        End Try
    End Sub
    Private Sub LoadSettings()
        Try

            Dim ConnectionLocal As MySqlConnection = LocalhostConn()
            Dim sql = "SELECT A_Export_Path, A_Tax, A_SIFormat, A_SIBeg, A_Terminal_No, A_ZeroRated, S_Zreading, S_Batter, S_Brownie_Mix , S_Upgrade_Price_Add , S_BackupInterval, S_BackupDate , S_Update_Version , P_Footer_Info , S_logo , S_Layout , printreceipt , reprintreceipt , printxzread , printreturns, autoresetinv, S_Waffle_Bag, S_Packets, printcount, Dev_Company_Name, S_ZeroRated_Tax, printsalesreport FROM loc_settings WHERE settings_id = 1"
            Dim cmd As MySqlCommand = New MySqlCommand(sql, ConnectionLocal)
            'Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
            'Dim dt As DataTable = New DataTable
            'da.Fill(dt)
            Using reader As MySqlDataReader = cmd.ExecuteReader
                If reader.HasRows Then
                    While reader.Read
                        S_ExportPath = ConvertB64ToString(reader("A_Export_Path"))
                        S_Tax = reader("A_Tax")
                        If reader("S_ZeroRated_Tax") = "" Then
                            S_ZeroRated_Tax = 0
                        Else
                            S_ZeroRated_Tax = Double.Parse(reader("S_ZeroRated_Tax"))
                        End If
                        S_SIFormat = reader("A_SIFormat")
                        S_SIBeg = reader("A_SIBeg")
                        S_Terminal_No = ClientStoreID
                        S_ZeroRated = reader("A_ZeroRated")
                        S_Zreading = reader("S_Zreading")
                        S_Batter = reader("S_Batter")
                        S_Brownie_Mix = reader("S_Brownie_Mix")
                        S_Upgrade_Price = reader("S_Upgrade_Price_Add")
                        S_Backup_Interval = reader("S_BackupInterval")
                        S_Backup_Date = reader("S_BackupDate")
                        S_Logo = reader("S_logo")
                        S_Layout = reader("S_Layout")
                        S_Print = reader("printreceipt")
                        S_Reprint = reader("reprintreceipt")
                        S_Print_XZRead = reader("printxzread")
                        S_Print_Returns = reader("printreturns")
                        S_Dev_Comp_Name = reader("Dev_Company_Name")
                        My.Settings.Footer = reader("P_Footer_Info")
                        My.Settings.Version = reader("S_Update_Version")
                        My.Settings.Save()
                        LabelVersion.Text = reader("S_Update_Version")
                        LabelFOOTER.Text = reader("P_Footer_Info")
                        If reader("autoresetinv") = "" Then
                            AutoInventoryReset = False
                        ElseIf reader("autoresetinv") = "0" Then
                            AutoInventoryReset = False
                        ElseIf reader("autoresetinv") = "1" Then
                            AutoInventoryReset = False
                        End If
                        S_Waffle_Bag = reader("S_Waffle_Bag")
                        S_Packets = reader("S_Packets")
                        S_PrintCount = reader("printcount")
                        S_Print_Sales_Report = reader("printsalesreport")
                    End While
                End If
            End Using
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
        End Try
    End Sub

    Private Sub LoadMasterList()
        Try
            If LocalConnectionIsOnOrValid Then
                Dim ConLoc As MySqlConnection = LocalhostConn()
                Dim query = "SELECT `masterlist_username`, `client_product_key`, `user_id`, `client_store_id` FROM admin_masterlist WHERE masterlist_id = 1"
                Using cmd As MySqlCommand = New MySqlCommand(query, ConLoc)
                    Dim reader = cmd.ExecuteReader
                    If reader.HasRows Then
                        RowsReturned = 1
                        While reader.Read
                            ClientGuid = reader("user_id")
                            ClientProductKey = reader("client_product_key")
                            ClientStoreID = reader("client_store_id")
                        End While
                    Else
                        RowsReturned = 0
                    End If
                End Using
            Else
                Label1.Text = "Cannot connect to local server..."
            End If
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub BackgroundWorker1_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        Try
            ProgressBar1.Value = e.ProgressPercentage
            Label2.Text = e.ProgressPercentage
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        Try
            If IfConnectionIsConfigured Then
                If RowsReturned = 1 Then
                    If IfInternetIsAvailable Then
                        If My.Settings.Auto_Update Then
                            If UPDATE_CATEGORY_DATATABLE.Rows.Count > 0 Or UPDATE_PRODUCTS_DATATABLE.Rows.Count > 0 Or UPDATE_FORMULA_DATATABLE.Rows.Count > 0 Or UPDATE_INVENTORY_DATATABLE.Rows.Count > 0 Or UPDATE_PRICE_CHANGE_DATATABLE.Rows.Count > 0 Or UPDATE_COUPON_APPROVAL_DATATABLE.Rows.Count > 0 Or UPDATE_CUSTOM_PROD_APP_DATATABLE.Rows.Count Or UPDATE_COUPONS_DATATABLE.Rows.Count > 0 Or UPDATE_PARTNERS_DATATABLE.Rows.Count > 0 Then
                                ProgressBar1.Value = 0
                                ProgressBar1.Maximum = 0
                                Dim TotalRows = UPDATE_CATEGORY_DATATABLE.Rows.Count + UPDATE_PRODUCTS_DATATABLE.Rows.Count + UPDATE_FORMULA_DATATABLE.Rows.Count + UPDATE_INVENTORY_DATATABLE.Rows.Count + UPDATE_PRICE_CHANGE_DATATABLE.Rows.Count + UPDATE_COUPON_APPROVAL_DATATABLE.Rows.Count + UPDATE_CUSTOM_PROD_APP_DATATABLE.Rows.Count + UPDATE_PARTNERS_DATATABLE.Rows.Count + UPDATE_COUPONS_DATATABLE.Rows.Count
                                ProgressBar1.Maximum = TotalRows
                                AuditTrail.LogToAuditTrail("System", "Loading: Update Detected, ", "Normal")
                                BackgroundWorkerInstallUpdates.WorkerReportsProgress = True
                                BackgroundWorkerInstallUpdates.WorkerSupportsCancellation = True
                                BackgroundWorkerInstallUpdates.RunWorkerAsync()
                            Else
                                If IfNeedsToReset Then
                                    BackgroundWorker2.WorkerSupportsCancellation = True
                                    BackgroundWorker2.WorkerReportsProgress = True
                                    BackgroundWorker2.RunWorkerAsync()
                                Else
                                    If S_Layout = "" Then
                                        ChooseLayout.Show()
                                        Close()
                                    Else
                                        GetLocalPosData()
                                    End If
                                End If
                            End If
                        Else
                            If IfNeedsToReset Then
                                BackgroundWorker2.WorkerSupportsCancellation = True
                                BackgroundWorker2.WorkerReportsProgress = True
                                BackgroundWorker2.RunWorkerAsync()
                            Else
                                If S_Layout = "" Then
                                    ChooseLayout.Show()
                                    Close()
                                Else
                                    GetLocalPosData()
                                End If
                            End If
                        End If
                    Else
                        If My.Settings.Auto_Update Then
                            If UPDATE_CATEGORY_DATATABLE.Rows.Count > 0 Or UPDATE_PRODUCTS_DATATABLE.Rows.Count > 0 Or
                                UPDATE_FORMULA_DATATABLE.Rows.Count > 0 Or UPDATE_INVENTORY_DATATABLE.Rows.Count > 0 Or
                                UPDATE_PRICE_CHANGE_DATATABLE.Rows.Count > 0 Or UPDATE_COUPON_APPROVAL_DATATABLE.Rows.Count > 0 Or
                                UPDATE_CUSTOM_PROD_APP_DATATABLE.Rows.Count Or UPDATE_COUPONS_DATATABLE.Rows.Count > 0 Or UPDATE_PARTNERS_DATATABLE.Rows.Count > 0 Then

                                ProgressBar1.Value = 0
                                ProgressBar1.Maximum = 0
                                Dim TotalRows = UPDATE_CATEGORY_DATATABLE.Rows.Count + UPDATE_PRODUCTS_DATATABLE.Rows.Count + UPDATE_FORMULA_DATATABLE.Rows.Count + UPDATE_INVENTORY_DATATABLE.Rows.Count + UPDATE_PRICE_CHANGE_DATATABLE.Rows.Count + UPDATE_COUPON_APPROVAL_DATATABLE.Rows.Count + UPDATE_CUSTOM_PROD_APP_DATATABLE.Rows.Count + UPDATE_PARTNERS_DATATABLE.Rows.Count + UPDATE_COUPONS_DATATABLE.Rows.Count
                                ProgressBar1.Maximum = TotalRows

                                BackgroundWorkerInstallUpdates.WorkerReportsProgress = True
                                BackgroundWorkerInstallUpdates.WorkerSupportsCancellation = True
                                BackgroundWorkerInstallUpdates.RunWorkerAsync()
                            Else
                                If IfNeedsToReset Then
                                    BackgroundWorker2.WorkerSupportsCancellation = True
                                    BackgroundWorker2.WorkerReportsProgress = True
                                    BackgroundWorker2.RunWorkerAsync()
                                Else
                                    NoInternetConnection()
                                End If
                            End If
                        Else
                            If IfNeedsToReset Then
                                BackgroundWorker2.WorkerSupportsCancellation = True
                                BackgroundWorker2.WorkerReportsProgress = True
                                BackgroundWorker2.RunWorkerAsync()
                            Else
                                NoInternetConnection()
                            End If
                        End If
                    End If
                Else
                    NotYetActivated()
                End If
            Else
                ConnectionIsClose()
            End If
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub BackgroundWorker2_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker2.DoWork
        Try
            ProgressBar1.Value = 0
            thread = New Thread(AddressOf Temptinventory)
            thread.Start()
            threadList.Add(thread)
            thread = New Thread(AddressOf ResetStocks)
            thread.Start()
            threadList.Add(thread)
            For i = 0 To 100
                BackgroundWorker2.ReportProgress(i)
                Thread.Sleep(50)
                If i = 0 Then
                    Label1.Text = "Performing inventory reset."
                ElseIf i = 20 Then
                    Label1.Text = "Performing inventory reset.."
                ElseIf i = 40 Then
                    Label1.Text = "Performing inventory reset..."
                ElseIf i = 60 Then
                    Label1.Text = "Performing inventory reset."
                ElseIf i = 80 Then
                    Label1.Text = "Performing inventory reset.."
                ElseIf i = 100 Then
                    Label1.Text = "Performing inventory reset..."
                End If
            Next
            For Each t In threadList
                t.Join()
            Next
            If BackgroundWorker2.CancellationPending Then
                e.Cancel = True
            End If
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
        End Try
    End Sub
    '===========================================================================================
    Private Sub NotYetActivated()
        Try
            ChangeProgBarColor(ProgressBar1, ProgressBarColor.Yellow)
            Dim result As Integer = MessageBox.Show("Your POS system is not yet activated. Would you like to activate the software now ?", "Activation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.Yes Then
                Dispose()
                ConfigManager.Show()
            Else
                Application.Exit()
            End If
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub ConnectionIsClose()
        Try
            Dim msg2 As Integer = MessageBox.Show("Would you like to setup server configuration?", "Setup", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
            If msg2 = DialogResult.Yes Then
                ConfigManager.Show()
                Close()
            ElseIf msg2 = DialogResult.No Then
                Application.Exit()
            End If
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub NoInternetConnection()
        Try
            Dim msg As Integer = MessageBox.Show("No internet connection found, Would you like to continue ?", "No internet connection", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
            If msg = DialogResult.Yes Then
                If S_Layout = "" Then
                    ChooseLayout.Show()
                    Close()
                Else
                    GetLocalPosData()
                End If
            ElseIf msg = DialogResult.No Then
                Application.Exit()
            End If
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub GetLocalPosData()
        Try
            Dim sql = "SELECT * FROM admin_outlets WHERE store_id = " & ClientStoreID & ";"
            Dim cmd As MySqlCommand = New MySqlCommand(sql, LocalhostConn)
            Dim dr As MySqlDataReader = cmd.ExecuteReader()
            While dr.Read()
                ClientBrand = dr("brand_name")
                ClientLocation = dr("location_name")
                ClientPostalCode = dr("postal_code")
                ClientAddress = dr("address")
                ClientBrgy = dr("Barangay")
                ClientMunicipality = dr("municipality")
                ClientProvince = dr("province")
                ClientTin = dr("tin_no")
                ClientTel = dr("tel_no")
                ClientStorename = dr("store_name")
                ClientMIN = dr("MIN")
                ClientMSN = dr("MSN")
                ClientPTUN = dr("PTUN")
                getmunicipality = dr("municipality_name")
                getprovince = dr("province_name")
            End While
            cmd.Dispose()
            Dispose()
            Login.Show()
            Login.Focus()
            Login.txtusername.Focus()
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub Temptinventory()
        Try
            Dim ConnectionLocal As MySqlConnection = LocalhostConn()
            Using mCmd = New MySqlCommand("", ConnectionLocal)
                mCmd.CommandText = "INSERT INTO loc_inv_temp_data (`store_id`, `formula_id`, `product_ingredients`, `sku`, `stock_primary`, `stock_secondary`, `stock_no_of_servings`, `stock_status`, `critical_limit`, `guid`, `created_at`)  SELECT `store_id`, `formula_id`, `product_ingredients`, `sku`, `stock_primary`, `stock_secondary`, `stock_no_of_servings`, `stock_status`, `critical_limit`, `guid` ,(SELECT date_add(date_add(LAST_DAY(NOW()),interval 1 DAY),interval -1 MONTH) AS first_day) FROM loc_pos_inventory"
                mCmd.ExecuteNonQuery()
                mCmd.Dispose()
            End Using
            ConnectionLocal.Close()
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub ResetStocks()
        Try
            Dim ConnectionLocal As MySqlConnection = LocalhostConn()
            Using mCmd = New MySqlCommand("", ConnectionLocal)
                mCmd.CommandText = "UPDATE `loc_pos_inventory` SET `stock_primary`= 0,`stock_secondary`= 0"
                mCmd.ExecuteNonQuery()
                mCmd.Dispose()
            End Using
            ConnectionLocal.Close()
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub BackgroundWorker2_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker2.ProgressChanged
        ProgressBar1.Value = e.ProgressPercentage
        Label2.Text = e.ProgressPercentage
    End Sub
    Private Sub BackgroundWorker2_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker2.RunWorkerCompleted
        If S_Layout = "" Then
            ChooseLayout.Show()
            Close()
        Else
            GetLocalPosData()
        End If
        Dispose()
        Login.Show()
        Login.Focus()
        Login.txtusername.Focus()
    End Sub
#Region "Script Runner"
    Private Function FillScript() As List(Of ScriptRunnerCls)
        Dim scriptCls As New List(Of ScriptRunnerCls)
        Try
            Dim ConnectionLocal As MySqlConnection = LocalhostConn()
            Using mCmd = New MySqlCommand("", ConnectionLocal)
                mCmd.CommandText = "SELECT created_at FROM loc_script_runner"
                Using reader = mCmd.ExecuteReader
                    While reader.Read
                        Dim script As New ScriptRunnerCls
                        script.CreatedAt = reader("created_at").ToString
                        scriptCls.Add(script)
                    End While
                    reader.Dispose()
                End Using
                mCmd.Dispose()
            End Using
            ConnectionLocal.Close()
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
        End Try
        Return scriptCls
    End Function
    Private Sub RunScript()
        Try
            If ListOfScripts.Count > 0 Then
                Dim ConnectionLocal As MySqlConnection = LocalhostConn()
                Dim ConnectionCloud As MySqlConnection = ServerCloudCon()
                Dim dateRes As String = ""

                For Each dts As ScriptRunnerCls In ListOfScripts
                    If dts Is ListOfScripts.Last Then
                        dateRes &= $"'{dts.CreatedAt}'"
                    Else
                        dateRes &= $"'{dts.CreatedAt}',"
                    End If
                Next

                Using mCmd = New MySqlCommand("", ConnectionCloud)
                    mCmd.CommandText = $"SELECT * FROM admin_script_runner WHERE created_at NOT IN ({dateRes}) AND store_id IN ('All', '{ClientStoreID}')"
                    Dim dt As New DataTable

                    Using reader = mCmd.ExecuteReader
                        dt.Load(reader)
                        reader.Dispose()
                    End Using

                    For Each row As DataRow In dt.Rows
                        Using lmCmd = New MySqlCommand("", ConnectionLocal)
                            lmCmd.CommandText = row("script_command").ToString
                            lmCmd.ExecuteNonQuery()
                            If row("truncatescript").ToString = "NO" Then
                                lmCmd.CommandText = "INSERT INTO loc_script_runner (script_command, created_at, active) VALUES (@script_command, @createdAt, @active)"
                                lmCmd.Parameters.Clear()
                                lmCmd.Parameters.AddWithValue("@script_command", row("script_command"))
                                lmCmd.Parameters.AddWithValue("@createdAt", row("created_at"))
                                lmCmd.Parameters.AddWithValue("@active", row("active"))
                                lmCmd.ExecuteNonQuery()
                            End If
                            lmCmd.Dispose()
                        End Using

                        AuditTrail.LogToAuditTrail("Script", "Script: " & row("script_command").ToString, "Normal")
                    Next
                    mCmd.Dispose()
                End Using
                ConnectionLocal.Close()
                ConnectionCloud.Close()
            Else
                Dim ConnectionLocal As MySqlConnection = LocalhostConn()
                Dim ConnectionCloud As MySqlConnection = ServerCloudCon()

                Using mCmd = New MySqlCommand("", ConnectionCloud)
                    mCmd.CommandText = $"SELECT * FROM admin_script_runner WHERE store_id IN ('All', '{ClientStoreID}')"
                    Dim dt As New DataTable

                    Using reader = mCmd.ExecuteReader
                        dt.Load(reader)
                        reader.Dispose()
                    End Using

                    For Each row As DataRow In dt.Rows
                        Using lmCmd = New MySqlCommand("", ConnectionLocal)
                            lmCmd.CommandText = row("script_command").ToString
                            lmCmd.ExecuteNonQuery()
                            If row("truncatescript").ToString = "NO" Then
                                lmCmd.CommandText = "INSERT INTO loc_script_runner (script_command, created_at, active) VALUES (@script_command, @createdAt, @active)"
                                lmCmd.Parameters.Clear()
                                lmCmd.Parameters.AddWithValue("@script_command", row("script_command"))
                                lmCmd.Parameters.AddWithValue("@createdAt", row("created_at"))
                                lmCmd.Parameters.AddWithValue("@active", row("active"))
                                lmCmd.ExecuteNonQuery()
                            End If
                            lmCmd.Dispose()
                        End Using

                        AuditTrail.LogToAuditTrail("Script", "Script: " & row("script_command").ToString, "Normal")
                    Next
                    mCmd.Dispose()
                End Using
                ConnectionLocal.Close()
                ConnectionCloud.Close()
            End If
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
        End Try
    End Sub

#End Region

#Region "UPDATES"
    Dim ThreadUpdate As Thread
    Dim THREADLISTUPDATE As List(Of Thread) = New List(Of Thread)
    Private Sub BackgroundWorkerInstallUpdates_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorkerInstallUpdates.DoWork
        Try
            If ValidLocalConnection Then
                ThreadUpdate = New Thread(Sub() InstallUpdatesFormula(0))
                ThreadUpdate.Start()
                THREADLISTUPDATE.Add(ThreadUpdate)
                ThreadUpdate = New Thread(Sub() InstallUpdatesInventory(0))
                ThreadUpdate.Start()
                THREADLISTUPDATE.Add(ThreadUpdate)
                ThreadUpdate = New Thread(Sub() InstallUpdatesCategory(0))
                ThreadUpdate.Start()
                THREADLISTUPDATE.Add(ThreadUpdate)
                ThreadUpdate = New Thread(Sub() InstallUpdatesCoupons(0))
                ThreadUpdate.Start()
                THREADLISTUPDATE.Add(ThreadUpdate)
                ThreadUpdate = New Thread(Sub() InstallUpdatesProducts(0))
                ThreadUpdate.Start()
                THREADLISTUPDATE.Add(ThreadUpdate)
                ThreadUpdate = New Thread(Sub() InstallUpdatesPriceChange())
                ThreadUpdate.Start()
                THREADLISTUPDATE.Add(ThreadUpdate)
                ThreadUpdate = New Thread(Sub() InstallCoupons())
                ThreadUpdate.Start()
                THREADLISTUPDATE.Add(ThreadUpdate)
                ThreadUpdate = New Thread(Sub() InstallProducts())
                ThreadUpdate.Start()
                THREADLISTUPDATE.Add(ThreadUpdate)
                ThreadUpdate = New Thread(Sub() InstallUpdatesPartners(0))
                ThreadUpdate.Start()
                THREADLISTUPDATE.Add(ThreadUpdate)
                For Each t In THREADLISTUPDATE
                    t.Join()
                    If (BackgroundWorkerInstallUpdates.CancellationPending) Then
                        e.Cancel = True
                        UPDATE_WORKER_CANCEL = True
                        Exit Sub
                    End If
                Next
            End If

        Catch ex As Exception
            ValidCloudConnection = False
            BackgroundWorkerInstallUpdates.CancelAsync()
            If UPDATE_WORKER_CANCEL Then
                MsgBox("Cannot fetch data. Please check your internet connection")
            End If
            AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
            Exit Sub
        End Try
    End Sub

    Private Sub BackgroundWorkerInstallUpdates_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorkerInstallUpdates.RunWorkerCompleted
        AuditTrail.LogToAuditTrail("System", "Loading: Update successful, ", "Normal")

        If S_Layout = "" Then
            ChooseLayout.Show()
            Close()
        Else
            GetLocalPosData()
        End If
        Dispose()
        Login.Show()
        Login.Focus()
        Login.txtusername.Focus()
    End Sub
#End Region
End Class
