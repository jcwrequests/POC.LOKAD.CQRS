Imports Lokad.Cqrs.Envelope
Imports Lokad.Cqrs.Build
Imports Lokad.Cqrs
Imports Lokad.Cqrs.AtomicStorage
Imports System.Transactions
Imports System.Threading

Public Class InboxFactory
    Private _storage As FileStorageConfig
    Private _streamer As EnvelopeStreamer

    Public Sub New(storageDirectory As IO.DirectoryInfo, streamer As EnvelopeStreamer)
        _storage = FileStorage.CreateConfig(storageDirectory)
        _storage.Wipe()
        _storage.EnsureDirectory()
        _streamer = streamer
    End Sub
    Public Function CreateInbox(inbox As String) As Partition.FilePartitionInbox
        Return _storage.CreateInbox(inbox)
    End Function
    Public Function CreateAppendOnlyStore(store As String)
        Return _storage.CreateAppendOnlyStore(store)
    End Function
    Public Function CreateDocumentStore(strategy As IDocumentStrategy) As IDocumentStore
        Return _storage.CreateDocumentStore(strategy)
    End Function
    Public Function CreateMessageSender(inbox As String) As MessageSender
        Return _storage.CreateMessageSender(_streamer, inbox)
    End Function
End Class
