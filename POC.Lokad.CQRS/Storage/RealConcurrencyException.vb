Imports System.Runtime.Serialization

<Serializable()>
Public Class RealConcurrencyException
    Inherits Exception

    Public Sub New()

    End Sub
    Public Sub New(message As String)
        MyBase.New(message)
    End Sub
    Public Sub New(message As String, inner As Exception)
        MyBase.New(message, inner)
    End Sub

    Protected Sub New(info As SerializationInfo, context As StreamingContext)
        MyBase.New(info, context)
    End Sub
End Class
