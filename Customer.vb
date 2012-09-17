Imports System.ComponentModel
Imports System.Runtime.Serialization
Imports Lokad.Cqrs.AtomicStorage
Imports Lokad.Cqrs.Build
Imports ProtoBuf
Imports System.Security.Cryptography
Imports System.Text

<ProtoContract()>
<DataContract()>
Public NotInheritable Class CreateCustomer
    <ProtoMember(1)> <DataMember()> Public CustomerName As String
    <ProtoMember(2)> <DataMember()> Public CustomerID As Integer
End Class

<ProtoContract()>
<DataContract()>
Public NotInheritable Class CustomerCreated
    <ProtoMember(1)> <DataMember()> Public CustomerID As Integer
    <ProtoMember(2)> <DataMember()> Public CustomerName As String
End Class

<ProtoContract()>
<DataContract()>
Public NotInheritable Class Customer
    <ProtoMember(1)> <DataMember()> Public Name As String
    <ProtoMember(2)> <DataMember()> Public Id As Integer
    Public Sub New(id As Integer, name As String)
        Me.Name = name
        Me.Id = id
    End Sub
End Class
<ProtoContract()>
<DataContract()>
Public NotInheritable Class CustomerIndexLookUp
    Private _id As Integer

    <ProtoMember(1)> <DataMember(Order:=1)>
    Public Property ID As Integer
        Get
            Return _id
        End Get
        Set(value As Integer)
            _id = value
        End Set
    End Property
End Class

Public NotInheritable Class CustomerIndexProjection
    ReadOnly _writer As IDocumentWriter(Of Integer, CustomerIndexLookUp)

    Public Sub New(writer As IDocumentWriter(Of Integer, CustomerIndexLookUp))
        _writer = writer
    End Sub

    Function GetKey(name As String) As Byte
        Using md As New MD5CryptoServiceProvider()
            Dim bytes = Encoding.UTF8.GetBytes(name.ToLowerInvariant)
            Return md.ComputeHash(bytes)(0)
        End Using
    End Function

    Public Sub [When](e As CustomerCreated)
        Dim b = GetKey(e.CustomerID)

        Me._writer.Add(b, New CustomerIndexLookUp() With {.ID = e.CustomerID})

    End Sub



End Class
