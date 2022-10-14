Imports MySql.Data.MySqlClient

Public Class AuditTrailFilter
    Private Sub AuditTrailFilter_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        AuditTrail.Enabled = True
    End Sub

    Private Sub AuditTrailFilter_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadGroups()
        LoadUserNames()
        LoadSeverities()
        ComboBox3.SelectedIndex = 0
    End Sub
    Private Sub LoadGroups()
        Try
            Dim ConnectionLocal As MySqlConnection = LocalhostConn()
            Dim Query As String = "Select DISTINCT group_name FROM loc_audit_trail ORDER BY crew_id ASC"
            Dim Command As MySqlCommand = New MySqlCommand(Query, ConnectionLocal)
            Using reader As MySqlDataReader = Command.ExecuteReader
                If reader.HasRows Then
                    ComboBox1.Items.Add("All")
                    While reader.Read
                        ComboBox1.Items.Add(reader("group_name"))
                    End While
                    ComboBox1.SelectedIndex = 0
                End If
            End Using
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "AuditFilter/LoadGroups(): " & ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub LoadUserNames()
        Try
            Dim ConnectionLocal As MySqlConnection = LocalhostConn()
            Dim Query As String = "Select DISTINCT crew_id FROM loc_audit_trail ORDER BY crew_id ASC"
            Dim Command As MySqlCommand = New MySqlCommand(Query, ConnectionLocal)
            Using reader As MySqlDataReader = Command.ExecuteReader
                If reader.HasRows Then
                    ComboBox2.Items.Add("All")
                    While reader.Read
                        ComboBox2.Items.Add(reader("crew_id"))
                    End While
                    ComboBox2.SelectedIndex = 0
                End If
            End Using
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "AuditFilter/LoadUserNames(): " & ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub LoadSeverities()
        Try
            Dim ConnectionLocal As MySqlConnection = LocalhostConn()
            Dim Query As String = "Select DISTINCT severity FROM loc_audit_trail ORDER BY crew_id ASC"
            Dim Command As MySqlCommand = New MySqlCommand(Query, ConnectionLocal)
            Using reader As MySqlDataReader = Command.ExecuteReader
                If reader.HasRows Then
                    ComboBoxSeverity.Items.Add("All")
                    While reader.Read
                        ComboBoxSeverity.Items.Add(reader("severity"))
                    End While
                    ComboBoxSeverity.SelectedIndex = 0
                End If
            End Using
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "AuditFilter/LoadSeverities(): " & ex.ToString, "Critical")
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        With AuditTrail
            .ATGroupName = ComboBox1.Text
            .ATUserName = ComboBox2.Text
            .ATFromDate = Format(DateTimePicker1.Value, "yyyy-MM-dd")
            .ATToDate = Format(DateTimePicker2.Value, "yyyy-MM-dd")
            .ATRowLimit = ComboBox3.Text
            .ATSeverity = ComboBoxSeverity.Text
            .LoadLogs(False)
        End With
        Close()
    End Sub
End Class