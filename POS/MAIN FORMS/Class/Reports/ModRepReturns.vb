Imports MySql.Data.MySqlClient
Module ModRepReturns
    Public Function GetProducts(ByVal trxNo As String) As List(Of RepReturnsCls)
        Dim RepReturns As New List(Of RepReturnsCls)
        Dim ConnectionLocal As MySqlConnection = LocalhostConn()

        Try
            Using mCmd = New MySqlCommand("", ConnectionLocal)
                With mCmd
                    .Parameters.Clear()
                    .CommandText = "SELECT `product_name`, `quantity`, `price`, `total` FROM loc_daily_transaction_details WHERE transaction_number = @trxNo"
                    .Parameters.AddWithValue("@trxNo", trxNo)
                    .Prepare()
                    Using reader = .ExecuteReader
                        While reader.Read
                            Dim Returns As New RepReturnsCls
                            Returns.ProductName = reader("product_name")
                            Returns.ProductQuantity = reader("quantity")
                            Returns.Price = reader("price")
                            Returns.Total = reader("total")
                            RepReturns.Add(Returns)
                        End While
                    End Using

                End With
            End Using
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        Return RepReturns
    End Function

End Module
