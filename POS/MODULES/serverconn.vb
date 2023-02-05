Imports MySql.Data.MySqlClient
Module serverlocalconn
    Public Sub LoadCloudConnString()
        Try
            Dim ConnectionLocal = LocalhostConn()
            Dim sql = "SELECT `C_Server`, `C_Username`, `C_Password`, `C_Database`, `C_Port` FROM loc_settings WHERE settings_id = 1"
            Dim cmd As MySqlCommand = New MySqlCommand(sql, ConnectionLocal)

            Using reader = cmd.ExecuteReader
                If reader.HasRows Then
                    While reader.Read
                        CloudConnectionString = $"server={ConvertB64ToString(reader("C_Server").ToString)};userid={ConvertB64ToString(reader("C_Username").ToString)};password={ConvertB64ToString(reader("C_Password").ToString)};port={ConvertB64ToString(reader("C_Port").ToString)};database={ConvertB64ToString(reader("C_Database").ToString)};"
                    End While
                End If
                reader.Close()
            End Using

        Catch ex As Exception
        End Try
    End Sub
    Public Function ServerCloudCon() As MySqlConnection
        Dim servercloudconn As New MySqlConnection
        Try
            servercloudconn.ConnectionString = CloudConnectionString
            servercloudconn.Open()
            If servercloudconn.State = ConnectionState.Open Then
                ValidCloudConnection = True
            End If
        Catch ex As Exception
            ValidCloudConnection = False
        End Try
        Return servercloudconn
    End Function
End Module
