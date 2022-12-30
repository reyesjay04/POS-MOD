Imports MySql.Data.MySqlClient

Public Class AuditTrailFilter
    Private Sub AuditTrailFilter_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        AuditTrail.Enabled = True
    End Sub
    Property Group As String
    Property Severity As String
    Property UserID As String
    Property Limit As String
    Property FromDate As String
    Property ToDate As String
    Public Sub New(ByVal flGroup As String, ByVal flSeverity As String, ByVal fluserid As String, ByVal flLimit As String, ByVal flfromdate As String, ByVal fltodate As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Group = flGroup
        Severity = flSeverity
        UserID = fluserid
        Limit = flLimit
        FromDate = flfromdate
        ToDate = fltodate
    End Sub
    Private Sub AuditTrailFilter_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadGroups()
        LoadUserNames()
        LoadSeverities()
        cmbLimit.SelectedItem = Limit

        Try
            DateTimePicker1.Value = Date.ParseExact(FromDate, "yyyy-MM-dd", Globalization.DateTimeFormatInfo.InvariantInfo)
            DateTimePicker2.Value = Date.ParseExact(ToDate, "yyyy-MM-dd", Globalization.DateTimeFormatInfo.InvariantInfo)
        Catch ex As Exception
        End Try
    End Sub
    Private Sub LoadGroups()
        Try
            Dim ConnectionLocal As MySqlConnection = LocalhostConn()
            Dim Query As String = "Select DISTINCT group_name FROM loc_audit_trail ORDER BY crew_id ASC"
            Dim Command As MySqlCommand = New MySqlCommand(Query, ConnectionLocal)
            Using reader As MySqlDataReader = Command.ExecuteReader
                If reader.HasRows Then
                    cmbGroupName.Items.Add("All")
                    While reader.Read
                        cmbGroupName.Items.Add(reader("group_name"))
                    End While
                    cmbGroupName.SelectedItem = Group
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
                    cmbUser.Items.Add("All")
                    While reader.Read
                        cmbUser.Items.Add(reader("crew_id"))
                    End While
                    cmbUser.SelectedItem = UserID
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
                    cmbSeverities.Items.Add("All")
                    While reader.Read
                        cmbSeverities.Items.Add(reader("severity"))
                    End While
                    cmbSeverities.SelectedItem = Severity
                End If
            End Using
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "AuditFilter/LoadSeverities(): " & ex.ToString, "Critical")
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        With AuditTrail
            .ATGroupName = cmbGroupName.Text
            .ATUserName = cmbUser.Text
            .ATFromDate = Format(DateTimePicker1.Value, "yyyy-MM-dd")
            .ATToDate = Format(DateTimePicker2.Value, "yyyy-MM-dd")
            .ATRowLimit = cmbLimit.Text
            .ATSeverity = cmbSeverities.Text
            .FromFilter = True
            .LoadLogs(False)
        End With
        Close()
    End Sub
End Class