Imports Lokad.Cqrs.Envelope
Imports Lokad.Cqrs.Build
Imports Lokad.Cqrs
Imports Lokad.Cqrs.AtomicStorage
Imports System.Transactions
Imports System.Threading

Public Class ServiceEnvironment
    Private observer As New ConsoleObserver
    Private builder As CqrsEngineBuilder
    Private inboxFactory As InboxFactory
    Private events As RedirectToDynamicEvent
    Private _inbox As Partition.FilePartitionInbox
    Private _sender As MessageSender
    Public Sub Send(command As ICommand)
        _sender.Send(command)
    End Sub
    Public Sub SendBatch(ParamArray commands() As ICommand)
        _sender.SendBatch(True, commands)
    End Sub
    Public Function BuildEngine() As CqrsEngineBuilder


        SystemObserver.Swap(observer)

        Dim streamer As New EnvelopeStreamer(New DataSerialization({GetType(CreateCustomer),
                                                                   GetType(CustomerCreated),
                                                                   GetType(HelpCustomer),
                                                                   GetType(CustomerHelped)}))


        builder = New CqrsEngineBuilder(streamer)

        inboxFactory = New InboxFactory(New IO.DirectoryInfo("..\..\Store"), streamer)



        Dim tapesContainer = inboxFactory.CreateAppendOnlyStore("sample-store")


        Dim nuclear = New NuclearStorage(inboxFactory.CreateDocumentStore(New TestStrategy))


        _inbox = inboxFactory.CreateInbox("input")
        Dim projectionsInbox = inboxFactory.CreateInbox("views")



        _sender = inboxFactory.CreateMessageSender("input")
        Dim projector = inboxFactory.CreateMessageSender("views")
        Dim hFactor As New HandlerFactory(nuclear, sender, projector)
        Dim handler = hFactor.Create()


        builder.Handle(_inbox, Sub(envelope)
                                   Using tx As New TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew)
                                       handler.Invoke(envelope.Message)
                                       tx.Complete()
                                   End Using

                               End Sub)


        events = New RedirectToDynamicEvent()

        Dim docs = nuclear.Container

        events.WireToWhen(New CustomerIndexProjection(docs.GetWriter(Of Integer, CustomerIndexLookUp)))

        builder.Handle(projectionsInbox, Sub(envelope)
                                             Dim content = envelope.Message
                                             events.InvokeEvent(content)
                                         End Sub)

        Return builder

    End Function
End Class
