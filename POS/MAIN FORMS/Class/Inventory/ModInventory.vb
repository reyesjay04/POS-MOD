Imports MySql.Data.MySqlClient
Module ModInventory

    Public Function loadinventory() As List(Of InventoryCls)
        Dim InvClassList As New List(Of InventoryCls)
        Try

            Dim ConnectionLocal As MySqlConnection = LocalhostConn()
            Using mCmd = New MySqlCommand("", ConnectionLocal)
                With mCmd
                    .Parameters.Clear()
                    .CommandText = $"SELECT I.product_ingredients as Ingredients, i.sku , I.stock_primary as PrimaryValue ,
                                        CONCAT_WS(' ', ROUND(I.stock_secondary,0), F.secondary_unit) as UOM , ROUND(I.stock_no_of_servings,0) as NoofServings, 
                                        I.stock_status, I.critical_limit, I.date_modified
                                    FROM loc_pos_inventory I INNER JOIN loc_product_formula F ON F.server_formula_id = I.server_inventory_id 
                                    WHERE I.stock_status = 1 AND I.store_id = @ClientStoreID ORDER BY I.product_ingredients ASC"
                    .Parameters.AddWithValue("@ClientStoreID", ClientStoreID)
                    .Prepare()
                    Using reader = .ExecuteReader

                        While reader.Read
                            Dim invCls As New InventoryCls
                            invCls.Ingredientname = reader("Ingredients")
                            invCls.SKU = reader("sku")
                            invCls.PrimaryValue = reader("PrimaryValue")
                            invCls.UnitOfMeasure = reader("UOM")
                            invCls.Servings = reader("NoofServings")
                            invCls.Status = reader("stock_status")
                            invCls.Limit = reader("critical_limit")
                            invCls.DateCreated = reader("date_modified")
                            InvClassList.Add(invCls)
                        End While
                    End Using
                End With
            End Using

        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "Inventory/loadinventory(): " & ex.ToString, "Critical")
        End Try
        Return InvClassList
    End Function
End Module
