Imports MySql.Data.MySqlClient
Imports System.Threading
Public Class Auth
    Property threadList As List(Of Thread) = New List(Of Thread)
    Property thread As Thread
    Property Account As String
    Property TimerCount As Integer = 0
    Property UserList As New List(Of AuthCls)
    Private Sub Auth_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            CheckForIllegalCrossThreadCalls = False
            BackgroundWorker1.WorkerReportsProgress = True
            BackgroundWorker1.WorkerSupportsCancellation = True
            BackgroundWorker1.RunWorkerAsync()
            ChangeProgBarColor(ProgressBar1, ProgressBarColor.Yellow)
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "Auth/Load: " & ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Try
            ProgressBar1.Maximum = 100
            ProgressBar1.Value = 0
            For i = 0 To 100
                BackgroundWorker1.ReportProgress(i)
                Thread.Sleep(50)
                If i = 0 Then
                    thread = New Thread(AddressOf SyncToLocalUsers)
                    thread.Start()
                    threadList.Add(thread)
                End If
                If i = 50 Then
                    Label4.Text = "Checking user(s) availability...  "
                End If
            Next
            For Each t In threadList
                t.Join()
            Next
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "Auth/Get user accounts: " & ex.ToString, "Critical")
        End Try
    End Sub
    Private Function IfUserExist(uniqID) As Boolean
        Dim ReturnBool As Boolean = False
        Try
            Dim ConnectionLocal As MySqlConnection = LocalhostConn()
            Dim sql = "SELECT uniq_id FROM loc_users WHERE uniq_id = '" & uniqID & "'"
            Dim cmd As MySqlCommand = New MySqlCommand(sql, ConnectionLocal)
            Using reader As MySqlDataReader = cmd.ExecuteReader
                If reader.HasRows Then
                    ReturnBool = True
                Else
                    ReturnBool = False
                End If
            End Using
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "Auth/IfUserExist(): " & ex.ToString, "Critical")
        End Try
        Return ReturnBool
    End Function
    Private Sub SyncToLocalUsers()
        Try
            Dim ConnectionServer As MySqlConnection = ServerCloudCon()
            Using mCmd = New MySqlCommand("", ConnectionServer)
                With mCmd
                    .Parameters.Clear()
                    .CommandText = "SELECT `user_level`, `full_name`, `username`, `password`, `contact_number`, `email`, `position`, `gender`, `uniq_id` , `created_at`, `updated_at` , `pwd`, `user_code` FROM `loc_users` WHERE store_id IN ('" & ClientStoreID & "','0') AND synced IN ('Unsynced','N/A') AND active = 1"
                    .Prepare()
                    Using reader = .ExecuteReader
                        While reader.Read
                            If Not IfUserExist(reader("uniq_id")) Then
                                Dim AuthClass As New AuthCls
                                AuthClass.Userlevel = reader("user_level")
                                AuthClass.FullName = reader("full_name")
                                AuthClass.UserName = reader("username")
                                AuthClass.Password = reader("password")
                                AuthClass.Contact = reader("contact_number")
                                AuthClass.EmailAddress = reader("email")
                                AuthClass.Position = reader("position")
                                AuthClass.Gender = reader("gender")
                                AuthClass.UniqueID = reader("uniq_id")
                                AuthClass.DateCreated = reader("created_at")
                                AuthClass.DateModified = reader("updated_at")
                                AuthClass.PwdUnencrypted = reader("pwd")
                                AuthClass.UserCode = reader("user_code")
                                UserList.Add(AuthClass)
                                Account += $"Username: {AuthClass.UserName}{vbNewLine}Password: **********{vbNewLine}"
                            End If
                        End While
                        .Dispose()
                    End Using
                    .Dispose()
                End With
            End Using
            ConnectionServer.Close()
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "Auth/SyncToLocalUsers(): " & ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub BackgroundWorker1_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        ProgressBar1.Value = e.ProgressPercentage
        Label2.Text = e.ProgressPercentage
    End Sub
    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted

        Dim hasNewUser As Integer = UserList.Count

        Try
            If hasNewUser > 0 Then
                Dim ConnectionLocal As MySqlConnection = LocalhostConn()
                Dim ConnectionServer As MySqlConnection = ServerCloudCon()
                For Each row As AuthCls In UserList
                    Using mCmd = New MySqlCommand("", ConnectionLocal)
                        With mCmd
                            .Parameters.Clear()
                            .CommandText = "INSERT INTO triggers_loc_users
                                            (
                                                `user_level`, `full_name`, `username`, `password`, `contact_number`, `email`, 
                                                `position`, `gender`, `active`, `store_id`, `uniq_id`, `synced`, 
                                                `user_code`
                                            ) VALUES (
                                                @user_level, @full_name, @username, @password, @contact_number, @email,
                                                @position, @gender, @active, @store_id, @uniq_id, @synced, 
                                                @user_code
                                            )"
                            .Parameters.AddWithValue("@user_level", row.Userlevel)
                            .Parameters.AddWithValue("@full_name", row.FullName)
                            .Parameters.AddWithValue("@username", row.UserName)
                            .Parameters.AddWithValue("@password", row.Password)
                            .Parameters.AddWithValue("@contact_number", row.Contact)
                            .Parameters.AddWithValue("@email", row.EmailAddress)
                            .Parameters.AddWithValue("@position", row.Position)
                            .Parameters.AddWithValue("@gender", row.Gender)
                            .Parameters.AddWithValue("@active", 1)
                            .Parameters.AddWithValue("@store_id", ClientStoreID)
                            .Parameters.AddWithValue("@uniq_id", row.UniqueID)
                            .Parameters.AddWithValue("@synced", "Y")
                            .Parameters.AddWithValue("@user_code", row.UserCode)
                            .ExecuteNonQuery()

                            If row.Position <> "Admin" Then
                                .Parameters.Clear()
                                .Connection = ConnectionServer
                                .CommandText = $"UPDATE loc_users SET synced = 'Synced' WHERE uniq_id = '{row.UniqueID}'"
                                .ExecuteNonQuery()
                            End If
                            .Dispose()
                        End With
                    End Using
                Next

                ConnectionLocal.Close()
                ConnectionServer.Close()

                AuditTrail.LogToAuditTrail("System", "Auth: Update successful, " & Account, "Normal")
                Label4.Text = "New user(s) available.  "
                Dim msg = "New user account has been added" & vbNewLine & Account
                Dim msgs = MsgBox(msg, MsgBoxStyle.OkOnly)
                If msgs = DialogResult.OK Then
                    Timer1.Start()
                End If
            Else
                Label4.Text = "No new user(s) available.  "
                Timer1.Start()
            End If
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub Auth_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Login.Enabled = True
    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Try
            TimerCount = TimerCount + 1
            If TimerCount = 1 Then
                Label1.Text = "Closing..."
            ElseIf TimerCount = 2 Then
                Close()
            End If
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", ex.ToString, "Critical")
        End Try
    End Sub
End Class