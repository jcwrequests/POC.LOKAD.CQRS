
Public Interface IEventStore
    Function LoadEventStream(id As IIdentity) As EventStream
    Function LoadEventStream(id As IIdentity, skipEvents As Long, maxCount As Integer) As EventStream
    Sub AppendToStream(id As IIdentity, expectedVersion As Long, events As ICollection(Of IEvent))
End Interface
