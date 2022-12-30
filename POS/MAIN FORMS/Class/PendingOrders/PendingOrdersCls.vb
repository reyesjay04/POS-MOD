Public Class PendingOrdersCls
    Property Name As String
    Property Items As List(Of OrdersCls)
    Public Class OrdersCls
        Property ProductName As String
        Property Quantity As String
        Property Price As Double
        Property Total As Double
        Property CreatedAt As String
        Property Guid As String
        Property Status As Integer
        Property Increment As String
    End Class
End Class
