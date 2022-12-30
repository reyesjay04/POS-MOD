Imports MySql.Data.MySqlClient
Module Updatemodule
    Public Sub GLOBAL_FUNCTION_UPDATE(ByVal table, ByVal fields, ByVal where)
        Dim ConnectionLocal As MySqlConnection = LocalhostConn()
        Try
            Dim sql = "UPDATE " + table + " SET " + fields + " WHERE " & where
            Dim cmd As MySqlCommand = New MySqlCommand(sql, ConnectionLocal)
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "ModUpdate/GLOBAL_FUNCTION_UPDATE(): " & ex.ToString, "Critical")
        Finally
            ConnectionLocal.Close()
            cmd.Dispose()
        End Try
    End Sub

    Public Sub UpdateInventory(returnVoid As Boolean)

        Dim secondary_value As Double = 0
        Dim stock_secondary As Double = 0
        Dim SqlDt As New DataTable
        Dim UpdateInventoryCon As MySqlConnection = LocalhostConn()

        Try
            Dim Query As String = ""
            For i As Integer = 0 To INVENTORY_DATATABLE.Rows.Count - 1 Step +1
                Dim TotalQuantity As Double = 0
                Dim TotalServingValue As Double = 0
                Dim Secondary As Double = 0
                Dim ServingValue As Double = 0
                Dim TotalPrimary As Double = 0
                TotalQuantity = INVENTORY_DATATABLE(i)(2)
                TotalServingValue = Double.Parse(INVENTORY_DATATABLE(i)(0).ToString)

                Dim querySecVal As Double = 0
                If INVENTORY_DATATABLE(i)(9).ToString = "Server" Then
                    Query = "SELECT `secondary_value` FROM `loc_product_formula` WHERE server_formula_id = " & INVENTORY_DATATABLE(i)(1)
                Else
                    Query = "SELECT `secondary_value` FROM `loc_product_formula` WHERE formula_id = " & INVENTORY_DATATABLE(i)(1)
                End If

                Using mCmd = New MySqlCommand(Query, UpdateInventoryCon)
                    Using reader = mCmd.ExecuteReader
                        While reader.Read
                            secondary_value = reader("secondary_value")
                        End While
                        reader.Dispose()
                    End Using
                    mCmd.Dispose()
                End Using

                If INVENTORY_DATATABLE(i)(9).ToString = "Server" Then
                    Query = "Select `stock_secondary` FROM `loc_pos_inventory` WHERE server_inventory_id = " & INVENTORY_DATATABLE(i)(1)
                Else
                    Query = "Select `stock_secondary` FROM `loc_pos_inventory` WHERE inventory_id = " & INVENTORY_DATATABLE(i)(1)
                End If

                Using mCmd = New MySqlCommand(Query, UpdateInventoryCon)
                    Using reader = mCmd.ExecuteReader
                        While reader.Read
                            stock_secondary = reader("stock_secondary")
                        End While
                        reader.Dispose()
                    End Using
                    mCmd.Dispose()
                End Using

                If returnVoid Then
                    Secondary = stock_secondary + TotalServingValue
                Else
                    Secondary = stock_secondary - TotalServingValue
                End If

                ServingValue = Secondary / Double.Parse(INVENTORY_DATATABLE(i)(5).ToString)
                TotalPrimary = Secondary / secondary_value

                If INVENTORY_DATATABLE(i)(9).ToString = "Server" Then
                    Query = "UPDATE loc_pos_inventory Set stock_secondary = " & Secondary & " , stock_no_of_servings = " & ServingValue & ", stock_primary = " & TotalPrimary & ", date_modified = '" & FullDate24HR() & "' WHERE server_inventory_id = " & INVENTORY_DATATABLE(i)(1)
                Else
                    Query = "UPDATE loc_pos_inventory SET stock_secondary = " & Secondary & " , stock_no_of_servings = " & ServingValue & " , stock_primary = " & TotalPrimary & ", date_modified = '" & FullDate24HR() & "' WHERE inventory_id = " & INVENTORY_DATATABLE(i)(1)
                End If

                Using mCmd = New MySqlCommand(Query, UpdateInventoryCon)
                    mCmd.ExecuteNonQuery()
                    mCmd.Dispose()
                End Using

            Next
            UpdateInventoryCon.Close()
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "ModUpdate/UpdateInventory(): " & ex.ToString, "Critical")
        End Try
    End Sub
End Module
