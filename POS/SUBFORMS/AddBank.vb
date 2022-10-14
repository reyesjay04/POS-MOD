﻿Public Class AddBank
    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        Try
            If Not String.IsNullOrEmpty(TextBoxBankName.Text) Then
                table = "loc_partners_transaction"
                If SettingsForm.AddOrEdit = True Then
                    Dim arrid = count("arrid", table)
                    fields = "(`arrid`, `bankname`,  `active`, `crew_id`, `synced`, `store_id`, `guid`)"
                    value = "(" & arrid + 1 & ", '" & Trim(TextBoxBankName.Text) & "', " & 1 & ", '" & ClientCrewID & "', 'N', '" & ClientStoreID & "', '" & ClientGuid & "')"
                    GLOBAL_INSERT_FUNCTION(table, fields, value)
                    SettingsForm.LoadPartners()
                Else
                    fields = "bankname = '" & Trim(TextBoxBankName.Text) & "'"
                    where = "id = " & SettingsForm.DataGridViewPartners.SelectedRows(0).Cells(0).Value & ""
                    GLOBAL_FUNCTION_UPDATE(table, fields, where)
                    SettingsForm.LoadPartners()
                End If
            End If
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "AddBank/Button11: " & ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub AddBank_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        SettingsForm.Enabled = True
    End Sub
    Private Sub TextBoxBankName_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBoxBankName.KeyPress
        Try
            If InStr(DisallowedCharacters, e.KeyChar) > 0 Then
                e.Handled = True
            End If
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "AddBank/TextBoxBankName: " & ex.ToString, "Critical")
        End Try
    End Sub
End Class