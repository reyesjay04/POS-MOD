﻿Imports MySql.Data.MySqlClient
Imports System.Threading
Imports System.IO
Imports System.Text
Imports System.Globalization
Imports System.Drawing.Imaging
'Requirements
'my.settings.validlocalconn/cloudconn = 1
'franchiseeacc = true/ accountexist = true
Public Class ConfigManager
    Dim BGWIdentifyer As Integer = 0
    Dim thread1 As Thread
    Property FranchiseeStoreValidation As Boolean

    Dim BTNSaveLocalConn As Boolean = False
    Dim BTNSaveCloudConn As Boolean = False
    Dim Autobackup As Boolean = False
    Dim ConfirmAdditionalSettings As Boolean = False
    Dim ConfirmDevInfoSettings As Boolean = False
    Dim POSVersion As String = ""
    Dim LOCALCONNDATA As Boolean = False
    Dim CLOUDCONDATA As Boolean = False

    Dim TestModeIsOFF As Boolean = True
    Property listOfProducts As New List(Of ProductsCls)
    Property listOfReceiptInfo As New List(Of ReceiptInfoCls)
    Property MasterList As New MasterlistCls
    Property listOfOutlets As New List(Of OutletsCls)
    Property MasterOutletInfo As New MasterlistOutletCls
    Property AccountExist As Boolean
    Property ValidProductKey As Boolean
    Property SyncFrom As DateTime
    Property SyncTo As DateTime
    Private Sub ConfigManager_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckForIllegalCrossThreadCalls = False
        TabControl1.TabPages(0).Text = "General Settings"
        TabControl1.TabPages(1).Text = "License And Activation"
        TabControl2.TabPages(0).Text = "Connection Settings"
        TabControl2.TabPages(1).Text = "Additional Settings"
        TabControl3.TabPages(0).Text = "Account/ Store Settings"
        TabControl3.TabPages(1).Text = "Activation"
        If File.Exists(My.Settings.LocalConnectionPath) Then
            BackgroundWorkerLOAD.WorkerReportsProgress = True
            BackgroundWorkerLOAD.WorkerReportsProgress = True
            BackgroundWorkerLOAD.RunWorkerAsync()
        Else
            ValidCloudConnection = False
            ValidLocalConnection = False
        End If
    End Sub
    Private Function TestLocalConnection()
        Dim Conn As MySqlConnection = New MySqlConnection
        Try
            Conn.ConnectionString = "server=" & Trim(TextBoxLocalServer.Text) &
                ";user id= " & Trim(TextBoxLocalUsername.Text) &
                ";password=" & Trim(TextBoxLocalPassword.Text) &
                ";database=" & Trim(TextBoxLocalDatabase.Text) &
                ";port=" & Trim(TextBoxLocalPort.Text)
            Conn.Open()
            If Conn.State = ConnectionState.Open Then
                ValidLocalConnection = True
                LOCALCONNDATA = True
            End If
        Catch ex As Exception
            ValidLocalConnection = False
            LOCALCONNDATA = False
        End Try
        Return Conn
    End Function
    Private Function TestCloudConnection()
        Dim cloudconn As MySqlConnection = New MySqlConnection
        Try
            cloudconn.ConnectionString = "server=" & Trim(TextBoxCloudServer.Text) &
            ";user id= " & Trim(TextBoxCloudUsername.Text) &
            ";password=" & Trim(TextBoxCloudPassword.Text) &
            ";database=" & Trim(TextBoxCloudDatabase.Text) &
            ";port=" & Trim(TextBoxCloudPort.Text)
            cloudconn.Open()
            If cloudconn.State = ConnectionState.Open Then
                ValidCloudConnection = True
                CLOUDCONDATA = True
            End If
        Catch ex As Exception
            ValidCloudConnection = False
            CLOUDCONDATA = False
        End Try
        Return cloudconn
    End Function
    Private Sub CreateFolder(Path As String, FolderName As String, Optional ByVal Attributes As System.IO.FileAttributes = IO.FileAttributes.Normal)
        Try
            My.Computer.FileSystem.CreateDirectory(Path & "\" & FolderName)
            If Not Attributes = IO.FileAttributes.Normal Then
                My.Computer.FileSystem.GetDirectoryInfo(Path & "\" & FolderName).Attributes = Attributes
            End If
            CreateUserConfig(Path, "user.config", FolderName)
        Catch ex As Exception
        End Try
    End Sub
    Public Sub CreateUserConfig(path As String, FileName As String, FolderName As String, Optional ByVal Attributes As System.IO.FileAttributes = IO.FileAttributes.Normal)
        Try
            Dim CompletePath As String = path & "\" & FolderName & "\" & "user.config"
            My.Computer.FileSystem.CreateDirectory(path & "\" & FolderName)
            If Not Attributes = IO.FileAttributes.Normal Then
                My.Computer.FileSystem.GetDirectoryInfo(path & "\" & FolderName).Attributes = Attributes
            End If
            Dim ConnString(5) As String
            ConnString(0) = "server=" & ConvertToBase64(Trim(TextBoxLocalServer.Text))
            ConnString(1) = "user id=" & ConvertToBase64(Trim(TextBoxLocalUsername.Text))
            ConnString(2) = "password=" & ConvertToBase64(Trim(TextBoxLocalPassword.Text))
            ConnString(3) = "database=" & ConvertToBase64(Trim(TextBoxLocalDatabase.Text))
            ConnString(4) = "port=" & ConvertToBase64(Trim(TextBoxLocalPort.Text))
            ConnString(5) = "Allow Zero Datetime=True"
            File.WriteAllLines(CompletePath, ConnString, Encoding.UTF8)
            CreateConn(CompletePath)
            My.Settings.LocalConnectionPath = CompletePath
            My.Settings.Save()
            If LOCALCONNDATA = False Then
                MsgBox("Saved")
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub ButtonTestLocConn_Click(sender As Object, e As EventArgs) Handles ButtonTestLocConn.Click
        Try
            TextboxEnableability(Panel5, False)
            ButtonEnableability(Panel5, False)
            BackgroundWorker1.WorkerSupportsCancellation = True
            BackgroundWorker1.WorkerReportsProgress = True
            BackgroundWorker1.RunWorkerAsync()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub ButtonClearLocal_Click(sender As Object, e As EventArgs) Handles ButtonClearLocal.Click
        ClearTextBox(Panel5)
        ValidLocalConnection = False
    End Sub
    Private Sub Button8_Click_1(sender As Object, e As EventArgs) Handles ButtonEditLocal.Click
        Try
            BTNSaveLocalConn = False
            TextboxEnableability(Panel5, True)
            ButtonClearLocal.Enabled = True
            ButtonTestLocConn.Enabled = True
        Catch ex As Exception

        End Try
    End Sub
    Private Sub ButtonClearCloud_Click(sender As Object, e As EventArgs) Handles ButtonClearCloud.Click
        ClearTextBox(Panel9)
    End Sub
    Dim threadListConLocal As List(Of Thread) = New List(Of Thread)
    Dim threadconlocal As Thread
    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Try
            For i = 0 To 100
                LabelLocal.Text = "Checking Connection " & i & " %"
                BackgroundWorker1.ReportProgress(i)
                Thread.Sleep(50)
                If i = 10 Then
                    threadconlocal = New Thread(AddressOf TestLocalConnection)
                    threadconlocal.Start()
                    threadListConLocal.Add(threadconlocal)
                End If
            Next
            For Each t In threadListConLocal
                t.Join()
            Next
        Catch ex As Exception

        End Try
    End Sub
    Private Sub BackgroundWorker1_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        ProgressBar1.Value = e.ProgressPercentage
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        If ValidLocalConnection = False Then
            ChangeProgBarColor(ProgressBar1, ProgressBarColor.Yellow)
            LabelLocal.Text = "Invalid connection please try again."
        Else
            ChangeProgBarColor(ProgressBar1, ProgressBarColor.Green)
            LabelLocal.Text = "Connected successfully!"

            ButtonSaveLocalCon.PerformClick()
        End If
        TextboxEnableability(Panel5, True)
        ButtonEnableability(Panel5, True)
    End Sub

    Private Sub ButtonTestCloudConn_Click(sender As Object, e As EventArgs) Handles ButtonTestCloudConn.Click
        Try
            TextboxEnableability(Panel9, False)
            ButtonEnableability(Panel9, False)
            BackgroundWorker2.WorkerSupportsCancellation = True
            BackgroundWorker2.WorkerReportsProgress = True
            BackgroundWorker2.RunWorkerAsync()
        Catch ex As Exception

        End Try
    End Sub
    Dim threadListConCloud As List(Of Thread) = New List(Of Thread)
    Dim threadcloud As Thread
    Private Sub BackgroundWorker2_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker2.DoWork
        Try
            For i = 0 To 100
                LabelCloud.Text = "Checking Connection " & i & " %"
                BackgroundWorker2.ReportProgress(i)
                Thread.Sleep(50)
                If i = 10 Then
                    threadcloud = New Thread(AddressOf TestCloudConnection)
                    threadcloud.Start()
                    threadListConCloud.Add(threadcloud)
                End If
            Next
            For Each t In threadListConCloud
                t.Join()
            Next
        Catch ex As Exception

        End Try
    End Sub
    Private Sub BackgroundWorker2_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker2.ProgressChanged
        ProgressBar2.Value = e.ProgressPercentage
    End Sub
    Private Sub BackgroundWorker2_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker2.RunWorkerCompleted
        If ValidCloudConnection = False Then
            ChangeProgBarColor(ProgressBar2, ProgressBarColor.Yellow)
            LabelCloud.Text = "Invalid connection please try again."
        Else
            ChangeProgBarColor(ProgressBar2, ProgressBarColor.Green)
            LabelCloud.Text = "Connected successfully!"
        End If
        TextboxEnableability(Panel9, True)
        ButtonEnableability(Panel9, True)
    End Sub
    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles ButtonEditCloud.Click
        Try
            TextboxEnableability(Panel9, True)
            ButtonClearCloud.Enabled = True
            ButtonTestCloudConn.Enabled = True
        Catch ex As Exception

        End Try
    End Sub
    Dim FillUp As Boolean = False
    Private Sub ButtonSaveCloudConn_Click(sender As Object, e As EventArgs) Handles ButtonSaveCloudConn.Click
        Try
            table = "loc_settings"
            where = "settings_id = 1"
            If ValidLocalConnection = True Then
                If ValidCloudConnection = True Then
                    Dim ConnectionLocal As MySqlConnection = TestLocalConnection()
                    fields = "C_Server, C_Username, C_Password, C_Database, C_Port"
                    Dim sql = "Select " & fields & " FROM " & table & " WHERE " & where
                    Dim cmd As MySqlCommand = New MySqlCommand(sql, ConnectionLocal)
                    Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
                    Dim dt As DataTable = New DataTable
                    da.Fill(dt)
                    If dt.Rows.Count > 0 Then
                        fields = "C_Server = '" & ConvertToBase64(Trim(TextBoxCloudServer.Text)) & "', C_Username = '" & ConvertToBase64(Trim(TextBoxCloudUsername.Text)) & "', C_Password = '" & ConvertToBase64(Trim(TextBoxCloudPassword.Text)) & "', C_Database = '" & ConvertToBase64(Trim(TextBoxCloudDatabase.Text)) & "', C_Port = '" & ConvertToBase64(Trim(TextBoxCloudPort.Text)) & "'"
                        sql = "UPDATE " & table & " SET " & fields & " WHERE " & where
                        cmd = New MySqlCommand(sql, ConnectionLocal)
                        cmd.ExecuteNonQuery()
                        If CLOUDCONDATA = False Then
                            MsgBox("Saved!")
                        End If
                    Else
                        fields = "(C_Server, C_Username, C_Password, C_Database, C_Port, S_Zreading)"
                        value = "('" & ConvertToBase64(Trim(TextBoxCloudServer.Text)) & "'
                     ,'" & ConvertToBase64(Trim(TextBoxCloudUsername.Text)) & "'
                     ,'" & ConvertToBase64(Trim(TextBoxCloudPassword.Text)) & "'
                     ,'" & ConvertToBase64(Trim(TextBoxCloudDatabase.Text)) & "'
                     ,'" & ConvertToBase64(Trim(TextBoxCloudPort.Text)) & "'
                     ,'" & Format(Now(), "yyyy-MM-dd") & "')"
                        sql = "INSERT INTO " & table & " " & fields & " VALUES " & value
                        cmd = New MySqlCommand(sql, ConnectionLocal)
                        cmd.ExecuteNonQuery()
                        If CLOUDCONDATA = False Then
                            MsgBox("Saved!")
                        End If
                    End If
                    LoadPrintOptions()
                    LoadDefaultSettingsAdd()
                    LoadDefaultSettingsDev()
                    TextboxEnableability(Panel9, False)
                    BTNSaveCloudConn = True
                    ButtonClearCloud.Enabled = False
                    ButtonTestCloudConn.Enabled = False
                    FillUp = True
                    SaveDevInfo()
                    SaveAddSettings()
                    ConnectionLocal.Close()
                Else
                    FillUp = False
                    BTNSaveCloudConn = False
                    MsgBox("Connection must be valid")
                End If
            Else
                MsgBox("Local connection must be valid first.")
            End If

        Catch ex As Exception

        End Try
    End Sub
    Dim FooterInfo As String = ""
    Private Sub LoadPrintOptions()
        Try
            If ValidLocalConnection Then
                Dim ConnectionLocal As MySqlConnection = TestLocalConnection()
                Dim sql = "SELECT `printreceipt`, `reprintreceipt`, `printxzread`, `printreturns`, `printsalesreport` FROM loc_settings WHERE settings_id = 1"
                Dim cmd As MySqlCommand = New MySqlCommand(sql, ConnectionLocal)
                Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
                Dim dt As DataTable = New DataTable
                da.Fill(dt)
                If dt.Rows.Count > 0 Then
                    If dt(0)(0) <> Nothing Then
                        If dt(0)(0) = "YES" Then
                            PrintOption = "YES"
                            RadioButtonPrintReceiptYes.Checked = True
                        Else
                            PrintOption = "NO"
                            RadioButtonPrintReceiptNo.Checked = True
                        End If
                        PrintOptionIsSet = True
                    Else
                        PrintOptionIsSet = False
                    End If
                    If dt(0)(1) <> Nothing Then
                        If dt(0)(1) = "YES" Then
                            RePrintOption = "YES"
                            RadioButtonRePrintReceiptYes.Checked = True
                        Else
                            RePrintOption = "NO"
                            RadioButtonRePrintReceiptNo.Checked = True
                        End If
                        RePrintOptionIsSet = True
                    Else
                        RePrintOptionIsSet = False
                    End If
                    If dt(0)(2) <> Nothing Then
                        If dt(0)(2) = "YES" Then
                            PrintXZReadOption = "YES"
                            RadioButtonPrintXZReadYes.Checked = True
                        Else
                            PrintXZReadOption = "NO"
                            RadioButtonPrintXZReadNo.Checked = True
                        End If
                        PrintXZRead = True
                    Else
                        PrintXZRead = False
                    End If
                    If dt(0)(3) <> Nothing Then
                        If dt(0)(3) = "YES" Then
                            PrintReturns = "YES"
                            RadioButtonPrintReturnsYes.Checked = True
                        Else
                            PrintReturns = "NO"
                            RadioButtonPrintReturnsNo.Checked = True
                        End If
                        PrintReturnsBool = True
                    Else
                        PrintReturnsBool = False
                    End If

                    If dt(0)(4) <> Nothing Then
                        If dt(0)(4) = "YES" Then
                            PrintSalesReport = "YES"
                            RadioButtonPrintSRYes.Checked = True
                        Else
                            PrintSalesReport = "NO"
                            RadioButtonPrintSRNo.Checked = True
                        End If
                        PrintSalesReportBool = True
                    Else
                        PrintSalesReportBool = False
                    End If

                End If
            End If

        Catch ex As Exception

            AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub LoadDefaultSettingsAdd()
        Try
            If ValidCloudConnection = True And ValidLocalConnection = True Then
                If File.Exists(My.Settings.LocalConnectionPath) Then
                    Dim EXPORTPATH = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\Innovention"
                    Dim ConnectionCloud As MySqlConnection = TestCloudConnection()
                    Dim sql = "SELECT `A_Tax`, `A_SIFormat`, `A_Terminal_No`, `A_ZeroRated`, `S_Batter`, `S_Brownie_Mix`, `S_Upgrade_Price_Add` , `S_Update_Version` , `S_Waffle_Bag`, `S_Packets`, `P_Footer_Info`, `S_ZeroRated_Tax` FROM admin_settings_org WHERE settings_id = 1"
                    Dim cmd As MySqlCommand = New MySqlCommand(sql, ConnectionCloud)
                    Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
                    Dim dt As DataTable = New DataTable
                    da.Fill(dt)
                    If dt.Rows.Count > 0 Then
                        TextBoxExportPath.Text = EXPORTPATH
                        TextBoxTax.Text = dt(0)(0)
                        TextBoxSINumber.Text = dt(0)(1)
                        TextBoxTerminalNo.Text = dt(0)(2)
                        If dt(0)(3) = "0" Then
                            RadioButtonNO.Checked = True
                        ElseIf dt(0)(3) = "1" Then
                            RadioButtonYES.Checked = False
                        End If
                        TextBoxBATTERID.Text = dt(0)(4)
                        TextBoxBROWNIEID.Text = dt(0)(5)
                        TextBoxBROWNIEPRICE.Text = dt(0)(6)
                        My.Settings.Version = dt(0)(7)
                        My.Settings.Save()
                        POSVersion = dt(0)(7)
                        TextBoxWaffleBag.Text = dt(0)(8)
                        TextBoxSugarPackets.Text = dt(0)(9)
                        ConfirmAdditionalSettings = True
                        FooterInfo = dt(0)(10)
                        TextBoxZeroRatedTax.Text = dt(0)(11)
                        ConnectionCloud.Close()
                    Else
                        ConfirmAdditionalSettings = False
                    End If
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub LoadDefaultSettingsDev()
        Try
            If ValidCloudConnection = True And ValidLocalConnection = True Then
                Dim ConnectionCloud As MySqlConnection = TestCloudConnection()
                Dim sql = "SELECT `Dev_Company_Name`, `Dev_Address`, `Dev_Tin`, `Dev_Accr_No`, `Dev_Accr_Date_Issued`, `Dev_Accr_Valid_Until`, `Dev_PTU_No`, `Dev_PTU_Date_Issued`, `Dev_PTU_Valid_Until` FROM admin_settings_org WHERE settings_id = 1"
                Dim cmd As MySqlCommand = New MySqlCommand(sql, ConnectionCloud)
                Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
                Dim dt As DataTable = New DataTable
                da.Fill(dt)
                If dt.Rows.Count > 0 Then
                    TextBoxDevname.Text = dt(0)(0)
                    TextBoxDevAdd.Text = dt(0)(1)
                    TextBoxDevTIN.Text = dt(0)(2)
                    TextBoxDevAccr.Text = dt(0)(3)
                    DateTimePicker1ACCRDI.Text = dt(0)(4)
                    DateTimePicker2ACCRVU.Text = dt(0)(5)
                    TextBoxDEVPTU.Text = dt(0)(6)
                    DateTimePicker4PTUDI.Text = dt(0)(7)
                    DateTimePickerPTUVU.Text = dt(0)(8)
                    ConfirmDevInfoSettings = True
                Else
                    ConfirmDevInfoSettings = False
                End If
                ConnectionCloud.Close()
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub LoadConn()
        Try
            If My.Settings.LocalConnectionPath <> "" Then
                If File.Exists(My.Settings.LocalConnectionPath) Then
                    'The File exists 
                    Dim CreateConnString As String = ""
                    Dim filename As String = String.Empty
                    Dim TextLine As String = ""
                    Dim objReader As New System.IO.StreamReader(My.Settings.LocalConnectionPath)
                    Dim lineCount As Integer
                    Do While objReader.Peek() <> -1
                        TextLine = objReader.ReadLine()
                        If lineCount = 0 Then
                            TextBoxLocalServer.Text = ConvertB64ToString(RemoveCharacter(TextLine, "server="))
                        End If
                        If lineCount = 1 Then
                            TextBoxLocalUsername.Text = ConvertB64ToString(RemoveCharacter(TextLine, "user id="))
                        End If
                        If lineCount = 2 Then
                            TextBoxLocalPassword.Text = ConvertB64ToString(RemoveCharacter(TextLine, "password="))
                        End If
                        If lineCount = 3 Then
                            TextBoxLocalDatabase.Text = ConvertB64ToString(RemoveCharacter(TextLine, "database="))
                        End If
                        If lineCount = 4 Then
                            TextBoxLocalPort.Text = ConvertB64ToString(RemoveCharacter(TextLine, "port="))
                        End If
                        lineCount = lineCount + 1
                    Loop
                    objReader.Close()
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub
    Public Sub LoadCloudConn()
        Try
            If ValidLocalConnection = True Then
                Dim ConnectionLocal As MySqlConnection = TestLocalConnection()
                Dim sql = "SELECT C_Server, C_Username, C_Password, C_Database, C_Port FROM loc_settings WHERE settings_id = 1"
                Dim cmd As MySqlCommand = New MySqlCommand(sql, ConnectionLocal)
                Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
                Dim dt As DataTable = New DataTable
                da.Fill(dt)
                If dt.Rows.Count > 0 Then
                    TextBoxCloudServer.Text = ConvertB64ToString(dt(0)(0))
                    TextBoxCloudUsername.Text = ConvertB64ToString(dt(0)(1))
                    TextBoxCloudPassword.Text = ConvertB64ToString(dt(0)(2))
                    TextBoxCloudDatabase.Text = ConvertB64ToString(dt(0)(3))
                    TextBoxCloudPort.Text = ConvertB64ToString(dt(0)(4))
                    ValidCloudConnection = True
                    CLOUDCONDATA = True
                Else
                    ValidCloudConnection = False
                    CLOUDCONDATA = False
                End If
                ConnectionLocal.Close()
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub LoadAutoBackup()
        Try
            If ValidLocalConnection = True Then
                Dim ConnectionLocal As MySqlConnection = TestLocalConnection()
                Dim sql = "SELECT `S_BackupInterval`, `S_BackupDate` FROM loc_settings WHERE settings_id = 1"
                Dim cmd As MySqlCommand = New MySqlCommand(sql, ConnectionLocal)
                Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
                Dim dt As DataTable = New DataTable
                da.Fill(dt)
                For Each row As DataRow In dt.Rows
                    If row("S_BackupInterval") <> "" Then
                        If row("S_BackupDate") <> "" Then
                            Autobackup = True
                            Dim interval = row("S_BackupInterval")
                            If interval = "1" Then
                                RadioButtonDaily.Checked = True
                            ElseIf interval = "2" Then
                                RadioButtonWeekly.Checked = True
                            ElseIf interval = "3" Then
                                RadioButtonMonthly.Checked = True
                            ElseIf interval = "4" Then
                                RadioButtonYearly.Checked = True
                            End If
                        Else
                            Autobackup = False
                            Exit For
                        End If
                    Else
                        Autobackup = False
                        Exit For
                    End If
                Next
                ConnectionLocal.Close()
            Else
                Autobackup = False
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub LoadAdditionalSettings()
        Try
            If ValidLocalConnection = True Then
                Dim ConnectionLocal As MySqlConnection = TestLocalConnection()
                Dim sql = "SELECT A_Export_Path, A_Tax, A_SIFormat, A_Terminal_No, A_ZeroRated FROM loc_settings WHERE settings_id = 1"
                Dim cmd As MySqlCommand = New MySqlCommand(sql, ConnectionLocal)
                Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
                Dim dt As DataTable = New DataTable
                da.Fill(dt)
                For Each row As DataRow In dt.Rows
                    If row("A_Export_Path") <> "" Then
                        If row("A_Tax") <> "" Then
                            If row("A_SIFormat") <> "" Then
                                If row("A_Terminal_No") <> "" Then
                                    If row("A_ZeroRated") <> "" Then
                                        TextBoxExportPath.Text = ConvertB64ToString(row("A_Export_Path"))
                                        TextBoxTax.Text = Val(row("A_Tax")) * 100
                                        TextBoxSINumber.Text = row("A_SIFormat")
                                        TextBoxTerminalNo.Text = row("A_Terminal_No")
                                        If Val(row("A_ZeroRated")) = 0 Then
                                            RadioButtonNO.Checked = True
                                        ElseIf dt(0)(4) = 1 Then
                                            RadioButtonYES.Checked = True
                                        End If
                                        ConfirmAdditionalSettings = True
                                    Else
                                        ConfirmAdditionalSettings = False
                                        Exit For
                                    End If
                                Else
                                    ConfirmAdditionalSettings = False
                                    Exit For
                                End If
                            Else
                                ConfirmAdditionalSettings = False
                                Exit For
                            End If
                        Else
                            ConfirmAdditionalSettings = False
                            Exit For
                        End If
                    Else
                        ConfirmAdditionalSettings = False
                        Exit For
                    End If
                Next
                ConnectionLocal.Close()
            Else
                ConfirmAdditionalSettings = False
            End If
            My.Settings.Save()
        Catch ex As Exception

        End Try
    End Sub
    Private Function LoadDevInfo() As DevInfoCls
        Dim DevInfo As New DevInfoCls
        Try
            If ValidLocalConnection Then
                Dim ConnectionLocal As MySqlConnection = TestLocalConnection()
                Dim sql = "SELECT Dev_Company_Name, Dev_Address, Dev_Tin, Dev_Accr_No, Dev_Accr_Date_Issued, Dev_Accr_Valid_Until, Dev_PTU_No, Dev_PTU_Date_Issued, Dev_PTU_Valid_Until FROM loc_settings WHERE settings_id = 1"
                Using cmd As New MySqlCommand(sql, ConnectionLocal)
                    Dim reader = cmd.ExecuteReader
                    If reader.HasRows Then
                        While reader.Read
                            With DevInfo
                                .Dev_Company_Name = reader("Dev_PTU_Valid_Until")
                                .Dev_Address = reader("Dev_PTU_Valid_Until")
                                .Dev_Tin = reader("Dev_PTU_Valid_Until")
                                .Dev_Accr_No = reader("Dev_PTU_Valid_Until")
                                .Dev_Accr_Date_Issued = reader("Dev_PTU_Valid_Until")
                                .Dev_Accr_Valid_Until = reader("Dev_PTU_Valid_Until")
                                .Dev_PTU_No = reader("Dev_PTU_Valid_Until")
                                .Dev_PTU_Date_Issued = reader("Dev_PTU_Valid_Until")
                                .Dev_PTU_Valid_Until = reader("Dev_PTU_Valid_Until")
                                ConfirmDevInfoSettings = True
                            End With
                        End While
                    Else
                        ConfirmDevInfoSettings = False
                    End If
                End Using
            End If
        Catch ex As Exception
            ConfirmDevInfoSettings = False
            MsgBox(ex.Message, MsgBoxStyle.Critical, "DEV INFO FETCH")
        End Try
        Return DevInfo
    End Function

    Private Function LoadOutlets() As List(Of OutletsCls)
        Dim OutletCls As New List(Of OutletsCls)
        Try
            Dim ConnectionCloud As MySqlConnection = TestCloudConnection()
            Dim sql = "SELECT * FROM admin_outlets WHERE user_id = @userID AND active = 1"
            Using CloudCmd As New MySqlCommand(sql, ConnectionCloud)
                CloudCmd.Parameters.Clear()
                CloudCmd.Parameters.AddWithValue("@userID", MasterList.user_id)
                Dim Reader = CloudCmd.ExecuteReader
                While Reader.Read
                    Dim Outlet As New OutletsCls With {
                        .StoreID = Reader("store_id"),
                        .BrandName = Reader("brand_name"),
                        .StoreName = Reader("store_name"),
                        .UserID = Reader("user_id"),
                        .Location = Reader("location_name"),
                        .Postal = Reader("postal_code"),
                        .Address = Reader("address"),
                        .Barangay = Reader("Barangay"),
                        .Municipality = Reader("municipality"),
                        .Province = Reader("province"),
                        .TinNumber = Reader("tin_no"),
                        .TelNumber = Reader("tel_no"),
                        .MIN = Reader("MIN"),
                        .MSN = Reader("MSN"),
                        .PTUN = Reader("PTUN")
                    }
                    OutletCls.Add(Outlet)
                End While
            End Using
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "OUTLET FETCH")
        End Try
        Return OutletCls
    End Function
    Private Sub GetLogo(BrandName)
        Try
            Dim ConnectionCloud As MySqlConnection = TestCloudConnection()
            Dim sql = "SELECT brand_logo FROM admin_brand WHERE brand_name = '" & BrandName & "' "
            Dim CloudCmd As MySqlCommand = New MySqlCommand(sql, TestCloudConnection)
            RichTextBoxLogo.Text = CloudCmd.ExecuteScalar()
            PictureBoxLogo.BackgroundImage = Base64ToImage(CloudCmd.ExecuteScalar())
            ConnectionCloud.Close()
        Catch ex As Exception

        End Try
    End Sub

    Public Sub CreateConn(path As String)
        Try
            Dim CreateConnString As String = ""
            Dim filename As String = String.Empty
            Dim TextLine As String = ""
            Dim objReader As New StreamReader(path)
            Dim lineCount As Integer
            Do While objReader.Peek() <> -1
                TextLine = objReader.ReadLine()
                If lineCount = 0 Then
                    CreateConnString += TextLine & ";"
                End If
                If lineCount = 1 Then
                    CreateConnString += TextLine & ";"
                End If
                If lineCount = 2 Then
                    CreateConnString += TextLine & ";"
                End If
                If lineCount = 3 Then
                    CreateConnString += TextLine & ";"
                End If
                If lineCount = 4 Then
                    CreateConnString += TextLine & ";"
                End If
                If lineCount = 5 Then
                    CreateConnString += TextLine
                End If
                lineCount = lineCount + 1
            Loop
            objReader.Close()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles ButtonExit.Click
        Application.Exit()

    End Sub
    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles ButtonValidateAccount.Click
        If ValidCloudConnection = True Then
            If String.IsNullOrWhiteSpace(TextBoxFrancUser.Text) Then
                MsgBox("All fields are required")
                'Pass
            Else
                If CheckForInternetConnection() = True Then
                    TextboxEnableability(Panel14, False)
                    ButtonEnableability(Panel14, False)
                    ClearTextBox(Panel15)
                    DataGridViewOutlets.Rows.Clear()
                    BackgroundWorker3.WorkerSupportsCancellation = True
                    BackgroundWorker3.WorkerReportsProgress = True
                    BackgroundWorker3.RunWorkerAsync()
                    DataGridViewOutlets.Focus()
                    DataGridViewOutlets.DataSource = Nothing
                Else
                    MsgBox("No Internet Connection")
                End If
            End If
        Else
            MsgBox("Cloud Server Connection must be valid first.")
        End If
    End Sub
    Dim threadList As List(Of Thread) = New List(Of Thread)
    Private Sub BackgroundWorker3_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker3.DoWork
        Try
            Dim isExist As Boolean = False
            For i = 0 To 100
                LabelAccCheck.Text = "Checking Account " & i & " %"
                BackgroundWorker3.ReportProgress(i)
                If i = 0 Then
                    thread1 = New Thread(Sub() isExist = checkacc())
                    thread1.Start()
                    threadList.Add(thread1)
                End If
                Thread.Sleep(20)
            Next
            For Each t In threadList
                t.Join()
            Next
            e.Result = isExist
        Catch ex As Exception

        End Try
    End Sub
    Private Function checkacc() As Boolean
        Dim isExist As Boolean = False
        Try
            FranchiseeStoreValidation = False
            MasterList = New MasterlistCls
            Dim ConnectionCloud As MySqlConnection = TestCloudConnection()
            Dim sql = "SELECT user_guid, user_id FROM admin_user WHERE user_name = @userName AND user_role = 'Client' AND status = 1; "
            Using cmd As New MySqlCommand(sql, ConnectionCloud)
                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("@userName", Trim(TextBoxFrancUser.Text))
                Dim reader = cmd.ExecuteReader
                If reader.HasRows Then
                    isExist = True
                    While reader.Read
                        MasterList.user_id = reader("user_id")
                    End While
                Else
                    isExist = False
                End If
                cmd.Dispose()
            End Using
            ConnectionCloud.Close()
        Catch ex As MySqlException
            isExist = False
        End Try
        Return isExist
    End Function
    Private Sub BackgroundWorker3_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker3.ProgressChanged
        ProgressBar3.Value = e.ProgressPercentage
    End Sub
    Private Sub BackgroundWorker3_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker3.RunWorkerCompleted
        If e.Result Then
            AccountExist = True
            BackgroundWorker4.WorkerSupportsCancellation = True
            BackgroundWorker4.WorkerReportsProgress = True
            BackgroundWorker4.RunWorkerAsync()

            TextboxEnableability(Panel14, False)
            ButtonEnableability(Panel14, True)
            ChangeProgBarColor(ProgressBar3, ProgressBarColor.Green)
            LabelAccCheck.Text = "Complete!"
        Else
            AccountExist = False
            ChangeProgBarColor(ProgressBar3, ProgressBarColor.Yellow)
            TextboxEnableability(Panel14, True)
            ButtonEnableability(Panel14, True)
            LabelAccCheck.Text = "Invalid franchisee's Account."
        End If
    End Sub
    Private Sub BackgroundWorker4_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker4.DoWork
        Try
            For i = 0 To 100
                BackgroundWorker4.ReportProgress(i)
                LabelAccCheck.Text = "Getting Account information " & i & " %"
                If i = 0 Then
                    thread1 = New Thread(Sub() listOfOutlets = LoadOutlets())
                    thread1.Start()
                    threadList.Add(thread1)
                End If
                Thread.Sleep(20)
            Next
            For Each t In threadList
                t.Join()
            Next
        Catch ex As Exception

        End Try
    End Sub
    Private Sub BackgroundWorker4_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker4.ProgressChanged
        ProgressBar3.Value = e.ProgressPercentage
    End Sub
    Private Sub BackgroundWorker4_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker4.RunWorkerCompleted
        For Each outlets As OutletsCls In listOfOutlets
            With outlets
                DataGridViewOutlets.Rows.Add(.StoreID, .BrandName, .StoreName, .UserID, .Location, .Postal, .Address, .Barangay, .Municipality, .Province, .TinNumber, .TelNumber, .MIN, .MSN, .PTUN)
            End With
        Next
    End Sub
    Private Sub DataGridViewOutlets_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridViewOutlets.CellClick
        ButtonSelectOutlet.PerformClick()
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles ButtonSelectOutlet.Click
        Try
            If DataGridViewOutlets.Rows.Count > 0 Then
                MasterOutletInfo = New MasterlistOutletCls
                Dim getOutletIndex = listOfOutlets.ToList.First(Function(x) x.StoreID = DataGridViewOutlets.SelectedRows(0).Cells(0).Value)
                With getOutletIndex
                    TextBoxBrandname.Text = .BrandName
                    TextBoxLocation.Text = .Location
                    TextBoxPostalCode.Text = .Postal
                    TextBoxAddress.Text = .Address
                    TextBoxMun.Text = .Municipality
                    TextBoxProv.Text = .Province
                    TextBoxTIN.Text = .TinNumber
                    TextBoxTEL.Text = .TelNumber
                    TextBoxPTUN.Text = .PTUN
                    TextBoxMIN.Text = .MIN
                    TextBoxMSN.Text = .MSN
                    TextBoxMunName.Text = ReturnMunicipalityName(.Municipality)
                    TextBoxProvName.Text = ReturnProvinceName(.Province)
                    FranchiseeStoreValidation = True
                    With MasterOutletInfo
                        .store_id = getOutletIndex.StoreID
                        .brand_name = getOutletIndex.BrandName
                        .store_name = getOutletIndex.StoreName
                        .user_id = MasterList.user_id
                        .location_name = getOutletIndex.Location
                        .postal_code = getOutletIndex.Postal
                        .address = getOutletIndex.Address
                        .Barangay = getOutletIndex.Barangay
                        .municipality = getOutletIndex.Municipality
                        .municipality_name = TextBoxMunName.Text
                        .province = getOutletIndex.Province
                        .province_name = TextBoxProvName.Text
                        .tin_no = getOutletIndex.TinNumber
                        .tel_no = getOutletIndex.TelNumber
                        .MIN = getOutletIndex.MIN
                        .MSN = getOutletIndex.MSN
                        .PTUN = getOutletIndex.PTUN
                    End With
                End With
                FranchiseeStoreValidation = True
                GetLogo(TextBoxBrandname.Text)
            Else
                FranchiseeStoreValidation = False
            End If
        Catch ex As Exception
        End Try
    End Sub
    Private Function ReturnMunicipalityName(id) As String
        Dim ServerConn As MySqlConnection = TestCloudConnection()
        Dim ReturnMun As String = ""
        Try
            Dim sql = "SELECT mn_name FROM admin_municipality WHERE mn_id = " & id & ""
            Dim cmd As MySqlCommand = New MySqlCommand(sql, ServerConn)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    While reader.Read
                        ReturnMun = reader("mn_name")
                    End While
                End If
            End Using
            cmd.Dispose()
            ServerConn.Close()
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
        End Try
        Return ReturnMun
    End Function
    Private Function ReturnProvinceName(id) As String
        Dim ServerConn As MySqlConnection = TestCloudConnection()
        Dim ReturnProv As String = ""
        Try
            Dim sql = "SELECT province FROM admin_province WHERE add_id = " & id & ""
            Dim cmd As MySqlCommand = New MySqlCommand(sql, ServerConn)
            Using reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    While reader.Read
                        ReturnProv = reader("province")
                    End While
                End If
            End Using
            ServerConn.Close()
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
        End Try
        Return ReturnProv
    End Function
    Private Sub RDButtons(tf As Boolean)
        Try
            RadioButtonNO.Enabled = tf
            RadioButtonYES.Enabled = tf
        Catch ex As Exception
        End Try
    End Sub
    Private Sub ButtonEdit_Click(sender As Object, e As EventArgs) Handles ButtonEditAddSettings.Click
        TextboxEnableability(GroupBox10, True)
        RDButtons(True)
        ButtonGetExportPath.Enabled = True
        FillUp = False
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles ButtonSaveAddSettings.Click
        FillUp = False
        SaveAddSettings()
    End Sub
    Private Sub SaveAddSettings()
        Try
            Dim RButton As Integer
            Dim Tax = Val(TextBoxTax.Text) / 100
            If Not TextboxIsEmpty(GroupBox10) Then
                If ValidLocalConnection = True Then
                    Dim ConnectionLocal As MySqlConnection = TestLocalConnection()
                    Dim table = "loc_settings"
                    Dim fields = "A_Export_Path, A_Tax, A_SIFormat, A_Terminal_No, A_ZeroRated, S_Zreading, S_Batter, S_Brownie_Mix, S_Upgrade_Price_Add, S_Update_Version, S_Waffle_Bag , S_Packets , P_Footer_Info, S_ZeroRated_Tax"
                    Dim where = "settings_id = 1"
                    Dim sql = "Select " & fields & " FROM " & table & " WHERE " & where
                    Dim cmd As MySqlCommand = New MySqlCommand(sql, ConnectionLocal)
                    Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
                    Dim dt As DataTable = New DataTable
                    da.Fill(dt)
                    If dt.Rows.Count > 0 Then
                        If RadioButtonYES.Checked = True Then
                            RButton = 1
                        ElseIf RadioButtonNO.Checked = True Then
                            RButton = 0
                        End If
                        Dim fields1 = "A_Export_Path = '" & ConvertToBase64(Trim(TextBoxExportPath.Text)) & "', A_Tax = '" & Tax & "' , A_SIFormat = '" & Trim(TextBoxSINumber.Text) & "' , A_Terminal_No = '" & Trim(TextBoxTerminalNo.Text) & "' , A_ZeroRated = '" & RButton & "', S_Zreading = '" & Format(Now(), "yyyy-MM-dd") & "' , S_Batter = '" & Trim(TextBoxBATTERID.Text) & "', S_Brownie_Mix = '" & Trim(TextBoxBROWNIEID.Text) & "', S_Upgrade_Price_Add = '" & Trim(TextBoxBROWNIEPRICE.Text) & "' , `S_Waffle_Bag` = '" & Trim(TextBoxWaffleBag.Text) & "' , `S_Packets` = '" & Trim(TextBoxSugarPackets.Text) & "' , S_Update_Version = '" & POSVersion & "', P_Footer_Info = '" & FooterInfo & "', printcount = 2, S_ZeroRated_Tax = '" & Trim(TextBoxZeroRatedTax.Text) & "'"
                        sql = "UPDATE " & table & " SET " & fields1 & " WHERE " & where
                        cmd = New MySqlCommand(sql, ConnectionLocal)
                        cmd.ExecuteNonQuery()
                        If FillUp = True Then
                        Else
                            MsgBox("Saved!")
                        End If
                    Else
                        Dim fields2 = "(A_Export_Path, A_Tax, A_SIFormat, A_Terminal_No, A_ZeroRated, S_Zreading, S_Batter, S_Brownie_Mix, S_Upgrade_Price_Add , S_Update_Version , S_Waffle_Bag , S_Packets, P_Footer_Info, printcount, S_ZeroRated_Tax)"
                        Dim value = "('" & ConvertToBase64(Trim(TextBoxExportPath.Text)) & "'
                     ,'" & Tax & "'
                     ,'" & Trim(TextBoxSINumber.Text) & "'
                     ,'" & Trim(TextBoxTerminalNo.Text) & "'
                     ,'" & RButton & "'
                     ,'" & Format(Now(), "yyyy-MM-dd") & "'
                     ,'" & Trim(TextBoxBATTERID.Text) & "'
                     ,'" & Trim(TextBoxBROWNIEID.Text) & "'
                     ,'" & Trim(TextBoxBROWNIEPRICE.Text) & "'
                     ,'" & POSVersion & "'
                     ,'" & Trim(TextBoxWaffleBag.Text) & "'
                     ,'" & Trim(TextBoxSugarPackets.Text) & "'
                     ,'" & FooterInfo & "'
                     ,2
                     ,'" & Trim(TextBoxZeroRatedTax.Text) & "')"

                        sql = "INSERT INTO " & table & " " & fields2 & " VALUES " & value
                        cmd = New MySqlCommand(sql, ConnectionLocal)
                        cmd.ExecuteNonQuery()
                        If FillUp = True Then
                        Else
                            MsgBox("Saved!")
                        End If
                    End If
                    ConfirmAdditionalSettings = True
                    TextboxEnableability(GroupBox10, False)
                    RDButtons(False)
                    ButtonGetExportPath.Enabled = False
                    ConnectionLocal.Close()
                Else
                    ConfirmAdditionalSettings = False
                    MsgBox("Invalid Local Connection.")
                End If
            Else
                ConfirmAdditionalSettings = False
                MsgBox("All fields are required.")
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles ButtonGetExportPath.Click
        If (FolderBrowserDialog1.ShowDialog() = DialogResult.OK) Then
            TextBoxExportPath.Text = FolderBrowserDialog1.SelectedPath
        End If
    End Sub
    Private Sub BackgroundWorkerLOAD_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorkerLOAD.DoWork
        Try
            Thread.Sleep(30)
            For i = 0 To 100
                BackgroundWorkerLOAD.ReportProgress(i)
                If i = 0 Then
                    thread1 = New Thread(AddressOf LoadConn)
                    thread1.Start()
                    threadList.Add(thread1)
                    For Each t In threadList
                        t.Join()
                    Next
                    thread1 = New Thread(AddressOf TestLocalConnection)
                    thread1.Start()
                    threadList.Add(thread1)
                    For Each t In threadList
                        t.Join()
                    Next
                    thread1 = New Thread(AddressOf LoadCloudConn)
                    thread1.Start()
                    threadList.Add(thread1)
                    thread1 = New Thread(AddressOf LoadAutoBackup)
                    thread1.Start()
                    threadList.Add(thread1)
                    thread1 = New Thread(AddressOf LoadAdditionalSettings)
                    thread1.Start()
                    threadList.Add(thread1)
                    thread1 = New Thread(AddressOf LoadDevInfo)
                    thread1.Start()
                    threadList.Add(thread1)
                End If
            Next
            For Each t In threadList
                t.Join()
            Next
        Catch ex As Exception

        End Try
    End Sub
    Private Sub BackgroundWorkerLOAD_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorkerLOAD.ProgressChanged
        ProgressBar4.Value = e.ProgressPercentage
    End Sub
    Private Sub BackgroundWorkerLOAD_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorkerLOAD.RunWorkerCompleted
        If ValidLocalConnection = True Then
            ButtonSaveLocalCon.PerformClick()
            LOCALCONNDATA = False
        End If

        If ValidCloudConnection = True Then
            ButtonSaveCloudConn.PerformClick()
            CLOUDCONDATA = False
        End If
    End Sub
    Private Sub DatePickerState(tf As Boolean)
        Try
            DateTimePicker1ACCRDI.Enabled = tf
            DateTimePicker2ACCRVU.Enabled = tf
            DateTimePicker4PTUDI.Enabled = tf
            DateTimePickerPTUVU.Enabled = tf
        Catch ex As Exception

        End Try
    End Sub
    Private Sub ButtonEditDevSet_Click(sender As Object, e As EventArgs) Handles ButtonEditDevSet.Click
        TextboxEnableability(GroupBox11, True)
        DatePickerState(True)
        FillUp = False
    End Sub
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles ButtonSaveDevSettings.Click
        FillUp = False
        SaveDevInfo()
    End Sub
    Private Sub SaveDevInfo()
        Try
            Dim table = "loc_settings"
            Dim where = "settings_id = 1"
            If Not TextboxIsEmpty(GroupBox11) Then
                If ValidLocalConnection = True Then
                    Dim ConnectionLocal As MySqlConnection = TestLocalConnection()
                    Dim fields = "Dev_Company_Name, Dev_Address, Dev_Tin, Dev_Accr_No, Dev_Accr_Date_Issued, Dev_Accr_Valid_Until, Dev_PTU_No, Dev_PTU_Date_Issued, Dev_PTU_Valid_Until, S_DB_Version"
                    Dim sql = "Select " & fields & " FROM " & table & " WHERE " & where
                    Dim cmd As MySqlCommand = New MySqlCommand(sql, ConnectionLocal)
                    Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
                    Dim dt As DataTable = New DataTable
                    da.Fill(dt)
                    If dt.Rows.Count > 0 Then
                        Dim fields1 = "`Dev_Company_Name`= '" & Trim(TextBoxDevname.Text) & "',
                `Dev_Address`= '" & Trim(TextBoxDevAdd.Text) & "',
                `Dev_Tin`= '" & Trim(TextBoxDevTIN.Text) & "',
                `Dev_Accr_No`= '" & Trim(TextBoxDevAccr.Text) & "' ,
                `Dev_Accr_Date_Issued`= '" & Format(DateTimePicker1ACCRDI.Value, "yyy-MM-dd") & "',
                `Dev_Accr_Valid_Until`= '" & Format(DateTimePicker2ACCRVU.Value, "yyyy-MM-dd") & "',
                `Dev_PTU_No`= '" & Trim(TextBoxDEVPTU.Text) & "',
                `Dev_PTU_Date_Issued`= '" & Format(DateTimePickerPTUVU.Value, "yyyy-MM-dd") & "',
                `Dev_PTU_Valid_Until`= '" & Format(DateTimePicker4PTUDI.Value, "yyyy-MM-dd") & "',
                `S_DB_Version` = 'v.1.0.0 POS-MOD-2022-10-31'"
                        sql = "UPDATE " & table & " SET " & fields1 & " WHERE " & where
                        cmd = New MySqlCommand(sql, ConnectionLocal)
                        cmd.ExecuteNonQuery()
                        ConfirmDevInfoSettings = True
                        If FillUp = True Then
                        Else
                            MsgBox("Saved!")
                        End If

                    Else
                        Dim fields2 = "(Dev_Company_Name, Dev_Address, Dev_Tin, Dev_Accr_No, Dev_Accr_Date_Issued, Dev_Accr_Valid_Until, Dev_PTU_No, Dev_PTU_Date_Issued, Dev_PTU_Valid_Until, S_DB_Version)"
                        Dim value = "('" & Trim(TextBoxDevname.Text) & "'
                ,'" & Trim(TextBoxDevAdd.Text) & "'
                ,'" & Trim(TextBoxDevTIN.Text) & "'
                ,'" & Trim(TextBoxDevAccr.Text) & "'
                ,'" & Format(DateTimePicker1ACCRDI.Value, "yyyy-MM-dd") & "'
                ,'" & Format(DateTimePicker2ACCRVU.Value, "yyyy-MM-dd") & "'
                ,'" & Trim(TextBoxDEVPTU.Text) & "'
                ,'" & Format(DateTimePickerPTUVU.Value, "yyyy-MM-dd") & "'
                ,'" & Format(DateTimePicker4PTUDI.Value, "yyyy-MM-dd") & "'
                ,'v.1.0.0 POS-MOD-2022-10-31')"
                        sql = "INSERT INTO " & table & " " & fields2 & " VALUES " & value
                        cmd = New MySqlCommand(sql, ConnectionLocal)
                        cmd.ExecuteNonQuery()
                        ConfirmDevInfoSettings = True
                        If FillUp = True Then
                        Else
                            MsgBox("Saved!")
                        End If

                    End If
                    TextboxEnableability(GroupBox11, False)
                    DatePickerState(False)
                    ConnectionLocal.Close()
                Else
                    MsgBox("Invalid local connection")
                    ConfirmDevInfoSettings = False
                End If
            Else
                MsgBox("All fields are required")
                ConfirmDevInfoSettings = False
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles ButtonActivate.Click
        If ValidLocalConnection Then
            If BTNSaveLocalConn Then
                If ValidCloudConnection Then
                    If Autobackup Then
                        If BTNSaveCloudConn Then
                            If ConfirmAdditionalSettings Then
                                If ConfirmDevInfoSettings Then
                                    If PrintOptionIsSet And PrintOption <> "" Then
                                        If RePrintOptionIsSet And RePrintOption <> "" Then
                                            If PrintXZRead And PrintXZReadOption <> "" Then
                                                If PrintReturnsBool And PrintReturns <> "" Then
                                                    If PrintSalesReportBool And PrintSalesReport <> "" Then
                                                        If AccountExist Then
                                                            If FranchiseeStoreValidation Then
                                                                If Not String.IsNullOrWhiteSpace(TextBoxProdKey.Text) Then

                                                                    TabControl1.TabPages(0).Enabled = False
                                                                    TabControl3.TabPages(0).Enabled = False

                                                                    DataGridViewOutlets.Enabled = False
                                                                    TextboxEnableability(GroupBox12, False)
                                                                    ButtonEnableability(GroupBox12, False)

                                                                    BackgroundWorkerValidateSerial.WorkerReportsProgress = True
                                                                    BackgroundWorkerValidateSerial.WorkerSupportsCancellation = True
                                                                    BackgroundWorkerValidateSerial.RunWorkerAsync()

                                                                Else
                                                                    MsgBox("Please input serial key")
                                                                End If
                                                            Else
                                                                MsgBox("Please select store in Account and Store settings tab")
                                                            End If
                                                        Else
                                                            MsgBox("Franchisee's Account must be valid first")
                                                        End If
                                                    Else
                                                        MsgBox("Select sales report print option first")
                                                    End If
                                                Else
                                                    MsgBox("Select item returns print option first")
                                                End If
                                            Else
                                                MsgBox("Select x-zreading print option first")
                                            End If
                                        Else
                                            MsgBox("Select reprint option first")
                                        End If
                                    Else
                                        MsgBox("Select print option first")
                                    End If
                                Else
                                    MsgBox("Please fill up all fields in Developer Information Settings")
                                End If
                            Else
                                MsgBox("Please fill up all fields in Additional Settings")
                            End If
                        Else
                            MsgBox("Save Cloud connection first")
                        End If
                    Else
                        MsgBox("Automatic backup interval has not been defined")
                    End If
                Else
                    MsgBox("Invalid Cloud Connection")
                End If
            Else
                MsgBox("Save local connection first")
            End If
        Else
            MsgBox("Invalid Local Connection")
        End If
    End Sub



    Private Sub BackgroundWorkerValidateSerial_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorkerValidateSerial.DoWork
        Try
            For i = 0 To 100
                Thread.Sleep(10)
                BackgroundWorkerValidateSerial.ReportProgress(i)
                If i = 0 Then
                    ThreadActivation = New Thread(AddressOf SerialKey)
                    ThreadActivation.Start()
                    threadListActivation.Add(ThreadActivation)
                    For Each t In threadListActivation
                        t.Join()
                    Next
                End If
            Next
        Catch ex As Exception

        End Try
    End Sub

    Private Sub BackgroundWorkerValidateSerial_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorkerValidateSerial.ProgressChanged
        ProgressBar5.Value = e.ProgressPercentage
    End Sub

    Private Sub BackgroundWorkerValidateSerial_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorkerValidateSerial.RunWorkerCompleted
        If ValidProductKey = True Then
            BackgroundWorkerACTIVATION.WorkerReportsProgress = True
            BackgroundWorkerACTIVATION.WorkerSupportsCancellation = True
            BackgroundWorkerACTIVATION.RunWorkerAsync()
        Else
            MsgBox("Invalid Product key")
            TextboxEnableability(GroupBox12, True)
            ButtonEnableability(GroupBox12, True)
            TabControl1.TabPages(0).Enabled = True
            TabControl3.TabPages(0).Enabled = True
        End If
    End Sub
    Private Sub BackgroundWorkerACTIVATION_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorkerACTIVATION.DoWork
        Try
            For i = 0 To 100
                Thread.Sleep(50)
                BackgroundWorkerACTIVATION.ReportProgress(i)
                If i = 0 Then
                    ThreadActivation = New Thread(AddressOf adminserialkey)
                    ThreadActivation.Start()
                    threadListActivation.Add(ThreadActivation)
                    For Each t In threadListActivation
                        t.Join()
                    Next
                End If
                If i = 20 Then
                    ThreadActivation = New Thread(AddressOf adminoutlets)
                    ThreadActivation.Start()
                    threadListActivation.Add(ThreadActivation)
                    For Each t In threadListActivation
                        t.Join()
                    Next
                End If
                If i = 40 Then
                    ThreadActivation = New Thread(AddressOf insertintocloud)
                    ThreadActivation.Start()
                    threadListActivation.Add(ThreadActivation)
                    For Each t In threadListActivation
                        t.Join()
                    Next
                End If
                If i = 60 Then
                    ThreadActivation = New Thread(AddressOf insertintolocaloutlets)
                    ThreadActivation.Start()
                    threadListActivation.Add(ThreadActivation)
                    For Each t In threadListActivation
                        t.Join()
                    Next
                End If
                If i = 80 Then
                    ThreadActivation = New Thread(AddressOf InsertLocalMasterList)
                    ThreadActivation.Start()
                    threadListActivation.Add(ThreadActivation)
                    For Each t In threadListActivation
                        t.Join()
                    Next
                    ThreadActivation = New Thread(AddressOf SaveLogo)
                    ThreadActivation.Start()
                    threadListActivation.Add(ThreadActivation)
                    For Each t In threadListActivation
                        t.Join()
                    Next
                End If
            Next
        Catch ex As Exception

        End Try
    End Sub
    Private Sub BackgroundWorkerACTIVATION_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorkerACTIVATION.ProgressChanged
        ProgressBar5.Value = e.ProgressPercentage
    End Sub
    Private Sub BackgroundWorkerACTIVATION_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorkerACTIVATION.RunWorkerCompleted
        BackgroundWorkerPreAct.WorkerReportsProgress = True
        BackgroundWorkerPreAct.WorkerSupportsCancellation = True
        BackgroundWorkerPreAct.RunWorkerAsync()
    End Sub

    Private Function GLOBAL_SELECT_ALL_FUNCTION_CLOUD(ByRef tbl As String, ByRef flds As String, ByRef datagrid As DataGridView) As DataTable
        Dim dt As New DataTable
        Try
            Dim ConnectionCloud As MySqlConnection = TestCloudConnection()
            Dim sql = "SELECT " & flds & " FROM " & tbl
            Dim cmd As MySqlCommand = New MySqlCommand(sql, ConnectionCloud)
            Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
            da.Fill(dt)
            datagrid.ReadOnly = True
            ConnectionCloud.Close()
        Catch ex As Exception
            Throw New System.Exception(ex.Message)
        End Try
        Return dt
    End Function


#Region "Get Data From Cloud"
    Property threadListActivation As List(Of Thread) = New List(Of Thread)
    Property threadActivation As Thread

    Private Sub BackgroundWorkerPreAct_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorkerPreAct.DoWork
        Try
            SyncFrom = Now
            For i = 0 To 100
                Label22.Text = "Please wait " & i & " %"
                BackgroundWorkerPreAct.ReportProgress(i)
                Thread.Sleep(20)
                If i = 0 Then
                    Select Case SetCatStats
                        Case SyncCls.CategorySync.Stats.isReady, SyncCls.CategorySync.Stats.isFetchInterrupt
                            threadActivation = New Thread(AddressOf GetCategories)
                            threadActivation.Start()
                            threadListActivation.Add(threadActivation)
                            For Each t In threadListActivation
                                t.Join()
                            Next
                    End Select
                End If

                If i = 10 Then
                    Select Case SetInvStats
                        Case SyncCls.InventorySync.Stats.isReady, SyncCls.InventorySync.Stats.isFetchInterrupt
                            threadActivation = New Thread(AddressOf GetInventory)
                            threadActivation.Start()
                            threadListActivation.Add(threadActivation)
                            For Each t In threadListActivation
                                t.Join()
                            Next
                    End Select
                End If

                If i = 20 Then
                    Select Case SetFormStats
                        Case SyncCls.FormulaSync.Stats.isReady, SyncCls.FormulaSync.Stats.isFetchInterrupt
                            threadActivation = New Thread(AddressOf GetFormula)
                            threadActivation.Start()
                            threadListActivation.Add(threadActivation)
                            For Each t In threadListActivation
                                t.Join()
                            Next
                    End Select
                End If

                If i = 30 Then
                    Select Case SetCoupStats
                        Case SyncCls.CouponSync.Stats.isReady, SyncCls.CouponSync.Stats.isFetchInterrupt
                            threadActivation = New Thread(AddressOf GetCoupons)
                            threadActivation.Start()
                            threadListActivation.Add(threadActivation)
                            For Each t In threadListActivation
                                t.Join()
                            Next
                    End Select
                End If
                If i = 40 Then
                    Select Case SetPartStats
                        Case SyncCls.PartnersSync.Stats.isReady, SyncCls.PartnersSync.Stats.isFetchInterrupt
                            threadActivation = New Thread(AddressOf GetPartners)
                            threadActivation.Start()
                            threadListActivation.Add(threadActivation)
                            For Each t In threadListActivation
                                t.Join()
                            Next
                    End Select
                End If
                If i = 50 Then
                    Select Case SetStockCatStats
                        Case SyncCls.StockCatSync.Stats.isReady, SyncCls.StockCatSync.Stats.isFetchInterrupt
                            threadActivation = New Thread(AddressOf GetStockCategory)
                            threadActivation.Start()
                            threadListActivation.Add(threadActivation)
                            For Each t In threadListActivation
                                t.Join()
                            Next
                    End Select
                End If

                If i = 60 Then
                    Select Case SetProdStats
                        Case SyncCls.ProductsSync.Stats.isReady, SyncCls.ProductsSync.Stats.isFetchInterrupt
                            threadActivation = New Thread(Sub() GetProducts())
                            threadActivation.Start()
                            threadListActivation.Add(threadActivation)
                            For Each t In threadListActivation
                                t.Join()
                            Next
                    End Select
                End If

                If i = 70 Then

                    Select Case SetReceiptInfoStats
                        Case SyncCls.ReceiptInfoSync.Stats.isReady, SyncCls.ReceiptInfoSync.Stats.isFetchInterrupt
                            threadActivation = New Thread(Sub() listOfReceiptInfo = GetReceiptInfo())
                            threadActivation.Start()
                            threadListActivation.Add(threadActivation)
                            For Each t In threadListActivation
                                t.Join()
                            Next
                    End Select
                End If

                If i = 80 Then
                    thread1 = New Thread(AddressOf FillDgvProd)
                    thread1.Start()
                    threadListActivation.Add(thread1)
                    For Each t In threadListActivation
                        t.Join()
                    Next

                    Select Case SetCatStats
                        Case SyncCls.CategorySync.Stats.isFetchComplete, SyncCls.CategorySync.Stats.isProcessInterrupt
                            thread1 = New Thread(AddressOf InsertToCategories)
                            thread1.Start()
                            threadListActivation.Add(thread1)
                            For Each t In threadListActivation
                                t.Join()
                            Next
                    End Select

                    Select Case SetInvStats
                        Case SyncCls.InventorySync.Stats.isFetchComplete, SyncCls.InventorySync.Stats.isProcessInterrupt
                            thread1 = New Thread(AddressOf InsertToInventory)
                            thread1.Start()
                            threadListActivation.Add(thread1)
                            For Each t In threadListActivation
                                t.Join()
                            Next
                    End Select

                    Select Case SetFormStats
                        Case SyncCls.FormulaSync.Stats.isFetchComplete, SyncCls.FormulaSync.Stats.isProcessInterrupt
                            thread1 = New Thread(AddressOf InsertToFormula)
                            thread1.Start()
                            threadListActivation.Add(thread1)
                            For Each t In threadListActivation
                                t.Join()
                            Next
                    End Select

                    Select Case SetPartStats
                        Case SyncCls.PartnersSync.Stats.isFetchComplete, SyncCls.PartnersSync.Stats.isProcessInterrupt
                            thread1 = New Thread(AddressOf InsertPartnersTransacton)
                            thread1.Start()
                            threadListActivation.Add(thread1)
                            For Each t In threadListActivation
                                t.Join()
                            Next
                    End Select

                    Select Case SetCoupStats
                        Case SyncCls.CouponSync.Stats.isFetchComplete, SyncCls.CouponSync.Stats.isProcessInterrupt
                            thread1 = New Thread(AddressOf InsertCoupons)
                            thread1.Start()
                            threadListActivation.Add(thread1)
                            For Each t In threadListActivation
                                t.Join()
                            Next
                    End Select


                    Select Case SetProdStats
                        Case SyncCls.ProductsSync.Stats.isFetchComplete, SyncCls.ProductsSync.Stats.isProcessInterrupt
                            thread1 = New Thread(AddressOf InsertToProducts)
                            thread1.Start()
                            threadListActivation.Add(thread1)
                            For Each t In threadListActivation
                                t.Join()
                            Next
                    End Select

                    Select Case SetStockCatStats
                        Case SyncCls.StockCatSync.Stats.isFetchComplete, SyncCls.StockCatSync.Stats.isProcessInterrupt
                            thread1 = New Thread(AddressOf InsertToStockCategory)
                            thread1.Start()
                            threadListActivation.Add(thread1)
                            For Each t In threadListActivation
                                t.Join()
                            Next
                    End Select

                    Select Case SetReceiptInfoStats
                        Case SyncCls.ReceiptInfoSync.Stats.isFetchComplete, SyncCls.ReceiptInfoSync.Stats.isProcessInterrupt
                            thread1 = New Thread(AddressOf InsertToReceiptInfo)
                            thread1.Start()
                            threadListActivation.Add(thread1)
                            For Each t In threadListActivation
                                t.Join()
                            Next
                    End Select

                End If
            Next
        Catch ex As Exception
            '
        End Try
    End Sub
    Private Sub BackgroundWorkerPreAct_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorkerPreAct.ProgressChanged
        ProgressBar6.Value = e.ProgressPercentage
    End Sub
    Private Sub BackgroundWorkerPreAct_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorkerPreAct.RunWorkerCompleted

        If SetCatStats = SyncCls.CategorySync.Stats.isProcessComplete And
            SetProdStats = SyncCls.ProductsSync.Stats.isProcessComplete And
            SetInvStats = SyncCls.InventorySync.Stats.isProcessComplete And
            SetFormStats = SyncCls.FormulaSync.Stats.isProcessComplete And
            SetPartStats = SyncCls.PartnersSync.Stats.isProcessComplete And
            SetCoupStats = SyncCls.CouponSync.Stats.isProcessComplete And
            SetStockCatStats = SyncCls.StockCatSync.Stats.isProcessComplete And
            SetReceiptInfoStats = SyncCls.ReceiptInfoSync.Stats.isProcessComplete Then

            SyncTo = Now
            Dim TS As TimeSpan = SyncTo - SyncFrom
            rtbLogStats.Text += $"Synced data total of {TS.Seconds.ToString} seconds dated {Now}"

            Dim message As Integer = MessageBox.Show("Successfully Registered. Your system will automatically reboot after pressing OK button.", "Activated", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ValidProductKey = False
            TextboxEnableability(GroupBox12, True)
            ButtonEnableability(GroupBox12, True)
            Dim logPath = My.Computer.FileSystem.SpecialDirectories.MyDocuments
            Dim file As StreamWriter
            file = My.Computer.FileSystem.OpenTextFileWriter(logPath & "\configmanagerslogs.txt", True)
            file.WriteLine(rtbLogStats.Text)
            file.Close()
            Close()
            ChooseLayout.Show()
        Else
            Dim message As Integer = MessageBox.Show("Retry to download product data ?", "NOTICE", MessageBoxButtons.OK, MessageBoxIcon.Information)
            If message = DialogResult.OK Then
                BackgroundWorkerPreAct.WorkerReportsProgress = True
                BackgroundWorkerPreAct.WorkerSupportsCancellation = True
                BackgroundWorkerPreAct.RunWorkerAsync()
            End If
        End If

    End Sub
#Region "FETCH DATA"
    Property SetReceiptInfoStats As SyncCls.ReceiptInfoSync.Stats = SyncCls.ReceiptInfoSync.Stats.isReady
    Property SetReceiptInfoLastIndex As New SyncCls.ReceiptInfoSync
    Public Function GetReceiptInfo() As List(Of ReceiptInfoCls)
        Dim listofReceiptcls As New List(Of ReceiptInfoCls)
        Try
            Dim SqlCount As String = ""

            Dim Cond As String = ""

            If SetReceiptInfoStats = SyncCls.CategorySync.Stats.isFetchInterrupt Then
                Cond = $"WHERE id > {SetReceiptInfoLastIndex.lastFetchID}"
            End If

            SqlCount = $"SELECT * FROM admin_receipt_info_org {Cond}"

            SetReceiptInfoStats = SyncCls.ProductsSync.Stats.isFetching

            rtbLogStats.Text += $"{FullDate24HR()} :    Getting cloud server's receipt info data.{vbNewLine}"

            Dim Connection As MySqlConnection = TestCloudConnection()

            Using CmdCount As New MySqlCommand(SqlCount, Connection)
                Using Reader = CmdCount.ExecuteReader
                    While Reader.Read
                        Dim nwListOfReceipt As New ReceiptInfoCls With {
                            .Code = Reader("type").ToString,
                            .Description = Reader("description").ToString,
                            .Status = Reader("status").ToString
                        }
                        listofReceiptcls.Add(nwListOfReceipt)
                        SetReceiptInfoLastIndex.lastFetchID = Reader("id")
                    End While
                End Using
            End Using

            SetReceiptInfoStats = SyncCls.CategorySync.Stats.isFetchComplete
            rtbLogStats.Text += FullDate24HR() & " :    Complete(Fetching of categories data)" & vbNewLine

        Catch ex As Exception
            SetReceiptInfoStats = SyncCls.CategorySync.Stats.isFetchInterrupt
            rtbLogStats.Text += FullDate24HR() & " :    Failed(Fetching of categories data)" & vbNewLine
        End Try
        Return listofReceiptcls
    End Function
    Property SetCatStats As SyncCls.CategorySync.Stats = SyncCls.CategorySync.Stats.isReady
    Property SetCatLastIndex As New SyncCls.CategorySync
    Public Sub GetCategories()
        Try
            Dim Cond As String = ""

            If SetCatStats = SyncCls.CategorySync.Stats.isFetchInterrupt Then
                Cond = $"WHERE category_id > {SetCatLastIndex.lastFetchID}"
            End If

            SetCatStats = SyncCls.CategorySync.Stats.isFetching
            rtbLogStats.Text += FullDate24HR() & " :    Getting cloud server's categories table." & vbNewLine

            Dim Datatablecat = GLOBAL_SELECT_ALL_FUNCTION_CLOUD($"admin_category {Cond}", "*", DataGridViewCATEGORIES)

            For Each row As DataRow In Datatablecat.Rows
                DataGridViewCATEGORIES.Rows.Add(row("category_name"), row("brand_name"), row("updated_at"), row("origin"), row("status"))
                SetCatLastIndex.lastFetchID = row("category_id")
            Next

            SetCatStats = SyncCls.CategorySync.Stats.isFetchComplete
            rtbLogStats.Text += FullDate24HR() & " :    Complete(Fetching of categories data)" & vbNewLine

        Catch ex As Exception
            SetCatStats = SyncCls.CategorySync.Stats.isFetchInterrupt
            rtbLogStats.Text += FullDate24HR() & " :    Failed(Fetching of categories data)" & vbNewLine
        End Try
    End Sub
    Property SetInvStats As SyncCls.InventorySync.Stats = SyncCls.InventorySync.Stats.isReady
    Property SetInvLastIndex As New SyncCls.InventorySync
    Public Sub GetInventory()
        Try
            Dim Cond As String = ""

            If SetInvStats = SyncCls.InventorySync.Stats.isFetchInterrupt Then
                Cond = $"WHERE server_inventory_id > {SetInvLastIndex.lastFetchID}"
            End If

            SetInvStats = SyncCls.InventorySync.Stats.isFetching
            rtbLogStats.Text += FullDate24HR() & " :    Getting cloud server's inventories data." & vbNewLine

            Dim DatatableInv = GLOBAL_SELECT_ALL_FUNCTION_CLOUD($"admin_pos_inventory_org {Cond}", "*", DataGridViewINVENTORY)
            For Each row As DataRow In DatatableInv.Rows
                DataGridViewINVENTORY.Rows.Add(row("server_inventory_id"), row("server_formula_id"), row("product_ingredients"), row("sku"), row("stock_primary"), row("stock_secondary"), row("stock_no_of_servings"), row("stock_status"), row("critical_limit"), row("date_modified"), row("main_inventory_id"), row("origin"), row("show_stockin"))
                SetInvLastIndex.lastFetchID = row("server_inventory_id")
            Next
            rtbLogStats.Text += FullDate24HR() & " :    Complete(Fetching of inventories data)" & vbNewLine
            SetInvStats = SyncCls.InventorySync.Stats.isFetchComplete
        Catch ex As Exception
            rtbLogStats.Text += FullDate24HR() & " :    Failed(Fetching of inventories data)" & vbNewLine
            SetInvStats = SyncCls.InventorySync.Stats.isFetchInterrupt
        End Try
    End Sub
    Property SetProdStats As SyncCls.ProductsSync.Stats = SyncCls.ProductsSync.Stats.isReady
    Property SetProdLastIndex As New SyncCls.ProductsSync
    Private Sub GetProducts()
        Try
            Dim SqlCount As String = ""

            Dim Cond As String = ""

            If SetProdStats = SyncCls.CategorySync.Stats.isFetchInterrupt Then
                Cond = $"WHERE product_id > {SetProdLastIndex.lastFetchID}"
            End If

            SqlCount = $"SELECT * FROM admin_products_org {Cond}"

            SetProdStats = SyncCls.ProductsSync.Stats.isFetching

            rtbLogStats.Text += $"{FullDate24HR()} :    Getting cloud server's products data.{vbNewLine}"
            Dim Connection As MySqlConnection = TestCloudConnection()

            Using CmdCount As New MySqlCommand(SqlCount, Connection)
                Dim Reader = CmdCount.ExecuteReader
                While Reader.Read
                    Dim nwListofProd As New ProductsCls With {
                         .product_id = Reader("product_id"),
                         .product_sku = Reader("product_sku").ToString,
                         .product_name = Reader("product_name").ToString,
                         .formula_id = Reader("formula_id").ToString,
                         .product_barcode = Reader("product_barcode").ToString,
                         .product_category = Reader("product_category").ToString,
                         .product_price = Reader("product_price"),
                         .product_desc = Reader("product_desc").ToString,
                         .product_image = Reader("product_image"),
                         .product_status = If(Reader("product_status").ToString = "", "1", Reader("product_status").ToString),
                         .date_modified = Reader("date_modified"),
                         .inventory_id = Reader("inventory_id"),
                         .addontype = Reader("addontype").ToString,
                         .half_batch = Reader("half_batch"),
                         .partners = Reader("partners").ToString,
                         .arrangement = Reader("arrangement").ToString
                        }

                    Dim fndProd = listOfProducts.Exists(Function(x) x.product_id = nwListofProd.product_id)

                    If Not fndProd Then
                        listOfProducts.Add(nwListofProd)
                    End If

                    SetProdLastIndex.lastFetchID = Reader("product_id")
                End While
                CmdCount.Dispose()
            End Using
            rtbLogStats.Text += FullDate24HR() & " :    Complete(Fetching of products data)" & vbNewLine
            SetProdStats = SyncCls.ProductsSync.Stats.isFetchComplete
        Catch ex As Exception
            rtbLogStats.Text += FullDate24HR() & " :    Failed(Fetching of products data)" & vbNewLine
            SetProdStats = SyncCls.ProductsSync.Stats.isFetchInterrupt
        End Try
    End Sub
    Property SetFormStats As SyncCls.FormulaSync.Stats = SyncCls.FormulaSync.Stats.isReady
    Property SetFormLastIndex As New SyncCls.FormulaSync
    Public Sub GetFormula()
        Try
            Dim Cond As String = ""

            If SetFormStats = SyncCls.CategorySync.Stats.isFetchInterrupt Then
                Cond = $"WHERE server_formula_id > {SetFormLastIndex.lastFetchID}"
            End If

            SetFormStats = SyncCls.CategorySync.Stats.isFetching
            rtbLogStats.Text += FullDate24HR() & " :    Getting cloud server's formulas data." & vbNewLine

            Dim DatatableForm = GLOBAL_SELECT_ALL_FUNCTION_CLOUD($"admin_product_formula_org {Cond}", "*", DataGridViewFORMULA)
            For Each row As DataRow In DatatableForm.Rows
                DataGridViewFORMULA.Rows.Add(row("server_formula_id"), row("product_ingredients"), row("primary_unit"), row("primary_value"), row("secondary_unit"), row("secondary_value"), row("serving_unit"), row("serving_value"), row("no_servings"), row("status"), row("date_modified"), row("unit_cost"), row("origin"))
                SetFormLastIndex.lastFetchID = row("server_formula_id")
            Next

            rtbLogStats.Text += FullDate24HR() & " :    Complete(Fetching of formulas data)" & vbNewLine
            SetFormStats = SyncCls.CategorySync.Stats.isFetchComplete
        Catch ex As Exception
            rtbLogStats.Text += FullDate24HR() & " :    Failed(Fetching of formulas data)" & vbNewLine
            SetFormStats = SyncCls.CategorySync.Stats.isFetchInterrupt
        End Try
    End Sub
    Property SetCoupStats As SyncCls.CouponSync.Stats = SyncCls.CouponSync.Stats.isReady
    Property SetCoupLastIndex As New SyncCls.CouponSync
    Private Sub GetCoupons()
        Try
            Dim Cond As String = ""

            If SetCoupStats = SyncCls.CouponSync.Stats.isFetchInterrupt Then
                Cond = $"WHERE ID > {SetCoupLastIndex.lastFetchID}"
            End If

            rtbLogStats.Text += FullDate24HR() & " :    Getting cloud server's coupons data." & vbNewLine
            SetCoupStats = SyncCls.CouponSync.Stats.isFetching

            Dim DatatableCoupons = GLOBAL_SELECT_ALL_FUNCTION_CLOUD($"admin_coupon {Cond}", "*", DataGridViewCoupons)
            For Each row As DataRow In DatatableCoupons.Rows
                DataGridViewCoupons.Rows.Add(row("Couponname_"), row("Desc_"), row("Discountvalue_"), row("Referencevalue_"), row("Type"), row("Bundlebase_"), row("BBValue_"), row("Bundlepromo_"), row("BPValue_"), row("Effectivedate"), row("Expirydate"), row("date_created"), row("active"))
                SetCoupLastIndex.lastFetchID = row("ID")
            Next
            rtbLogStats.Text += FullDate24HR() & " :    Complete(Fetching of coupons data)" & vbNewLine
            SetCoupStats = SyncCls.CouponSync.Stats.isFetchComplete
        Catch ex As Exception
            rtbLogStats.Text += FullDate24HR() & " :    Failed(Fetching of coupons data)" & vbNewLine
            SetCoupStats = SyncCls.CouponSync.Stats.isFetchInterrupt
        End Try
    End Sub
    Property SetPartStats As SyncCls.PartnersSync.Stats = SyncCls.PartnersSync.Stats.isReady
    Property SetPartLastIndex As New SyncCls.PartnersSync
    Private Sub GetPartners()
        Try
            Dim Cond As String = ""

            If SetPartStats = SyncCls.PartnersSync.Stats.isFetchInterrupt Then
                Cond = $"WHERE id > {SetPartLastIndex.lastFetchID}"
            End If

            rtbLogStats.Text += FullDate24HR() & " :    Getting cloud server's partners data." & vbNewLine
            SetPartStats = SyncCls.PartnersSync.Stats.isFetching

            Dim DatatablePartners = GLOBAL_SELECT_ALL_FUNCTION_CLOUD($"admin_partners_transaction_org {Cond}", "*", DataGridViewPartners)
            For Each row As DataRow In DatatablePartners.Rows
                DataGridViewPartners.Rows.Add(row("arrid"), row("bankname"), row("date_modified"), row("active"))
                SetPartLastIndex.lastFetchID = row("id")
            Next

            rtbLogStats.Text += FullDate24HR() & " :    Complete(Fetching of partners data)" & vbNewLine
            SetPartStats = SyncCls.PartnersSync.Stats.isFetchComplete
        Catch ex As Exception
            rtbLogStats.Text += FullDate24HR() & " :    Failed(Fetching of partners data)" & vbNewLine
            SetPartStats = SyncCls.PartnersSync.Stats.isFetchInterrupt

        End Try
    End Sub
    Property SetStockCatStats As SyncCls.StockCatSync.Stats = SyncCls.StockCatSync.Stats.isReady
    Property SetStockCatLastIndex As New SyncCls.StockCatSync
    Private Sub GetStockCategory()
        Try
            Dim Cond As String = ""

            If SetStockCatStats = SyncCls.StockCatSync.Stats.isFetchInterrupt Then
                Cond = $"WHERE adj_id > {SetStockCatLastIndex.lastFetchID}"
            End If

            rtbLogStats.Text += FullDate24HR() & " :    Getting cloud server's stock adjustment categories data." & vbNewLine
            SetStockCatStats = SyncCls.StockCatSync.Stats.isFetching

            Dim DatatableStockAdjustment = GLOBAL_SELECT_ALL_FUNCTION_CLOUD($"admin_stock_category {Cond}", "*", DataGridViewStockAdjustment)

            For Each row As DataRow In DatatableStockAdjustment.Rows
                DataGridViewStockAdjustment.Rows.Add(row("adj_type"), row("created_at"), row("active"))
                SetStockCatLastIndex.lastFetchID = row("adj_id")
            Next

            rtbLogStats.Text += FullDate24HR() & " :    Complete(Fetching of stock adjustment categories data)" & vbNewLine
            SetStockCatStats = SyncCls.StockCatSync.Stats.isFetchComplete
        Catch ex As Exception
            rtbLogStats.Text += FullDate24HR() & " :    Failed(Fetching of stock adjustment categories data)" & vbNewLine
            SetStockCatStats = SyncCls.StockCatSync.Stats.isFetchInterrupt
        End Try
    End Sub
#End Region
#Region "INSERT DATA"
    Private Sub FillDgvProd()
        Try

            DataGridViewPRODUCTS.Rows.Clear()
            For Each prod As ProductsCls In listOfProducts
                With prod
                    DataGridViewPRODUCTS.Rows.Add(.product_id, .product_sku, .product_name, .formula_id, .product_barcode, .product_category, .product_price, .product_desc, .product_image, .product_status, .date_modified, .inventory_id, .addontype, .half_batch, .partners, .arrangement)
                End With
            Next
        Catch ex As Exception
        End Try
    End Sub
    Private Sub InsertToProducts()
        Try
            rtbLogStats.Text += FullDate24HR() & " :    Inserting data to local server's table(Products)" & vbNewLine
            SetProdStats = SyncCls.ProductsSync.Stats.isProcessing

            Dim ConnectionLocal As MySqlConnection = TestLocalConnection()

            For Each prod As ProductsCls In listOfProducts.Skip(SetProdLastIndex.lastInsertID)

                Dim Query = "INSERT INTO loc_admin_products 
                                (
                                    `server_product_id`,`product_sku`, `product_name`, `formula_id`, `product_barcode`, `product_category`, `product_price`, `product_desc`, 
                                    `product_image`, `origin`, `product_status`, `date_modified`, `server_inventory_id`, `guid`, `store_id`, `addontype`, `half_batch`, 
                                    `partners`, `arrangement`, `synced`
                                )
                             VALUES 
                                (
                                    @server_product_id, @product_sku, @product_name, @formula_id, @product_barcode, @product_category, @product_price, @product_desc, 
                                    @product_image, @origin, @product_status, @date_modified, @server_inventory_id, @guid, @store_id, @addontype, @half_batch, 
                                    @partners, @arrangement, @synced
                                )"
                Using cmdlocal As New MySqlCommand(Query, ConnectionLocal)
                    cmdlocal.Parameters.Clear()
                    With prod
                        cmdlocal.Parameters.AddWithValue("@server_product_id", .product_id)
                        cmdlocal.Parameters.AddWithValue("@product_sku", .product_sku)
                        cmdlocal.Parameters.AddWithValue("@product_name", .product_name)
                        cmdlocal.Parameters.AddWithValue("@formula_id", .formula_id)
                        cmdlocal.Parameters.AddWithValue("@product_barcode", .product_barcode)
                        cmdlocal.Parameters.AddWithValue("@product_category", .product_category)
                        cmdlocal.Parameters.AddWithValue("@product_price", .product_price)
                        cmdlocal.Parameters.AddWithValue("@product_desc", .product_desc)
                        cmdlocal.Parameters.AddWithValue("@product_image", .product_image)
                        cmdlocal.Parameters.AddWithValue("@product_status", .product_status)
                        cmdlocal.Parameters.AddWithValue("@origin", "Server")
                        cmdlocal.Parameters.AddWithValue("@date_modified", .date_modified)
                        cmdlocal.Parameters.AddWithValue("@server_inventory_id", .inventory_id)
                        cmdlocal.Parameters.AddWithValue("@guid", MasterList.user_id)
                        cmdlocal.Parameters.AddWithValue("@store_id", MasterOutletInfo.store_id)
                        cmdlocal.Parameters.AddWithValue("@addontype", .addontype)
                        cmdlocal.Parameters.AddWithValue("@half_batch", .half_batch)
                        cmdlocal.Parameters.AddWithValue("@partners", .partners)
                        cmdlocal.Parameters.AddWithValue("@arrangement", .arrangement)
                        cmdlocal.Parameters.AddWithValue("@synced", "Y")
                        SetProdLastIndex.lastInsertID += 1
                    End With

                    cmdlocal.ExecuteNonQuery()
                End Using
            Next

            rtbLogStats.Text += FullDate24HR() & " :    Complete(Products data insertion)" & vbNewLine
            SetProdStats = SyncCls.ProductsSync.Stats.isProcessComplete
        Catch ex As Exception
            rtbLogStats.Text += FullDate24HR() & " :    Failed(Products data insertion)" & vbNewLine
            SetProdStats = SyncCls.ProductsSync.Stats.isProcessInterrupt
        End Try
    End Sub
    Private Sub InsertToInventory()
        Try
            rtbLogStats.Text += FullDate24HR() & " :    Inserting data to local server's table(Inventories)." & vbNewLine
            SetInvStats = SyncCls.InventorySync.Stats.isProcessing

            With DataGridViewINVENTORY
                Dim ConnectionLocal As MySqlConnection = TestLocalConnection()
                Dim cmdlocal As MySqlCommand
                For i As Integer = SetInvLastIndex.lastInsertID To .Rows.Count - 1 Step +1
                    cmdlocal = New MySqlCommand("INSERT INTO loc_pos_inventory(`server_inventory_id`, `formula_id`, `product_ingredients`, `sku`, `stock_primary`, `stock_secondary`, `stock_no_of_servings`, `stock_status`, `critical_limit`, `server_date_modified`, `store_id`, `guid`, `date_modified`, `crew_id`, `synced`, `main_inventory_id`, `origin`, `show_stockin`)
                                             VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12, @13, @14, @15, @16, @17)", ConnectionLocal)
                    cmdlocal.Parameters.Add("@0", MySqlDbType.Int64).Value = .Rows(i).Cells(0).Value.ToString()
                    cmdlocal.Parameters.Add("@1", MySqlDbType.Int64).Value = 0
                    cmdlocal.Parameters.Add("@2", MySqlDbType.VarChar).Value = .Rows(i).Cells(2).Value.ToString()
                    cmdlocal.Parameters.Add("@3", MySqlDbType.VarChar).Value = .Rows(i).Cells(3).Value.ToString()
                    cmdlocal.Parameters.Add("@4", MySqlDbType.Decimal).Value = .Rows(i).Cells(4).Value.ToString()
                    cmdlocal.Parameters.Add("@5", MySqlDbType.Decimal).Value = .Rows(i).Cells(5).Value.ToString()
                    cmdlocal.Parameters.Add("@6", MySqlDbType.Decimal).Value = .Rows(i).Cells(6).Value.ToString()
                    cmdlocal.Parameters.Add("@7", MySqlDbType.Int64).Value = .Rows(i).Cells(7).Value.ToString()
                    cmdlocal.Parameters.Add("@8", MySqlDbType.Int64).Value = .Rows(i).Cells(8).Value
                    cmdlocal.Parameters.Add("@9", MySqlDbType.Text).Value = .Rows(i).Cells(9).Value
                    cmdlocal.Parameters.Add("@10", MySqlDbType.VarChar).Value = DataGridViewOutlets.SelectedRows(0).Cells(0).Value
                    cmdlocal.Parameters.Add("@11", MySqlDbType.VarChar).Value = MasterList.user_id
                    cmdlocal.Parameters.Add("@12", MySqlDbType.Text).Value = .Rows(i).Cells(9).Value
                    cmdlocal.Parameters.Add("@13", MySqlDbType.VarChar).Value = "0"
                    cmdlocal.Parameters.Add("@14", MySqlDbType.VarChar).Value = "Synced"
                    cmdlocal.Parameters.Add("@15", MySqlDbType.Text).Value = .Rows(i).Cells(10).Value
                    cmdlocal.Parameters.Add("@16", MySqlDbType.Text).Value = .Rows(i).Cells(11).Value
                    cmdlocal.Parameters.Add("@17", MySqlDbType.Int64).Value = .Rows(i).Cells(12).Value
                    cmdlocal.ExecuteNonQuery()
                    SetInvLastIndex.lastInsertID += 1
                Next
                ConnectionLocal.Close()
            End With
            rtbLogStats.Text += FullDate24HR() & " :    Complete(Inventories data insertion)" & vbNewLine
            SetInvStats = SyncCls.InventorySync.Stats.isProcessComplete
        Catch ex As Exception
            rtbLogStats.Text += FullDate24HR() & " :    Failed(Inventories data insertion)" & vbNewLine
            SetInvStats = SyncCls.InventorySync.Stats.isProcessInterrupt

        End Try
    End Sub
    Private Sub InsertToCategories()
        Try
            SetCatStats = SyncCls.CategorySync.Stats.isProcessing

            rtbLogStats.Text += FullDate24HR() & " :    Inserting data to local server's table(Categories)." & vbNewLine
            With DataGridViewCATEGORIES
                Dim ConnectionLocal As MySqlConnection = TestLocalConnection()
                Dim cmdlocal As MySqlCommand
                For i As Integer = SetCatLastIndex.lastInsertID To .Rows.Count - 1 Step +1
                    cmdlocal = New MySqlCommand("INSERT INTO loc_admin_category( `category_name`, `brand_name`, `updated_at`, `origin`, `status`)
                                             VALUES (@0, @1, @2, @3, @4)", ConnectionLocal)
                    cmdlocal.Parameters.Add("@0", MySqlDbType.VarChar).Value = .Rows(i).Cells(0).Value.ToString()
                    cmdlocal.Parameters.Add("@1", MySqlDbType.VarChar).Value = .Rows(i).Cells(1).Value.ToString()
                    cmdlocal.Parameters.Add("@2", MySqlDbType.VarChar).Value = .Rows(i).Cells(2).Value.ToString
                    cmdlocal.Parameters.Add("@3", MySqlDbType.VarChar).Value = .Rows(i).Cells(3).Value.ToString()
                    cmdlocal.Parameters.Add("@4", MySqlDbType.Int64).Value = .Rows(i).Cells(4).Value.ToString()
                    cmdlocal.ExecuteNonQuery()
                    SetCatLastIndex.lastInsertID = i + 1
                Next
                ConnectionLocal.Close()
            End With
            rtbLogStats.Text += FullDate24HR() & " :    Complete(Categories data insertion)" & vbNewLine
            SetCatStats = SyncCls.CategorySync.Stats.isProcessComplete
        Catch ex As Exception
            rtbLogStats.Text += FullDate24HR() & " :    Failed(Categories data insertion)" & vbNewLine
            SetCatStats = SyncCls.CategorySync.Stats.isProcessInterrupt
        End Try
    End Sub
    Private Sub InsertToStockCategory()
        Try
            rtbLogStats.Text += FullDate24HR() & " :    Inserting data to local server's table(Stock Adjustment Categories)." & vbNewLine
            SetStockCatStats = SyncCls.StockCatSync.Stats.isProcessing

            With DataGridViewStockAdjustment
                Dim ConnectionLocal As MySqlConnection = TestLocalConnection()
                Dim cmdlocal As MySqlCommand
                For i As Integer = SetStockCatLastIndex.lastInsertID To .Rows.Count - 1 Step +1
                    cmdlocal = New MySqlCommand("INSERT INTO loc_transfer_data( `transfer_cat`, `crew_id`, `created_at`, `created_by`, `updated_at`, `active`)
                                             VALUES (@0, @1, @2, @3, @4, @5)", ConnectionLocal)
                    cmdlocal.Parameters.Add("@0", MySqlDbType.Text).Value = .Rows(i).Cells(0).Value.ToString()
                    cmdlocal.Parameters.Add("@1", MySqlDbType.Text).Value = "Server"
                    cmdlocal.Parameters.Add("@2", MySqlDbType.Text).Value = .Rows(i).Cells(1).Value.ToString()
                    cmdlocal.Parameters.Add("@3", MySqlDbType.Text).Value = "Server"
                    cmdlocal.Parameters.Add("@4", MySqlDbType.Text).Value = "N/A"
                    cmdlocal.Parameters.Add("@5", MySqlDbType.Int64).Value = .Rows(i).Cells(2).Value.ToString()
                    cmdlocal.ExecuteNonQuery()
                    SetStockCatLastIndex.lastInsertID += 1
                Next
                ConnectionLocal.Close()
            End With
            rtbLogStats.Text += FullDate24HR() & " :    Complete(Stock Adjustment Categories data insertion)" & vbNewLine
            SetStockCatStats = SyncCls.StockCatSync.Stats.isProcessComplete
        Catch ex As Exception
            rtbLogStats.Text += FullDate24HR() & " :    Failed(Stock Adjustment  data insertion)" & vbNewLine
            SetStockCatStats = SyncCls.StockCatSync.Stats.isProcessInterrupt
        End Try
    End Sub
    Private Sub InsertPartnersTransacton()
        Try
            rtbLogStats.Text += FullDate24HR() & " :    Inserting data to local server's table(Partners)." & vbNewLine
            SetPartStats = SyncCls.PartnersSync.Stats.isProcessing

            With DataGridViewPartners
                Dim ConnectionLocal As MySqlConnection = TestLocalConnection()
                Dim cmdlocal As MySqlCommand
                For i As Integer = SetPartLastIndex.lastInsertID To .Rows.Count - 1 Step +1
                    cmdlocal = New MySqlCommand("INSERT INTO loc_partners_transaction(`arrid`, `bankname`, `date_modified`, `crew_id`, `store_id`, `guid`, `active`, `synced`)
                                             VALUES (@0, @1, @2, @3, @4 ,@5 ,@6 ,@7)", ConnectionLocal)
                    cmdlocal.Parameters.Add("@0", MySqlDbType.Int64).Value = .Rows(i).Cells(0).Value.ToString()
                    cmdlocal.Parameters.Add("@1", MySqlDbType.VarChar).Value = .Rows(i).Cells(1).Value.ToString()
                    cmdlocal.Parameters.Add("@2", MySqlDbType.Text).Value = .Rows(i).Cells(2).Value.ToString
                    cmdlocal.Parameters.Add("@3", MySqlDbType.VarChar).Value = ""
                    cmdlocal.Parameters.Add("@4", MySqlDbType.VarChar).Value = DataGridViewOutlets.SelectedRows(0).Cells(0).Value
                    cmdlocal.Parameters.Add("@5", MySqlDbType.VarChar).Value = MasterList.user_id
                    cmdlocal.Parameters.Add("@6", MySqlDbType.Int64).Value = .Rows(i).Cells(3).Value.ToString()
                    cmdlocal.Parameters.Add("@7", MySqlDbType.VarChar).Value = "Synced"
                    cmdlocal.ExecuteNonQuery()
                    SetPartLastIndex.lastInsertID += 1
                Next
                ConnectionLocal.Close()
            End With
            rtbLogStats.Text += FullDate24HR() & " :    Complete(Partners data insertion)" & vbNewLine
            SetPartStats = SyncCls.PartnersSync.Stats.isProcessComplete
        Catch ex As Exception
            rtbLogStats.Text += FullDate24HR() & " :    Failed(Partners data insertion)" & vbNewLine
            SetPartStats = SyncCls.PartnersSync.Stats.isProcessInterrupt
        End Try
    End Sub
    Private Sub InsertCoupons()
        Try

            rtbLogStats.Text += FullDate24HR() & " :    Inserting data to local server's table(Coupons)." & vbNewLine
            SetCoupStats = SyncCls.CouponSync.Stats.isProcessing

            With DataGridViewCoupons
                Dim ConnectionLocal As MySqlConnection = TestLocalConnection()
                Dim cmdlocal As MySqlCommand
                For i As Integer = SetCoupLastIndex.lastInsertID To .Rows.Count - 1 Step +1
                    cmdlocal = New MySqlCommand("INSERT INTO tbcoupon(`Couponname_`, `Desc_`, `Discountvalue_`, `Referencevalue_`, `Type`, `Bundlebase_`, `BBValue_`, `Bundlepromo_`, `BPValue_`, `Effectivedate`, `Expirydate`, `date_created`, `store_id`, `crew_id`, `guid`, `origin`, `synced`, `active`)
                                             VALUES (@0, @1, @2, @3, @4 ,@5 ,@6 ,@7 ,@8 ,@9 ,@10 ,@11 ,@12 ,@13, @14, @15, @16, @17)", ConnectionLocal)
                    cmdlocal.Parameters.Add("@0", MySqlDbType.Text).Value = .Rows(i).Cells(0).Value.ToString()
                    cmdlocal.Parameters.Add("@1", MySqlDbType.Text).Value = .Rows(i).Cells(1).Value.ToString()
                    cmdlocal.Parameters.Add("@2", MySqlDbType.Text).Value = .Rows(i).Cells(2).Value.ToString
                    cmdlocal.Parameters.Add("@3", MySqlDbType.Text).Value = .Rows(i).Cells(3).Value.ToString
                    cmdlocal.Parameters.Add("@4", MySqlDbType.Text).Value = .Rows(i).Cells(4).Value.ToString
                    cmdlocal.Parameters.Add("@5", MySqlDbType.Text).Value = .Rows(i).Cells(5).Value.ToString
                    cmdlocal.Parameters.Add("@6", MySqlDbType.Text).Value = .Rows(i).Cells(6).Value.ToString
                    cmdlocal.Parameters.Add("@7", MySqlDbType.Text).Value = .Rows(i).Cells(7).Value.ToString
                    cmdlocal.Parameters.Add("@8", MySqlDbType.Text).Value = .Rows(i).Cells(8).Value.ToString
                    cmdlocal.Parameters.Add("@9", MySqlDbType.Text).Value = .Rows(i).Cells(9).Value.ToString
                    cmdlocal.Parameters.Add("@10", MySqlDbType.Text).Value = .Rows(i).Cells(10).Value.ToString
                    cmdlocal.Parameters.Add("@11", MySqlDbType.Text).Value = .Rows(i).Cells(11).Value.ToString
                    cmdlocal.Parameters.Add("@12", MySqlDbType.Text).Value = DataGridViewOutlets.SelectedRows(0).Cells(0).Value
                    cmdlocal.Parameters.Add("@13", MySqlDbType.Text).Value = ""
                    cmdlocal.Parameters.Add("@14", MySqlDbType.Text).Value = MasterList.user_id
                    cmdlocal.Parameters.Add("@15", MySqlDbType.Text).Value = "Server"
                    cmdlocal.Parameters.Add("@16", MySqlDbType.Text).Value = "Synced"
                    cmdlocal.Parameters.Add("@17", MySqlDbType.Text).Value = .Rows(i).Cells(12).Value.ToString
                    cmdlocal.ExecuteNonQuery()
                    SetCoupLastIndex.lastInsertID += 1
                Next
                ConnectionLocal.Close()
            End With
            rtbLogStats.Text += FullDate24HR() & " :    Complete(Coupons data insertion)" & vbNewLine
            SetCoupStats = SyncCls.CouponSync.Stats.isProcessComplete
        Catch ex As Exception
            rtbLogStats.Text += FullDate24HR() & " :    Failed(Coupons data insertion)" & vbNewLine
            SetCoupStats = SyncCls.CouponSync.Stats.isProcessInterrupt
        End Try
    End Sub
    Private Sub InsertToFormula()
        Try
            rtbLogStats.Text += FullDate24HR() & " :    Inserting data to local server's table(Formulas)." & vbNewLine
            SetFormStats = SyncCls.FormulaSync.Stats.isProcessing

            With DataGridViewFORMULA
                Dim ConnectionLocal As MySqlConnection = TestLocalConnection()
                Dim cmdlocal As MySqlCommand
                For i As Integer = SetFormLastIndex.lastInsertID To .Rows.Count - 1 Step +1
                    cmdlocal = New MySqlCommand("INSERT INTO loc_product_formula(`server_formula_id`, `product_ingredients`, `primary_unit`, `primary_value`, `secondary_unit`, `secondary_value`, `serving_unit`, `serving_value`, `no_servings`, `status`, `date_modified`, `unit_cost`, `origin`, `server_date_modified`, `store_id`, `guid`)
                                             VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12, @13, @14, @15)", ConnectionLocal)
                    cmdlocal.Parameters.Add("@0", MySqlDbType.Int64).Value = .Rows(i).Cells(0).Value.ToString()
                    cmdlocal.Parameters.Add("@1", MySqlDbType.VarChar).Value = .Rows(i).Cells(1).Value.ToString()
                    cmdlocal.Parameters.Add("@2", MySqlDbType.VarChar).Value = .Rows(i).Cells(2).Value.ToString()
                    cmdlocal.Parameters.Add("@3", MySqlDbType.VarChar).Value = .Rows(i).Cells(3).Value.ToString()
                    cmdlocal.Parameters.Add("@4", MySqlDbType.VarChar).Value = .Rows(i).Cells(4).Value.ToString()
                    cmdlocal.Parameters.Add("@5", MySqlDbType.VarChar).Value = .Rows(i).Cells(5).Value.ToString()
                    cmdlocal.Parameters.Add("@6", MySqlDbType.VarChar).Value = .Rows(i).Cells(6).Value.ToString()
                    cmdlocal.Parameters.Add("@7", MySqlDbType.VarChar).Value = .Rows(i).Cells(7).Value.ToString()
                    cmdlocal.Parameters.Add("@8", MySqlDbType.VarChar).Value = .Rows(i).Cells(8).Value.ToString()
                    cmdlocal.Parameters.Add("@9", MySqlDbType.Int64).Value = .Rows(i).Cells(9).Value.ToString()
                    cmdlocal.Parameters.Add("@10", MySqlDbType.VarChar).Value = .Rows(i).Cells(10).Value
                    cmdlocal.Parameters.Add("@11", MySqlDbType.Decimal).Value = .Rows(i).Cells(11).Value.ToString()
                    cmdlocal.Parameters.Add("@12", MySqlDbType.VarChar).Value = .Rows(i).Cells(12).Value.ToString()
                    cmdlocal.Parameters.Add("@13", MySqlDbType.VarChar).Value = .Rows(i).Cells(10).Value
                    cmdlocal.Parameters.Add("@14", MySqlDbType.VarChar).Value = DataGridViewOutlets.SelectedRows(0).Cells(0).Value
                    cmdlocal.Parameters.Add("@15", MySqlDbType.VarChar).Value = MasterList.user_id
                    cmdlocal.ExecuteNonQuery()
                    SetFormLastIndex.lastInsertID += 1
                Next
                ConnectionLocal.Close()
            End With
            rtbLogStats.Text += FullDate24HR() & " :    Complete(Formulas data insertion)" & vbNewLine
            SetFormStats = SyncCls.FormulaSync.Stats.isProcessComplete
        Catch ex As Exception
            rtbLogStats.Text += FullDate24HR() & " :    Failed(Formulas data insertion)" & vbNewLine
            SetFormStats = SyncCls.FormulaSync.Stats.isProcessInterrupt

        End Try
    End Sub

    Private Sub InsertToReceiptInfo()
        Try
            rtbLogStats.Text += FullDate24HR() & " :    Inserting data to local server's table(Receipt Info)" & vbNewLine
            SetReceiptInfoStats = SyncCls.ReceiptInfoSync.Stats.isProcessing

            Dim ConnectionLocal As MySqlConnection = TestLocalConnection()

            For Each rp As ReceiptInfoCls In listOfReceiptInfo.Skip(SetReceiptInfoLastIndex.lastInsertID)

                Dim Query = "INSERT INTO loc_receipt 
                                (
                                    `type`, `description`, `created_by`, `created_at`, `updated_by`, `updated_at`, `status`
                                )
                             VALUES 
                                (
                                    @type, @description, @created_by, @created_at, @updated_by, @updated_at, @status
                                )"
                Using cmdlocal As New MySqlCommand(Query, ConnectionLocal)
                    cmdlocal.Parameters.Clear()
                    With rp
                        cmdlocal.Parameters.AddWithValue("@type", .Code)
                        cmdlocal.Parameters.AddWithValue("@description", .Description)
                        cmdlocal.Parameters.AddWithValue("@created_by", "Server")
                        cmdlocal.Parameters.AddWithValue("@created_at", FullDate24HR())
                        cmdlocal.Parameters.AddWithValue("@updated_by", "Server")
                        cmdlocal.Parameters.AddWithValue("@updated_at", FullDate24HR())
                        cmdlocal.Parameters.AddWithValue("@status", .Status)
                        SetReceiptInfoLastIndex.lastInsertID += 1
                    End With

                    cmdlocal.ExecuteNonQuery()
                End Using
            Next

            rtbLogStats.Text += FullDate24HR() & " :    Complete(Receipt Info data insertion)" & vbNewLine
            SetReceiptInfoStats = SyncCls.ReceiptInfoSync.Stats.isProcessComplete
        Catch ex As Exception
            rtbLogStats.Text += FullDate24HR() & " :    Failed(Products data insertion)" & vbNewLine
            SetReceiptInfoStats = SyncCls.ReceiptInfoSync.Stats.isProcessInterrupt
        End Try
    End Sub
    Private Sub SerialKey()
        Try
            ValidProductKey = False
            Dim CloudConnection As MySqlConnection = TestCloudConnection()
            Dim sql = "SELECT serial_key FROM admin_serialkeys WHERE active = 0 AND serial_key = @serialKey"
            Using cloudcmd As New MySqlCommand(sql, CloudConnection)
                cloudcmd.Parameters.Clear()
                cloudcmd.Parameters.AddWithValue("@serialKey", Trim(TextBoxProdKey.Text))
                Dim reader = cloudcmd.ExecuteReader
                If reader.HasRows Then
                    ValidProductKey = True
                End If
                cloudcmd.Dispose()
            End Using
            CloudConnection.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub adminserialkey()
        Try

            rtbLogStats.Text += FullDate24HR() & " :    Updating cloud server's table(Product Key)." & vbNewLine
            If TestModeIsOFF Then
                Dim CloudConnection As MySqlConnection = TestCloudConnection()
                Dim sql = "UPDATE admin_serialkeys SET active = @1 , date_used = @2 WHERE serial_key = @3"
                Using cloudcmd As MySqlCommand = New MySqlCommand(sql, CloudConnection)
                    cloudcmd.Parameters.Clear()
                    cloudcmd.Parameters.AddWithValue("@1", 1)
                    cloudcmd.Parameters.AddWithValue("@2", FullDate24HR())
                    cloudcmd.Parameters.AddWithValue("@3", Trim(TextBoxProdKey.Text))
                    cloudcmd.ExecuteNonQuery()
                    cloudcmd.Dispose()
                    CloudConnection.Close()
                End Using
            End If
            rtbLogStats.Text += FullDate24HR() & " :    Complete(Product key table updated)." & vbNewLine
        Catch ex As Exception
            rtbLogStats.Text += FullDate24HR() & " :    Failed(Updating of product key)." & vbNewLine

        End Try
    End Sub
    Public Sub adminoutlets()
        Try
            rtbLogStats.Text += FullDate24HR() & " :    Updating cloud server's table(Outlets)." & vbNewLine
            If TestModeIsOFF Then
                Dim CloudConnection As MySqlConnection = TestCloudConnection()
                Dim sql = "UPDATE admin_outlets SET active = @1 WHERE store_id = @storeID "
                Using cloudcmd As New MySqlCommand(sql, CloudConnection)
                    cloudcmd.Parameters.Clear()
                    cloudcmd.Parameters.AddWithValue("@1", 2)
                    cloudcmd.Parameters.AddWithValue("@storeID", MasterOutletInfo.store_id)
                    cloudcmd.ExecuteNonQuery()
                    cloudcmd.Dispose()
                    CloudConnection.Close()
                End Using

            End If
            rtbLogStats.Text += FullDate24HR() & " :    Complete(Outlets table updated)." & vbNewLine
        Catch ex As Exception
            rtbLogStats.Text += FullDate24HR() & " :    Failed(Updating of outlet)." & vbNewLine

        End Try
    End Sub
    Public Sub insertintocloud()
        Try
            rtbLogStats.Text += $"{FullDate24HR()} :    Inserting data to cloud server's table(Masterlist).{vbNewLine}"
            If TestModeIsOFF Then
                Dim CloudConnection As MySqlConnection = TestCloudConnection()
                Dim sql = "INSERT INTO admin_masterlist (`masterlist_username`,`client_guid`,`client_product_key`,`user_id`,`client_store_id`) VALUES (@1,@2,@3,@4,@5)"
                Using cloudcmd As New MySqlCommand(sql, CloudConnection)
                    cloudcmd.Parameters.Clear()
                    cloudcmd.Parameters.AddWithValue("@1", TextBoxFrancUser.Text)
                    cloudcmd.Parameters.AddWithValue("@2", MasterList.user_id)
                    cloudcmd.Parameters.AddWithValue("@3", TextBoxProdKey.Text)
                    cloudcmd.Parameters.AddWithValue("@4", MasterList.user_id)
                    cloudcmd.Parameters.AddWithValue("@5", MasterOutletInfo.store_id)
                    cloudcmd.ExecuteNonQuery()
                    cloudcmd.Dispose()
                    CloudConnection.Close()
                End Using
            End If
            rtbLogStats.Text += $"{FullDate24HR() } :    Complete(Masterlist data inserted).{vbNewLine}"
        Catch ex As Exception
            rtbLogStats.Text += $"{FullDate24HR() } :    Failed(Masterlist data insertion(Cloud)).{vbNewLine}"
        End Try
    End Sub
    Private Sub insertintolocaloutlets()
        Try
            rtbLogStats.Text += $"{FullDate24HR()} :    Getting cloud server's outlet data.{vbNewLine}"
            With MasterOutletInfo
                rtbLogStats.Text += $"{FullDate24HR()} :    Inserting outlet data..{vbNewLine}"
                Dim Query = "INSERT INTO admin_outlets 
                                (
                                    `store_id`, `brand_name`, `store_name`, `location_name`, `postal_code`, 
                                    `address`, `Barangay`, `municipality`, `municipality_name`, `province`, `province_name`, 
                                    `tin_no`, `tel_no`, `MIN`, `MSN`, `PTUN`
                                ) 
                             VALUES 
                                (
                                    @store_id, @brand_name, @store_name, @location_name, @postal_code, 
                                    @address, @Barangay, @municipality, @municipality_name, @province, @province_name,
                                    @tin_no, @tel_no, @MIN, @MSN, @PTUN
                                )"
                Dim Connectionlocal As MySqlConnection = TestLocalConnection()
                Using cmd As New MySqlCommand(Query, Connectionlocal)
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@store_id", .store_id)
                    cmd.Parameters.AddWithValue("@brand_name", .brand_name)
                    cmd.Parameters.AddWithValue("@store_name", .store_name)
                    cmd.Parameters.AddWithValue("@location_name", .location_name)
                    cmd.Parameters.AddWithValue("@postal_code", .postal_code)
                    cmd.Parameters.AddWithValue("@address", .address)
                    cmd.Parameters.AddWithValue("@Barangay", .Barangay)
                    cmd.Parameters.AddWithValue("@municipality", .municipality)
                    cmd.Parameters.AddWithValue("@municipality_name", .municipality_name)
                    cmd.Parameters.AddWithValue("@province", .province)
                    cmd.Parameters.AddWithValue("@province_name", .province_name)
                    cmd.Parameters.AddWithValue("@tin_no", .tin_no)
                    cmd.Parameters.AddWithValue("@tel_no", .tel_no)
                    cmd.Parameters.AddWithValue("@MIN", .MIN)
                    cmd.Parameters.AddWithValue("@MSN", .MSN)
                    cmd.Parameters.AddWithValue("@PTUN", .PTUN)
                    cmd.ExecuteNonQuery()
                End Using
                rtbLogStats.Text += $"{FullDate24HR()} :    Complete(Outlet data inserted).{vbNewLine}"
            End With
        Catch ex As Exception
            rtbLogStats.Text += $"{FullDate24HR()} :     Failed(Outlet data insertion(Local)).{vbNewLine}"
        End Try
    End Sub
    Private Sub SaveLogo()
        Try
            Dim Connectionlocal As MySqlConnection = TestLocalConnection()
            Dim sql = "UPDATE loc_settings SET S_logo = @logo WHERE settings_id = 1"
            Using cmd As New MySqlCommand(sql, Connectionlocal)
                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("@logo", RichTextBoxLogo.Text)
                cmd.ExecuteNonQuery()
                cmd.Dispose()
            End Using
            Connectionlocal.Close()
        Catch ex As Exception
            MsgBox("Contact Administrator Error Code: 3.0")
        End Try
    End Sub
    Private Sub InsertLocalMasterList()
        Try
            rtbLogStats.Text += FullDate24HR() & " :    Inserting masterlist data." & vbNewLine
            Dim Connectionlocal As MySqlConnection = TestLocalConnection()
            Dim sql = "INSERT INTO admin_masterlist 
                            (
                                `masterlist_username`,`client_product_key`,`user_id`,`client_store_id`
                            ) 
                       VALUES 
                            (
                                @masterlist_username, @client_product_key, @user_id, @client_store_id
                            )"

            Using cmd As New MySqlCommand(sql, Connectionlocal)
                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("@masterlist_username", Trim(TextBoxFrancUser.Text))
                cmd.Parameters.AddWithValue("@client_product_key", TextBoxProdKey.Text)
                cmd.Parameters.AddWithValue("@user_id", MasterList.user_id)
                cmd.Parameters.AddWithValue("@client_store_id", MasterOutletInfo.store_id)
                cmd.ExecuteNonQuery()
                Connectionlocal.Close()
            End Using

            rtbLogStats.Text += FullDate24HR() & " :    Complete(Masterlist data inserted)." & vbNewLine
        Catch ex As Exception
            rtbLogStats.Text += FullDate24HR() & " :    Failed(Masterlist data insertion)." & vbNewLine
            MsgBox("Contact Administrator Error Code: 3.0")
        End Try
    End Sub
#End Region
#End Region

#Region "System Database Option"
    Dim optimizeorrepair As Integer = 0
    Private Sub ButtonMaintenance_Click(sender As Object, e As EventArgs) Handles ButtonMaintenance.Click
        optimizeorrepair = 1
        BackgroundWorkerABTDB.WorkerReportsProgress = True
        BackgroundWorkerABTDB.WorkerSupportsCancellation = True
        BackgroundWorkerABTDB.RunWorkerAsync()
    End Sub
    Private Sub ButtonRepair_Click(sender As Object, e As EventArgs) Handles ButtonRepair.Click
        optimizeorrepair = 0
        BackgroundWorkerABTDB.WorkerReportsProgress = True
        BackgroundWorkerABTDB.WorkerSupportsCancellation = True
        BackgroundWorkerABTDB.RunWorkerAsync()
    End Sub
    Private Sub ButtonExport_Click(sender As Object, e As EventArgs) Handles ButtonExport.Click
        optimizeorrepair = 2
        BackgroundWorkerABTDB.WorkerReportsProgress = True
        BackgroundWorkerABTDB.WorkerSupportsCancellation = True
        BackgroundWorkerABTDB.RunWorkerAsync()
    End Sub
    Property bat As String
    Property threadABTDB As Thread
    Property threadListABTDB As List(Of Thread) = New List(Of Thread)
    Private Sub BackgroundWorkerABTDB_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorkerABTDB.DoWork
        Try
            If optimizeorrepair = 0 Then
                bat = "repair.bat"
                threadABTDB = New Thread(AddressOf StartCommandLine)
                threadABTDB.Start()
            ElseIf optimizeorrepair = 1 Then
                bat = "optimize.bat"
                threadABTDB = New Thread(AddressOf StartCommandLine)
                threadABTDB.Start()
            ElseIf optimizeorrepair = 2 Then
                bat = "backup.bat"
                threadABTDB = New Thread(AddressOf StartCommandLine)
                threadABTDB.Start()
            End If
            For Each t In threadListABTDB
                t.Join()
            Next
        Catch ex As Exception

        End Try
    End Sub
    Public Sub StartCommandLine(ByVal batfile As String)
        Try
            batfile = bat
            Dim p As Process = New Process()
            Dim psi As ProcessStartInfo = New ProcessStartInfo()
            psi.FileName = "CMD.EXE"
            psi.Arguments = "/K " & batfile
            p.StartInfo = psi
            p.Start()
            p.WaitForExit()
        Catch ex As Exception

        End Try
    End Sub
#End Region

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles ButtonEditAccount.Click
        TextboxEnableability(Panel14, True)
        AccountExist = False
        DataGridViewOutlets.Rows.Clear()
        FranchiseeStoreValidation = False
        DataGridViewOutlets.DataSource = Nothing
        MasterOutletInfo = New MasterlistOutletCls
        ClearTextBox(Panel15)
    End Sub
    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles ButtonSaveLocalCon.Click
        SaveLocalConnection()
    End Sub

    Private Sub SaveLocalConnection()
        Try
            If ValidLocalConnection Then
                Dim FolderName As String = "Innovention"
                Dim path = My.Computer.FileSystem.SpecialDirectories.MyDocuments
                CreateFolder(path, FolderName)
                BTNSaveLocalConn = True
                TextboxEnableability(Panel5, False)
                ButtonClearLocal.Enabled = False
                ButtonTestLocConn.Enabled = False
            Else
                MsgBox("Connection must be valid")
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub TextBoxLocalServer_TextChanged(sender As Object, e As EventArgs) Handles TextBoxLocalUsername.TextChanged, TextBoxLocalServer.TextChanged, TextBoxLocalPort.TextChanged, TextBoxLocalPassword.TextChanged, TextBoxLocalDatabase.TextChanged
        Try
            BTNSaveLocalConn = False
            ValidLocalConnection = False
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TextBoxCloudServer_TextChanged(sender As Object, e As EventArgs) Handles TextBoxCloudUsername.TextChanged, TextBoxCloudServer.TextChanged, TextBoxCloudPort.TextChanged, TextBoxCloudPassword.TextChanged, TextBoxCloudDatabase.TextChanged
        Try
            BTNSaveCloudConn = False
            ValidCloudConnection = False
            My.Settings.Save()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub TextBoxDEVPTU_TextChanged(sender As Object, e As EventArgs) Handles TextBoxDevTIN.TextChanged, TextBoxDEVPTU.TextChanged, TextBoxDevname.TextChanged, TextBoxDevAdd.TextChanged, TextBoxDevAccr.TextChanged
        Try
            ConfirmDevInfoSettings = False
            My.Settings.Save()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub DateTimePicker1ACCRDI_ValueChanged(sender As Object, e As EventArgs) Handles DateTimePickerPTUVU.ValueChanged, DateTimePicker4PTUDI.ValueChanged, DateTimePicker2ACCRVU.ValueChanged, DateTimePicker1ACCRDI.ValueChanged
        Try
            ConfirmDevInfoSettings = False
        Catch ex As Exception

        End Try
    End Sub
    Private Sub TextBoxExportPath_TextChanged(sender As Object, e As EventArgs) Handles TextBoxTerminalNo.TextChanged, TextBoxTax.TextChanged, TextBoxSINumber.TextChanged, TextBoxExportPath.TextChanged
        Try
            ConfirmAdditionalSettings = False
            My.Settings.Save()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub RadioButtonYES_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButtonYES.CheckedChanged, RadioButtonNO.CheckedChanged
        Try
            ConfirmAdditionalSettings = False
            My.Settings.Save()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub RadioButtonDaily_Click(sender As Object, e As EventArgs) Handles RadioButtonYearly.Click, RadioButtonWeekly.Click, RadioButtonMonthly.Click, RadioButtonDaily.Click
        Try
            If ValidLocalConnection = True Then
                Dim Interval As Integer = 0
                Dim IntervalName As String = ""
                If RadioButtonDaily.Checked = True Then
                    Interval = 1
                    IntervalName = "Daily"
                ElseIf RadioButtonWeekly.Checked = True Then
                    Interval = 2
                    IntervalName = "Weekly"
                ElseIf RadioButtonMonthly.Checked = True Then
                    Interval = 3
                    IntervalName = "Monthly"
                ElseIf RadioButtonYearly.Checked = True Then
                    Interval = 4
                    IntervalName = "Yearly"
                End If
                Dim ConnectionLocal As MySqlConnection = TestLocalConnection()

                Dim sql = "SELECT `S_BackupInterval` , `S_BackupDate` FROM loc_settings WHERE settings_id = 1"
                Dim cmd As MySqlCommand = New MySqlCommand(sql, ConnectionLocal)
                Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
                Dim dt As DataTable = New DataTable
                da.Fill(dt)
                If dt.Rows.Count > 0 Then
                    If Interval = 1 Then
                        sql = "UPDATE loc_settings SET `S_BackupInterval` = " & Interval & " , `S_BackupDate` = '" & Format(Now().AddDays(1), "yyyy-MM-dd") & "'"
                    ElseIf Interval = 2 Then
                        sql = "UPDATE loc_settings SET `S_BackupInterval` = " & Interval & " , `S_BackupDate` = '" & Format(Now().AddDays(7), "yyyy-MM-dd") & "'"
                    ElseIf Interval = 3 Then
                        sql = "UPDATE loc_settings SET `S_BackupInterval` = " & Interval & " , `S_BackupDate` = '" & Format(Now().AddMonths(1), "yyyy-MM-dd") & "'"
                    ElseIf Interval = 4 Then
                        sql = "UPDATE loc_settings SET `S_BackupInterval` = " & Interval & " , `S_BackupDate` = '" & Format(Now().AddYears(1), "yyyy-MM-dd") & "'"
                    End If
                    cmd = New MySqlCommand(sql, ConnectionLocal)
                    cmd.ExecuteNonQuery()
                    Autobackup = True
                Else
                    If Interval = 1 Then
                        sql = "INSERT INTO loc_settings (`S_BackupInterval` , `S_BackupDate`) VALUES ('" & Interval & "','" & Format(Now().AddDays(1), "yyyy-MM-dd") & "')"
                    ElseIf Interval = 2 Then
                        sql = "INSERT INTO loc_settings (`S_BackupInterval` , `S_BackupDate`) VALUES ('" & Interval & "','" & Format(Now().AddDays(7), "yyyy-MM-dd") & "')"
                    ElseIf Interval = 3 Then
                        sql = "INSERT INTO loc_settings (`S_BackupInterval` , `S_BackupDate`) VALUES ('" & Interval & "','" & Format(Now().AddMonths(1), "yyyy-MM-dd") & "')"
                    ElseIf Interval = 4 Then
                        sql = "INSERT INTO loc_settings (`S_BackupInterval` , `S_BackupDate`) VALUES ('" & Interval & "','" & Format(Now().AddYears(1), "yyyy-MM-dd") & "')"
                    End If
                    cmd = New MySqlCommand(sql, ConnectionLocal)
                    cmd.ExecuteNonQuery()
                    Autobackup = True
                End If
                MsgBox("Automatic system backup set to " & IntervalName & " backup")
                ConnectionLocal.Close()
            Else
                Autobackup = False
                MsgBox("Local connection must be valid first.")
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TextBoxLocalDatabase_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBoxLocalUsername.KeyPress, TextBoxLocalServer.KeyPress, TextBoxLocalPort.KeyPress, TextBoxLocalPassword.KeyPress, TextBoxLocalDatabase.KeyPress, TextBoxCloudUsername.KeyPress, TextBoxCloudServer.KeyPress, TextBoxCloudPort.KeyPress, TextBoxCloudPassword.KeyPress, TextBoxCloudDatabase.KeyPress, TextBoxTerminalNo.KeyPress, TextBoxTax.KeyPress, TextBoxSINumber.KeyPress, TextBoxExportPath.KeyPress, TextBoxDevTIN.KeyPress, TextBoxDEVPTU.KeyPress, TextBoxDevname.KeyPress, TextBoxDevAdd.KeyPress, TextBoxDevAccr.KeyPress, TextBoxFrancUser.KeyPress, TextBoxProdKey.KeyPress
        Try
            If InStr(DisallowedCharacters, e.KeyChar) > 0 Then
                e.Handled = True
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ButtonImportCloudConn_Click(sender As Object, e As EventArgs) Handles ButtonImportCloudConn.Click
        Try
            Dim myOpenFileDialog As New OpenFileDialog()

            myOpenFileDialog.CheckFileExists = True
            myOpenFileDialog.DefaultExt = "txt"
            myOpenFileDialog.InitialDirectory = "C:\"
            myOpenFileDialog.Multiselect = False
            If myOpenFileDialog.ShowDialog = DialogResult.OK Then
                Dim ImportPath = myOpenFileDialog.FileName
                Dim TextLine As String = ""
                Dim objReader As New StreamReader(ImportPath)
                Dim lineCount As Integer
                Do While objReader.Peek() <> -1
                    TextLine = objReader.ReadLine()
                    If lineCount = 0 Then
                        TextBoxCloudServer.Text = RemoveCharacter(TextLine, "server=")
                    End If
                    If lineCount = 1 Then
                        TextBoxCloudUsername.Text = RemoveCharacter(TextLine, "user id=")
                    End If
                    If lineCount = 2 Then
                        TextBoxCloudPassword.Text = RemoveCharacter(TextLine, "password=")
                    End If
                    If lineCount = 3 Then
                        TextBoxCloudDatabase.Text = RemoveCharacter(TextLine, "database=")
                    End If
                    If lineCount = 4 Then
                        TextBoxCloudPort.Text = RemoveCharacter(TextLine, "port=")
                    End If
                    lineCount = lineCount + 1
                Loop
                objReader.Close()
            End If
        Catch ex As Exception

        End Try
    End Sub
    Dim ImagePath As String
    Private Sub ButtonBrowseLogo_Click(sender As Object, e As EventArgs) Handles ButtonBrowseLogo.Click
        Try
            With OpenFileDialog1
                .Filter = ("Images | *.png; *.bmp; *.jpg; *.jpeg; *.gif; *.ico;")
                .FilterIndex = 4
            End With
            OpenFileDialog1.FileName = ""
            If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
                If My.Computer.FileSystem.FileExists(ImagePath) Then
                    convertimage()
                End If
                PictureBoxLogo.Image = Image.FromFile(OpenFileDialog1.FileName)
                PictureBoxLogo.SizeMode = PictureBoxSizeMode.StretchImage
            End If
        Catch ex As Exception

            AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub OpenFileDialog1_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk
        ImagePath = OpenFileDialog1.FileName
    End Sub
    Dim encodeType As ImageFormat = ImageFormat.Jpeg
    Dim decodingstring As String = String.Empty
    Private Sub convertimage()
        Try
            RichTextBoxLogo.Clear()
            Dim ImageToConvert As Bitmap = Bitmap.FromFile(ImagePath)
            ImageToConvert.MakeTransparent()
            Dim encoding As String = String.Empty
            If ImagePath.ToLower.EndsWith(".jpg") Then
                encodeType = ImageFormat.Jpeg
            ElseIf ImagePath.ToLower.EndsWith(".png") Then
                encodeType = ImageFormat.Png
            ElseIf ImagePath.ToLower.EndsWith(".gif") Then
                encodeType = ImageFormat.Gif
            ElseIf ImagePath.ToLower.EndsWith(".bmp") Then
                encodeType = ImageFormat.Bmp
            End If
            decodingstring = encoding
            RichTextBoxLogo.Text = ImageToBase64(ImageToConvert, encodeType)
        Catch ex As Exception

            AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
        End Try
    End Sub
#Region "Printing Receipt Options"
    Property PrintOptionIsSet As Boolean = False
    Property PrintOption As String = ""
    Private Sub RadioButtonPrintReceiptYes_Click(sender As Object, e As EventArgs) Handles RadioButtonPrintReceiptYes.Click, RadioButtonPrintReceiptNo.Click
        Try
            Dim table = "`loc_settings`"
            If ValidLocalConnection Then
                If RadioButtonPrintReceiptYes.Checked Then
                    PrintOption = "YES"
                    PrintOptionIsSet = True
                ElseIf RadioButtonPrintReceiptNo.Checked Then
                    PrintOption = "NO"
                    PrintOptionIsSet = True
                Else
                    PrintOptionIsSet = False
                    PrintOption = ""
                End If
                If PrintOptionIsSet Then
                    Dim ConnectionLocal As MySqlConnection = TestLocalConnection()
                    Dim sql = "SELECT `printreceipt` FROM " & table & " WHERE `settings_id` = 1"
                    Dim cmd As MySqlCommand = New MySqlCommand(sql, ConnectionLocal)
                    Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
                    Dim dt As DataTable = New DataTable
                    da.Fill(dt)
                    If dt.Rows.Count > 0 Then
                        Dim fields = "`printreceipt` = '" & PrintOption & "' "
                        sql = "UPDATE " & table & " SET " & fields & " WHERE `settings_id` = 1"
                        cmd = New MySqlCommand(sql, ConnectionLocal)
                        cmd.ExecuteNonQuery()
                    Else
                        Dim fields = "`printreceipt`"
                        Dim value = "'" & PrintOption & "'"
                        sql = "INSERT INTO " & table & " (" & fields & ") VALUES (" & value & ")"
                        cmd = New MySqlCommand(sql, ConnectionLocal)
                        cmd.ExecuteNonQuery()
                    End If
                    ConnectionLocal.Close()
                Else
                    MsgBox("Select option first")
                    PrintOptionIsSet = False
                    PrintOption = ""
                End If
            Else
                MsgBox("Connection must be valid first")
                PrintOptionIsSet = False
                PrintOption = ""
            End If
        Catch ex As Exception

            PrintOptionIsSet = False
            PrintOption = ""
        End Try
    End Sub
    Property RePrintOptionIsSet As Boolean = False
    Property RePrintOption As String = ""
    Private Sub RadioButtonRePrintReceiptYes_Click(sender As Object, e As EventArgs) Handles RadioButtonRePrintReceiptYes.Click, RadioButtonRePrintReceiptNo.Click
        Try
            Dim table = "`loc_settings`"
            If ValidLocalConnection Then
                If RadioButtonRePrintReceiptYes.Checked Then
                    RePrintOption = "YES"
                    RePrintOptionIsSet = True
                ElseIf RadioButtonRePrintReceiptNo.Checked Then
                    RePrintOption = "NO"
                    RePrintOptionIsSet = True
                Else
                    RePrintOption = ""
                    RePrintOptionIsSet = False
                End If
                If RePrintOptionIsSet Then
                    Dim ConnectionLocal As MySqlConnection = TestLocalConnection()
                    Dim sql = "Select `reprintreceipt` FROM " & table & " WHERE `settings_id` = 1"
                    Dim cmd As MySqlCommand = New MySqlCommand(sql, ConnectionLocal)
                    Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
                    Dim dt As DataTable = New DataTable
                    da.Fill(dt)
                    If dt.Rows.Count > 0 Then
                        Dim fields = "`reprintreceipt` = '" & RePrintOption & "' "
                        sql = "UPDATE " & table & " SET " & fields & " WHERE `settings_id` = 1"
                        cmd = New MySqlCommand(sql, ConnectionLocal)
                        cmd.ExecuteNonQuery()
                    Else
                        Dim fields = "`reprintreceipt`"
                        Dim value = "'" & RePrintOption & "'"
                        sql = "INSERT INTO " & table & " (" & fields & ") VALUES (" & value & ")"
                        cmd = New MySqlCommand(sql, ConnectionLocal)
                        cmd.ExecuteNonQuery()
                    End If
                    ConnectionLocal.Close()
                Else
                    MsgBox("Select option first")
                    RePrintOptionIsSet = False
                    RePrintOption = ""
                End If
            Else
                MsgBox("Connection must be valid first")
                RePrintOption = ""
                RePrintOptionIsSet = False
            End If
        Catch ex As Exception

            RePrintOption = ""
            RePrintOptionIsSet = False
        End Try
    End Sub
    Property PrintXZRead As Boolean = False
    Property PrintXZReadOption As String = ""
    Private Sub RadioButtonPrintXZReadYes_Click(sender As Object, e As EventArgs) Handles RadioButtonPrintXZReadYes.Click, RadioButtonPrintXZReadNo.Click
        Try
            Dim table = "`loc_settings`"
            If ValidLocalConnection Then
                If RadioButtonPrintXZReadYes.Checked Then
                    PrintXZReadOption = "YES"
                    PrintXZRead = True
                ElseIf RadioButtonPrintXZReadNo.Checked Then
                    PrintXZReadOption = "NO"
                    PrintXZRead = True
                Else
                    PrintXZReadOption = ""
                    PrintXZRead = False
                End If
                If PrintXZRead Then
                    Dim ConnectionLocal As MySqlConnection = TestLocalConnection()
                    Dim sql = "Select `printxzread` FROM " & table & " WHERE `settings_id` = 1"
                    Dim cmd As MySqlCommand = New MySqlCommand(sql, ConnectionLocal)
                    Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
                    Dim dt As DataTable = New DataTable
                    da.Fill(dt)
                    If dt.Rows.Count > 0 Then
                        Dim fields = "`printxzread` = '" & PrintXZReadOption & "' "
                        sql = "UPDATE " & table & " SET " & fields & " WHERE `settings_id` = 1"
                        cmd = New MySqlCommand(sql, ConnectionLocal)
                        cmd.ExecuteNonQuery()
                    Else
                        Dim fields = "`printxzread`"
                        Dim value = "'" & PrintXZReadOption & "'"
                        sql = "INSERT INTO " & table & " (" & fields & ") VALUES (" & value & ")"
                        cmd = New MySqlCommand(sql, ConnectionLocal)
                        cmd.ExecuteNonQuery()
                    End If
                    ConnectionLocal.Close()
                Else
                    MsgBox("Select option first")
                    PrintXZRead = False
                    PrintXZReadOption = ""
                End If
            Else
                MsgBox("Connection must be valid first")
                PrintXZReadOption = ""
                PrintXZRead = False
            End If

        Catch ex As Exception

            PrintXZReadOption = ""
            PrintXZRead = False
        End Try
    End Sub
    Property PrintReturns = ""
    Property PrintReturnsBool As Boolean = False
    Private Sub RadioButtonPrintReturnsYes_Click(sender As Object, e As EventArgs) Handles RadioButtonPrintReturnsYes.Click, RadioButtonPrintReturnsNo.Click
        Try
            Dim table = "`loc_settings`"
            If ValidLocalConnection Then
                If RadioButtonPrintReturnsYes.Checked Then
                    PrintReturns = "YES"
                    PrintReturnsBool = True
                ElseIf RadioButtonPrintReturnsNo.Checked Then
                    PrintReturns = "NO"
                    PrintReturnsBool = True
                Else
                    PrintReturns = ""
                    PrintReturnsBool = False
                End If
                If PrintReturnsBool Then
                    Dim ConnectionLocal As MySqlConnection = TestLocalConnection()
                    Dim sql = "Select `printreturns` FROM " & table & " WHERE `settings_id` = 1"
                    Dim cmd As MySqlCommand = New MySqlCommand(sql, ConnectionLocal)
                    Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
                    Dim dt As DataTable = New DataTable
                    da.Fill(dt)
                    If dt.Rows.Count > 0 Then
                        Dim fields = "`printreturns` = '" & PrintReturns & "' "
                        sql = "UPDATE " & table & " SET " & fields & " WHERE `settings_id` = 1"
                        cmd = New MySqlCommand(sql, ConnectionLocal)
                        cmd.ExecuteNonQuery()
                    Else
                        Dim fields = "`printreturns`"
                        Dim value = "'" & PrintReturns & "'"
                        sql = "INSERT INTO " & table & " (" & fields & ") VALUES (" & value & ")"
                        cmd = New MySqlCommand(sql, ConnectionLocal)
                        cmd.ExecuteNonQuery()
                    End If
                    ConnectionLocal.Close()
                Else
                    MsgBox("Select option first")
                    PrintReturnsBool = False
                    PrintReturns = ""
                End If
            Else
                MsgBox("Connection must be valid first")
                PrintReturns = ""
                PrintReturnsBool = False
            End If

        Catch ex As Exception

            PrintReturns = ""
            PrintReturnsBool = False
        End Try
    End Sub
    Property PrintSalesReport As String = ""
    Property PrintSalesReportBool As Boolean = False
    Private Sub RadioButtonPrintSRYes_Click(sender As Object, e As EventArgs) Handles RadioButtonPrintSRYes.Click, RadioButtonPrintSRNo.Click
        Try
            Dim table = "`loc_settings`"
            If ValidLocalConnection Then
                If RadioButtonPrintSRYes.Checked Then
                    PrintSalesReport = "YES"
                    PrintSalesReportBool = True
                ElseIf RadioButtonPrintSRNo.Checked Then
                    PrintSalesReport = "NO"
                    PrintSalesReportBool = True
                Else
                    PrintSalesReport = ""
                    PrintSalesReportBool = False
                End If
                If PrintSalesReportBool Then
                    Dim ConnectionLocal As MySqlConnection = TestLocalConnection()
                    Dim sql = "Select `printsalesreport` FROM " & table & " WHERE `settings_id` = 1"
                    Dim cmd As MySqlCommand = New MySqlCommand(sql, ConnectionLocal)
                    Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
                    Dim dt As DataTable = New DataTable
                    da.Fill(dt)
                    If dt.Rows.Count > 0 Then
                        Dim fields = "`printsalesreport` = '" & PrintSalesReport & "' "
                        sql = "UPDATE " & table & " SET " & fields & " WHERE `settings_id` = 1"
                        cmd = New MySqlCommand(sql, ConnectionLocal)
                        cmd.ExecuteNonQuery()
                    Else
                        Dim fields = "`printsalesreport`"
                        Dim value = "'" & PrintSalesReport & "'"
                        sql = "INSERT INTO " & table & " (" & fields & ") VALUES (" & value & ")"
                        cmd = New MySqlCommand(sql, ConnectionLocal)
                        cmd.ExecuteNonQuery()
                    End If
                    ConnectionLocal.Close()
                Else
                    MsgBox("Select option first")
                    PrintSalesReportBool = False
                    PrintSalesReport = ""
                End If
            Else
                MsgBox("Connection must be valid first")
                PrintSalesReport = ""
                PrintSalesReportBool = False
            End If

        Catch ex As Exception

            PrintSalesReport = ""
            PrintSalesReportBool = False
        End Try
    End Sub
#End Region

    Private Sub RadioButtonTestModeFalse_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButtonTestModeFalse.CheckedChanged
        If RadioButtonTestModeFalse.Checked Then
            TestModeIsOFF = True
        Else
            TestModeIsOFF = False
        End If
    End Sub

    Private Sub rtbLogStats_TextChanged(sender As Object, e As EventArgs) Handles rtbLogStats.TextChanged
        rtbLogStats.SelectionStart = rtbLogStats.Text.Length
        rtbLogStats.ScrollToCaret()
    End Sub

#Region "Test Insert"
    'Private Sub button734_click(sender As Object, e As EventArgs) Handles Button4.Click
    '    InsertToProducts()
    '    InsertToInventory()
    '    InsertToCategories()
    '    InsertToFormula()
    'End Sub

    'Private Sub button8_click_123(sender As Object, e As EventArgs) Handles Button8.Click
    '    GetCategories()
    '    GetProducts()
    '    GetInventory()
    '    GetFormula()
    'End Sub


#End Region
End Class