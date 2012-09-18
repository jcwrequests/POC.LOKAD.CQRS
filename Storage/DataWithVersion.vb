Public NotInheritable Class DataWithVersion
    Public ReadOnly Version As Long
    Public ReadOnly Data() As Byte
    Public Sub New(version As Long, data() As Byte)
        Me.Version = version
        Me.Data = data
    End Sub
End Class
