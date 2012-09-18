Imports System.Runtime.Serialization
Imports ProtoBuf
Public Class CustomerId
    Inherits AbstractIdentity(Of Integer)
    Public Const TagValue As String = "customer"

    Public Sub New()

    End Sub
    Public Sub New(id As Integer)
        Me.Id = id
    End Sub
    Public Overrides Function GetTag() As String
        Return TagValue
    End Function

    <DataMember(Order:=1)> <ProtoMember(1)>
    Public Overrides Property Id As Integer
        Get
            Return MyBase.Id
        End Get
        Protected Set(value As Integer)
            MyBase.Id = value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return String.Format("{0}-{1}", TagValue, Id.ToString.ToLowerInvariant)
    End Function
End Class
