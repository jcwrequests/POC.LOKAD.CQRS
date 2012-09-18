Imports System.Runtime.Serialization

<Serializable()>
Public Class OptimisticConcurrencyException
    Inherits Exception

    Private _actualVersion As Long
    Private _expectedVersion As Long
    Private _id As IIdentity
    Private _actualEvents As IList(Of IEvent)

    Public Sub New(message As String, actualVersion As Long, expectedVersion As Long, id As IIdentity, serverEvents As IList(Of IEvent))
        MyBase.New(message)
        Me.ActualVersion = actualVersion
        Me.ExpectedVersion = expectedVersion
        Me.ID = id
        Me.ActualEvents = serverEvents
    End Sub

    Public Shared Function Create(actual As Long, expected As Long, id As IIdentity, serverEvents As IList(Of IEvent)) As OptimisticConcurrencyException
        Dim message = String.Format("Expected v{0} but found v{1} in stream '{2}'", expected, actual, id)
        Return New OptimisticConcurrencyException(message, actual, expected, id, serverEvents)
    End Function

    Protected Sub New(info As SerializationInfo, context As StreamingContext)
        MyBase.New(info, context)
    End Sub

    Public Property ActualVersion As Long
        Get
            Return _actualVersion
        End Get
        Private Set(value As Long)
            _actualVersion = value
        End Set
    End Property
    Public Property ExpectedVersion As Long
        Get
            Return _expectedVersion
        End Get
        Private Set(value As Long)
            _expectedVersion = value
        End Set
    End Property
    Public Property ID As IIdentity
        Get
            Return _id
        End Get
        Private Set(value As IIdentity)
            _id = value
        End Set
    End Property
    Public Property ActualEvents As IList(Of IEvent)
        Get
            Return _actualEvents
        End Get
        Private Set(value As IList(Of IEvent))
            _actualEvents = value
        End Set
    End Property
End Class
