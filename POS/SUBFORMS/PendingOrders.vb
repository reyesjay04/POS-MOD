Imports MySql.Data.MySqlClient
Public Class PendingOrders
    Property ListofPendingOrders As New List(Of PendingOrdersCls)
    Private Sub PendingOrders_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DatagridviewStyle(dgvOrders)
        ListofPendingOrders = GetPendingOrders()
        PopulateNames()
    End Sub
    Private Sub PendingOrders_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        posandpendingenter = False
        POS.Enabled = True
    End Sub
    Private Function GetPendingOrders() As List(Of PendingOrdersCls)
        Dim pendingOrdList As New List(Of PendingOrdersCls)
        Try
            Dim ConnectionLocal As MySqlConnection = LocalhostConn()
            Dim nameDt As New DataTable
            Using mCmd = New MySqlCommand("", ConnectionLocal)
                mCmd.CommandText = "SELECT DISTINCT customer_name FROM loc_pending_orders"
                Using reader = mCmd.ExecuteReader
                    nameDt.Load(reader)
                    reader.Dispose()
                End Using
                mCmd.Dispose()
            End Using

            For Each row As DataRow In nameDt.Rows
                Dim pendingOrd As New PendingOrdersCls
                pendingOrd.Name = row("customer_name").ToString
                pendingOrd.Items = GetCustomerOrders(pendingOrd.Name, ConnectionLocal)
                pendingOrdList.Add(pendingOrd)
            Next

            ConnectionLocal.Close()
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "PendingOrders/GetPendingOrders(): " & ex.ToString, "Critical")
        End Try
        Return pendingOrdList
    End Function

    Private Function GetCustomerOrders(ByVal customerName As String, ByVal mConn As MySqlConnection) As List(Of PendingOrdersCls.OrdersCls)
        Dim newItemList As New List(Of PendingOrdersCls.OrdersCls)
        Try
            Dim ordDt As New DataTable
            Using mCmd = New MySqlCommand("", mConn)
                mCmd.Parameters.Clear()
                mCmd.CommandText = "SELECT `product_name`, `product_quantity`, `product_price`, `product_total`, `created_at`, `guid`, `active`, `increment` FROM loc_pending_orders WHERE customer_name = @customerName"
                mCmd.Parameters.AddWithValue("@customerName", customerName)
                mCmd.Prepare()
                Using reader = mCmd.ExecuteReader
                    ordDt.Load(reader)
                    reader.Dispose()
                End Using
                mCmd.Dispose()
            End Using

            For Each row As DataRow In ordDt.Rows
                Dim itemList As New PendingOrdersCls.OrdersCls
                itemList.ProductName = row("product_name")
                itemList.Quantity = row("product_quantity")
                itemList.Price = row("product_price")
                itemList.Total = row("product_total")
                itemList.CreatedAt = row("created_at")
                itemList.Guid = row("guid")
                itemList.Status = row("active")
                itemList.Increment = row("increment")
                newItemList.Add(itemList)
            Next

        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "PendingOrders/GetCustomerOrders(): " & ex.ToString, "Critical")
        End Try
        Return newItemList
    End Function

    Public Sub PopulateNames()
        Try
            ComboBoxCustomerName.Items.Clear()

            If ListofPendingOrders.Count > 0 Then
                For Each names As PendingOrdersCls In ListofPendingOrders
                    ComboBoxCustomerName.Items.Add(names.Name)
                Next
                ComboBoxCustomerName.SelectedIndex = 0
            End If

        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "PendingOrders/PopulateNames(): " & ex.ToString, "Critical")
        End Try
    End Sub

    Public Sub LoadOrders(ByVal customerName As String)
        Try
            Dim custOrder = ListofPendingOrders.Find(Function(x) x.Name = customerName).Items
            dgvOrders.Rows.Clear()
            If custOrder.Count > 0 Then
                For Each ord As PendingOrdersCls.OrdersCls In custOrder
                    dgvOrders.Rows.Add(ord.ProductName, ord.Quantity, ord.Price, ord.Total, ord.CreatedAt, ord.Guid, ord.Status, ord.Increment)
                Next
            End If
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "PendingOrders/pendingloadorders(): " & ex.ToString, "Critical")
        End Try
    End Sub
    Public Sub PopulateOrders()
        Try
            Dim command As New MySqlCommand("SELECT * FROM `loc_pending_orders` WHERE customer_name = '" & ComboBoxCustomerName.Text & "'", LocalhostConn())
            Dim adapter As New MySqlDataAdapter(command)
            Dim table As New DataTable()
            adapter.Fill(table)
            For Each row As DataRow In table.Rows
                POS.DataGridViewOrders.Rows.Add(row("product_name"), row("product_quantity"), NUMBERFORMAT(row("product_price")), NUMBERFORMAT(row("product_total")), row("increment"), row("product_id"), row("product_sku"), row("product_category"), row("product_addon_id"), row("ColumnSumID"), row("ColumnInvID"), row("Upgrade"), row("Origin"), row("addontype"), 0, 0, 0, 0, 0, 0, 0, 0)
            Next row
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "PendingOrders/populatedatagridvieworders(): " & ex.ToString, "Critical")
        End Try
        Try
            Dim command As New MySqlCommand("SELECT `hold_id`, `sr_total`, `f_id`, `qty`, `id`, `nm`, `org_serve`, `cog`, `ocog`, `prd.addid`, `origin` FROM loc_hold_inventory WHERE name = '" & ComboBoxCustomerName.Text & "'", LocalhostConn())
            Dim adapter As New MySqlDataAdapter(command)
            Dim table As New DataTable()
            adapter.Fill(table)
            For Each row As DataRow In table.Rows
                POS.DataGridViewInv.Rows.Add(row("sr_total"), row("f_id"), row("qty"), row("id"), row("nm"), row("org_serve"), row("cog"), row("ocog"), row("prd.addid"), row("origin"))
            Next row
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "PendingOrders/populatedatagridvieworders(): " & ex.ToString, "Critical")
        End Try
    End Sub
    Private Sub ButtonSelectCustomer_Click(sender As Object, e As EventArgs) Handles ButtonSelectCustomer.Click
        If POS.DataGridViewOrders.RowCount = 0 Then
            If dgvOrders.RowCount > 0 Then
                PopulateOrders()
                GLOBAL_DELETE_ALL_FUNCTION(tablename:="loc_pending_orders", where:=" customer_name = '" & ComboBoxCustomerName.Text & "'")
                GLOBAL_DELETE_ALL_FUNCTION(tablename:="loc_hold_inventory", where:=" name = '" & ComboBoxCustomerName.Text & "'")

                posandpendingenter = False

                With POS
                    .Label76.Text = NUMBERFORMAT(SumOfColumnsToDecimal(.DataGridViewOrders, 3))
                    .TextBoxQTY.Text = 0
                    .Buttonholdoder.Enabled = True
                    .ButtonPayMent.Enabled = True
                    .ButtonPendingOrders.Enabled = False
                    Compute()
                End With
                AuditTrail.LogToAuditTrail("User", $"New pending order: ", "Normal")
                Me.Close()
            End If
        Else
            MessageBox.Show("Hold Order(s) or end the transaction first!", "Pending Orders", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Private Sub ComboBoxCustomerName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxCustomerName.SelectedIndexChanged
        LoadOrders(ComboBoxCustomerName.Text)
    End Sub

    Private Sub ButtonKeyboard_Click(sender As Object, e As EventArgs) Handles ButtonKeyboard.Click
        ShowKeyboard()
    End Sub
End Class