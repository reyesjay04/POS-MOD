Imports System.IO
Imports System.Text
Imports MySql.Data.MySqlClient
Imports PdfSharp.Drawing
Imports PdfSharp.Pdf

Public Class AuditTrail
    Property ATGroupName As String = ""
    Property ATUserName As String = ""
    Property ATFromDate As String = ""
    Property ATToDate As String = ""
    Property ATRowLimit As String = ""
    Property ATSeverity As String = ""
    Property FromFilter As Boolean = False

    Private Sub AuditTrail_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        tscRowLimit.SelectedIndex = 0
        LoadLogs(True)
    End Sub

    Public Sub LoadLogs(LoadOnly As Boolean)
        Try
            If FromFilter Then
                tscRowLimit.SelectedItem = ATRowLimit
            End If
            DataGridViewAuditTrail.Rows.Clear()
            Dim ConnectionLocal As MySqlConnection = LocalhostConn()
            Dim Query As String = ""
            If LoadOnly Then
                LabelDate.Text = $"Filter Date From - To: {Format(Now(), "yyyy-MM-dd")} | {Format(Now(), "yyyy-MM-dd")}"
                Query = $"SELECT * FROM loc_audit_trail WHERE DATE_FORMAT(created_at, '%Y-%m-%d') = '{Format(Now(), "yyyy-MM-dd")}'"
            Else
                LabelDate.Text = $"Filter Date From - To: {ATFromDate} | {ATToDate}"
                Query = $"SELECT * FROM loc_audit_trail WHERE DATE(created_at) BETWEEN '{ATFromDate}' AND '{ATToDate}'"
                If ATGroupName <> "All" Then
                    Query &= $"AND group_name = '{ATGroupName}'"
                End If
                If ATUserName <> "All" Then
                    Query &= $"AND crew_id = '{ATUserName}'"
                End If
                If ATSeverity <> "All" Then
                    Query &= $"AND severity = '{ATSeverity}'"
                End If
            End If

            Query &= "ORDER BY created_at DESC"
            If ATRowLimit <> "All" Then
                Query &= $" LIMIT  {ATRowLimit}"
            End If

            Dim Command As MySqlCommand = New MySqlCommand(Query, ConnectionLocal)
            Dim Da As MySqlDataAdapter = New MySqlDataAdapter(Command)
            Dim Dt As DataTable = New DataTable
            Da.Fill(Dt)

            For i As Integer = 0 To Dt.Rows.Count - 1 Step +1
                DataGridViewAuditTrail.Rows.Add(Dt(i)(0), Dt(i)(1), Dt(i)(2), Dt(i)(3), Dt(i)(4), Dt(i)(5), Dt(i)(6), Dt(i)(7))
                If Dt(i)(3) = "Normal" Then
                    DataGridViewAuditTrail.Rows(i).Cells(3).Style.BackColor = Color.LightGreen
                Else
                    DataGridViewAuditTrail.Rows(i).Cells(3).Style.BackColor = Color.OrangeRed
                End If
            Next
            FromFilter = False
            tssrowcount.Text = $"Row Count: {DataGridViewAuditTrail.Rows.Count}"
        Catch ex As Exception
            LogToAuditTrail("System", $"Audit: {ex.ToString}", "Critical")
        End Try
    End Sub

    Public Sub LogToAuditTrail(GroupName As String, Description As String, Severity As String)
        Try
            If Severity = "Critical" Then
                MessageBox.Show("An error occured. Please contact the System Administrator", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
            If ValidLocalConnection Then

                Dim crewID = If(ClientCrewID <> "", ClientCrewID, "N/A")
                Dim storeId = If(ClientStoreID <> "", ClientStoreID, 0)

                Dim ConnectionLocal As MySqlConnection = LocalhostConn()
                Dim Query As String = "INSERT INTO loc_audit_trail (`created_at`, `group_name`, `severity`, `crew_id`, `description`,`info`, `store_id`, `synced`) VALUES (@1, @2, @3, @4, @5, @6, @7, @8)"
                Dim Command As MySqlCommand = New MySqlCommand(Query, ConnectionLocal)
                Command.Parameters.Clear()
                Command.Parameters.AddWithValue("@1", FullDate24HR())
                Command.Parameters.AddWithValue("@2", GroupName)
                Command.Parameters.AddWithValue("@3", Severity)
                Command.Parameters.AddWithValue("@4", crewID)
                Command.Parameters.AddWithValue("@5", Description)
                Command.Parameters.AddWithValue("@6", "DG POS, " & My.Settings.Version & "")
                Command.Parameters.AddWithValue("@7", storeId)
                Command.Parameters.AddWithValue("@8", "N")
                Command.ExecuteNonQuery()
            End If

        Catch ex As Exception
            MessageBox.Show("Please contact the System Administrator", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        Enabled = False
        Dim filter As New AuditTrailFilter(ATGroupName, ATSeverity, ATUserName, ATRowLimit, ATFromDate, ATToDate)
        filter.ShowDialog()
    End Sub

    Private Sub AuditTrail_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        MDIFORM.newMDIchildReports.Enabled = True
        MDIFORM.Enabled = True
    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Try
            Dim document As PdfDocument = New PdfDocument
            document.Info.Title = "PDF"
            Dim page As PdfPage = document.Pages.Add
            Dim gfx As XGraphics = XGraphics.FromPdfPage(page)
            Dim font As XFont = New XFont("Verdana", 5, XFontStyle.Regular)

            If DataGridViewAuditTrail.Rows.Count > 0 Then
                ' Create a new PDF document
                Dim NextPage As Integer = DataGridViewAuditTrail.Rows.Count
                Dim PageRows As Integer = 50
                Dim TotalRowsPerPage As Integer = NextPage / PageRows

                If NextPage <= 50 Then
                    TotalRowsPerPage = 1
                Else
                    TotalRowsPerPage += 1
                End If
                Dim Kahitano As Integer = 1
                Dim GetDgvRowCount As Integer = 0

                If ATFromDate = "" And ATToDate = "" Then
                    ATFromDate = Format(Now(), "yyyy-MM-dd")
                    ATToDate = Format(Now(), "yyyy-MM-dd")
                End If

                For a = 1 To TotalRowsPerPage
                    If a <> Kahitano Then
                        page = document.AddPage
                        gfx = XGraphics.FromPdfPage(page)
                        gfx.DrawString("Date From - To: " & ATFromDate & " | " & ATToDate, font, XBrushes.Black, 50, 50)
                        gfx.DrawString("AUDIT TRAIL REPORT", font, XBrushes.Black, 50, 60)
                        gfx.DrawString("ID", font, XBrushes.Black, 50, 80)
                        gfx.DrawString("Time", font, XBrushes.Black, 85, 80)
                        gfx.DrawString("Group", font, XBrushes.Black, 150, 80)
                        gfx.DrawString("Severity", font, XBrushes.Black, 190, 80)
                        gfx.DrawString("Username", font, XBrushes.Black, 230, 80)
                        gfx.DrawString("Description", font, XBrushes.Black, 270, 80)
                        gfx.DrawString("Info", font, XBrushes.Black, 490, 80)
                        Dim RowCount As Integer = 10
                        Dim CountPage As Integer = 0
                        With DataGridViewAuditTrail

                            For i As Integer = GetDgvRowCount To .Rows.Count - 1 Step +1
                                If CountPage < PageRows Then
                                    Dim descLimited = If(.Rows(i).Cells(5).Value.ToString.Length > 70, .Rows(i).Cells(5).Value.ToString.Substring(0, 69) & "...", .Rows(i).Cells(5).Value.ToString)
                                    gfx.DrawString(.Rows(i).Cells(0).Value, font, XBrushes.Black, 50, 80 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(1).Value, font, XBrushes.Black, 85, 80 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(2).Value, font, XBrushes.Black, 150, 80 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(3).Value, font, XBrushes.Black, 190, 80 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(4).Value, font, XBrushes.Black, 230, 80 + RowCount)
                                    gfx.DrawString(descLimited, font, XBrushes.Black, 270, 80 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(6).Value, font, XBrushes.Black, 490, 80 + RowCount)
                                    RowCount += 10
                                    CountPage += 1
                                    GetDgvRowCount += 1
                                Else
                                    Exit For
                                End If
                            Next
                        End With
                        Kahitano += 1
                    Else
                        gfx.DrawString("Date From - To: " & ATFromDate & " | " & ATToDate, font, XBrushes.Black, 50, 50)
                        gfx.DrawString("AUDIT TRAIL REPORT", font, XBrushes.Black, 50, 60)
                        gfx.DrawString("ID", font, XBrushes.Black, 50, 80)
                        gfx.DrawString("Time", font, XBrushes.Black, 85, 80)
                        gfx.DrawString("Group", font, XBrushes.Black, 150, 80)
                        gfx.DrawString("Severity", font, XBrushes.Black, 190, 80)
                        gfx.DrawString("Username", font, XBrushes.Black, 230, 80)
                        gfx.DrawString("Description", font, XBrushes.Black, 270, 80)
                        gfx.DrawString("Info", font, XBrushes.Black, 490, 80)

                        Dim RowCount As Integer = 10
                        Dim CountPage As Integer = 0
                        With DataGridViewAuditTrail

                            For i As Integer = 0 To .Rows.Count - 1 Step +1

                                If i < PageRows Then
                                    Dim descLimited = If(.Rows(i).Cells(5).Value.ToString.Length > 70, .Rows(i).Cells(5).Value.ToString.Substring(0, 69) & "...", .Rows(i).Cells(5).Value.ToString)

                                    gfx.DrawString(.Rows(i).Cells(0).Value, font, XBrushes.Black, 50, 80 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(1).Value, font, XBrushes.Black, 85, 80 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(2).Value, font, XBrushes.Black, 150, 80 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(3).Value, font, XBrushes.Black, 190, 80 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(4).Value, font, XBrushes.Black, 230, 80 + RowCount)
                                    gfx.DrawString(descLimited, font, XBrushes.Black, 270, 80 + RowCount)
                                    gfx.DrawString(.Rows(i).Cells(6).Value, font, XBrushes.Black, 490, 80 + RowCount)
                                    RowCount += 10
                                    CountPage += 1
                                    GetDgvRowCount += 1
                                Else
                                    Exit For
                                End If
                            Next
                        End With


                    End If
                Next

                LogToAuditTrail("Report", "Reports/Custom Report: Generated Report, " & ClientCrewID, "Normal")
                Dim filename = My.Computer.FileSystem.SpecialDirectories.Desktop & "\Audit-Trail" & FullDateFormatForSaving() & ".pdf"
                document.Save(filename)

                ' ...and start a viewer.
                Process.Start(filename)
            End If
        Catch ex As Exception
            LogToAuditTrail("System", "Audit: " & ex.ToString, "Critical")
        End Try

    End Sub

    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        GenerateEJournalV2()
    End Sub
    Private Sub GenerateEJournalV2()
        Try

            Dim CompleteDirectoryPath As String = ""

            If Not Directory.Exists(My.Computer.FileSystem.SpecialDirectories.Desktop & "\AuditTrail") Then
                Directory.CreateDirectory(My.Computer.FileSystem.SpecialDirectories.Desktop & "\AuditTrail")
                CompleteDirectoryPath = My.Computer.FileSystem.SpecialDirectories.Desktop & "\AuditTrail\" & FullDateFormatForSaving()
                Directory.CreateDirectory(CompleteDirectoryPath)
            Else
                CompleteDirectoryPath = My.Computer.FileSystem.SpecialDirectories.Desktop & "\AuditTrail\" & FullDateFormatForSaving()
                Directory.CreateDirectory(CompleteDirectoryPath)
            End If

            Dim GrandTotalLines As Integer = DataGridViewAuditTrail.Rows.Count + 1
            Dim TotalDgvRows As Integer = GrandTotalLines
            Dim TxtFileLine(TotalDgvRows) As String
            Dim a As Integer = 0

            TxtFileLine(a) = "ID      Time      Group      Severity     Username     Description     Info"
            a += 1

            With DataGridViewAuditTrail
                For i As Integer = 0 To .Rows.Count - 1 Step +1
                    Dim ID As String = .Rows(i).Cells(0).Value
                    Dim Time As String = .Rows(i).Cells(1).Value
                    Dim Group As String = .Rows(i).Cells(2).Value
                    Dim Severity As String = .Rows(i).Cells(3).Value
                    Dim UserName As String = .Rows(i).Cells(4).Value
                    Dim Description As String = .Rows(i).Cells(5).Value
                    Dim Info As String = .Rows(i).Cells(6).Value
                    TxtFileLine(a) = ID & "   " & Time & "   " & Group & "   " & Severity & "   " & UserName & "   " & Description & ";   " & Info
                    a += 1
                Next
            End With


            Dim CompletePath As String = CompleteDirectoryPath & "\AuditTrail" & FullDateFormatForSaving() & ".txt"
            LogToAuditTrail("Report", "Audit Trail: Txt file generated, " & CompleteDirectoryPath, "Normal")
            File.WriteAllLines(CompletePath, TxtFileLine, Encoding.UTF8)
            MsgBox("Complete", MsgBoxStyle.Information)
        Catch ex As Exception
            LogToAuditTrail("System", "Audit: " & ex.ToString, "Critical")
        End Try
    End Sub

    Private Sub ToolStripComboBox1_Click(sender As Object, e As EventArgs) Handles tscRowLimit.Click

    End Sub

    Private Sub ToolStripComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tscRowLimit.SelectedIndexChanged
        ATGroupName = If(ATGroupName = "", "All", ATGroupName)
        ATUserName = If(ATUserName = "", "All", ATUserName)
        ATSeverity = If(ATSeverity = "", "All", ATSeverity)
        ATFromDate = If(ATFromDate = "", Format(Now(), "yyyy-MM-dd"), ATFromDate)
        ATToDate = If(ATToDate = "", Format(Now(), "yyyy-MM-dd"), ATToDate)
        ATRowLimit = If(FromFilter, ATRowLimit, If(ATRowLimit = "", tscRowLimit.Text, If(ATRowLimit <> tscRowLimit.Text, tscRowLimit.Text, ATRowLimit)))

        Try
            LoadLogs(False)
        Catch ex As Exception
            LogToAuditTrail("System", "Audit: " & ex.ToString, "Critical")
        End Try
    End Sub

    Private Sub DataGridViewAuditTrail_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridViewAuditTrail.CellDoubleClick
        Dim showFrm As ViewLogs = New ViewLogs(DataGridViewAuditTrail.SelectedRows(0).Cells(5).Value.ToString)
        showFrm.ShowDialog()
    End Sub
End Class