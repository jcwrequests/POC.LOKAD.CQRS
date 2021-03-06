﻿Imports System.ComponentModel
Imports System.Runtime.Serialization
Imports Lokad.Cqrs.AtomicStorage
Imports Lokad.Cqrs.Build
Imports ProtoBuf
Imports System.Security.Cryptography
Imports System.Text

Public Interface IEvent

End Interface
Public Interface ICommand

End Interface

<ProtoContract()>
<DataContract()>
Public NotInheritable Class CreateCustomer
    Implements ICommand

    Public Sub New(CustomerID As CustomerId, CustomerName As String)
        Me.CustomerID = CustomerID
        Me.CustomerName = CustomerName
    End Sub
    <ProtoMember(1)> <DataMember()> Public Property CustomerID As CustomerId
    <ProtoMember(2)> <DataMember()> Public Property CustomerName As String

End Class

<ProtoContract()>
<DataContract()>
Public NotInheritable Class CustomerCreated
    Implements IEvent
    Public Sub New()

    End Sub
    <ProtoMember(1)> <DataMember()> Public Property CustomerID As CustomerId
    <ProtoMember(2)> <DataMember()> Public Property CustomerName As String
End Class

<ProtoContract()>
<DataContract()>
Public NotInheritable Class HelpCustomer
    Implements ICommand
    Public Sub New()

    End Sub
    <ProtoMember(1)> <DataMember()> Public Property CustomerID As CustomerId
End Class
<ProtoContract()>
<DataContract()>
Public NotInheritable Class CustomerHelped
    Implements IEvent
    Public Sub New()

    End Sub
    <ProtoMember(1)> <DataMember()> Public Property CustomerID As CustomerId
End Class

<ProtoContract()>
<DataContract()>
Public NotInheritable Class Customer
    <ProtoMember(1)> <DataMember()> Public Property Id As CustomerId
    <ProtoMember(2)> <DataMember()> Public Property Name As String
    <ProtoMember(3)> <DataMember()> Public Property TimesHelped As Integer

    Public Sub New(id As CustomerId, name As String)
        Me.Name = name
        Me.Id = id
        Me.TimesHelped = 0
    End Sub
    Public Sub New(id As CustomerId, name As String, TimesHelped As Integer)
        Me.Name = name
        Me.Id = id
        Me.TimesHelped = TimesHelped
    End Sub
    Public Sub New()

    End Sub
End Class
<ProtoContract()>
<DataContract()>
Public NotInheritable Class CustomerIndexLookUp
    Private _id As Integer
    Public Sub New()

    End Sub
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
        Dim b = GetKey(e.CustomerID.ToString)

        Me._writer.Add(b, New CustomerIndexLookUp() With {.ID = e.CustomerID.Id})

    End Sub



End Class
