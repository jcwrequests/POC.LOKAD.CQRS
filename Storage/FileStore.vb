Public Class FileStore
    Implements IEventStore

    Public Sub AppendToStream(id As IIdentity, expectedVersion As Long, events As System.Collections.Generic.ICollection(Of IEvent)) Implements IEventStore.AppendToStream

    End Sub

    Public Function LoadEventStream(id As IIdentity) As EventStream Implements IEventStore.LoadEventStream

    End Function

    Public Function LoadEventStream(id As IIdentity, skipEvents As Long, maxCount As Integer) As EventStream Implements IEventStore.LoadEventStream

    End Function
End Class
