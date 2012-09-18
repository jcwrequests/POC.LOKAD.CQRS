Public Class DataWithName
    Public ReadOnly Name As String
    Public ReadOnly Data() As Byte
    Public Sub New(name As String, data() As Byte)
        Me.Name = name
        Me.Data = data
    End Sub
End Class
