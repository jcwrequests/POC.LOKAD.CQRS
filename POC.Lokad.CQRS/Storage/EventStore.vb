Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Runtime.Serialization.Formatters.Binary

Public Class EventStore
    Implements IEventStore
    ReadOnly _formatter As New BinaryFormatter
    ReadOnly _appendOnlyStore As IAppendOnlyStore

    Public Sub New(appendOnlyStore As IAppendOnlyStore)
        _appendOnlyStore = appendOnlyStore
    End Sub
    Public Function SerializeEvent(e() As IEvent) As Byte()
        Using mem As New MemoryStream
            _formatter.Serialize(mem, e)
            Return mem.ToArray
        End Using
    End Function

    Public Function DeserializeEvent(data() As Byte) As IEvent()
        Using mem As New MemoryStream(data)
            Return CType(_formatter.Deserialize(mem), IEvent())
        End Using
    End Function
    Public Sub AppendToStream(id As IIdentity, expectedVersion As Long, events As System.Collections.Generic.ICollection(Of IEvent)) Implements IEventStore.AppendToStream
        If events.Count.Equals(0) Then Return

        Dim name = id.ToString
        Dim data = SerializeEvent(events.ToArray)

        Try
            _appendOnlyStore.Append(name, data, expectedVersion)
        Catch ex As AppendOnlyStoreConcurrencyException
            Dim server = LoadEventStream(id, 0, Integer.MaxValue)
            Throw OptimisticConcurrencyException.Create(server.Version, ex.ExpectedStreamVersion, id, server.Events)
        End Try

    End Sub

    Public Function LoadEventStream(id As IIdentity) As EventStream Implements IEventStore.LoadEventStream
        Return LoadEventStream(id, 0, Integer.MaxValue)
    End Function

    Public Function LoadEventStream(id As IIdentity, skipEvents As Long, maxCount As Integer) As EventStream Implements IEventStore.LoadEventStream
        Dim name = id.ToString
        Dim records = _appendOnlyStore.ReadRecords(name, skipEvents, maxCount)
        Dim _stream As New EventStream
        For Each record In records
            _stream.Events.AddRange(DeserializeEvent(record.Data))
            _stream.Version = record.Version
        Next
        Return _stream
    End Function
End Class
