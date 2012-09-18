Imports System.Runtime.Serialization

<Serializable()>
Public Class AppendOnlyStoreConcurrencyException
    Inherits Exception

    Private _expectedStreamVersion As Long
    Private _actualStreamVersion As Long
    Private _streamName As String

    Protected Sub New(info As SerializationInfo, context As StreamingContext)
        MyBase.New(info, context)
    End Sub
    Public Sub New(expectedVersion As Long, actualVersion As Long, name As String)
        MyBase.New(String.Format("Expected version {0} in stream '{1}' but got {2}", expectedVersion, name, actualVersion))
    End Sub
    Public Property ExpectedStreamVersion As Long
        Get
            Return _expectedStreamVersion
        End Get
        Private Set(value As Long)
            _expectedStreamVersion = value
        End Set
    End Property
    Public Property ActualStreamVersion As Long
        Get
            Return _actualStreamVersion
        End Get
        Private Set(value As Long)
            _actualStreamVersion = value
        End Set
    End Property
    Public Property StreamName As String
        Get
            Return _streamName
        End Get
        Private Set(value As String)
            _streamName = value
        End Set
    End Property


End Class
