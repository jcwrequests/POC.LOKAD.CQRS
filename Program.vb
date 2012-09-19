Imports Lokad.Cqrs.Envelope
Imports Lokad.Cqrs.Build
Imports Lokad.Cqrs
Imports Lokad.Cqrs.AtomicStorage
Imports System.Transactions
Imports System.Threading

Module Program

    Sub Main()

        'var observer = new ConsoleObserver();
        '    SystemObserver.Swap(observer);
        '    Context.SwapForDebug(s => SystemObserver.Notify(s));

        'Dim streamer = EnvelopeStreamer.CreateDefault(GetType(CreateCustomer),
        '                                              GetType(CustomerCreated),
        '                                              GetType(HelpCustomer),
        '                                              GetType(CustomerHelped))

        Dim streamer As New EnvelopeStreamer(New DataSerialization({GetType(CreateCustomer),
                                                                    GetType(CustomerCreated),
                                                                    GetType(HelpCustomer),
                                                                    GetType(CustomerHelped)}))


        Dim builder As New CqrsEngineBuilder(streamer)

        Dim account = FileStorage.CreateConfig(New IO.DirectoryInfo("..\..\Store"))
        account.Wipe()
        account.EnsureDirectory()

        Dim tapesContainer = account.CreateAppendOnlyStore("sample-store")


        Dim nuclear = New NuclearStorage(account.CreateDocumentStore(New TestStrategy))


        Dim inbox = account.CreateInbox("input")
        Dim projectionsInbox = account.CreateInbox("views")

        Dim sender = account.CreateMessageSender(streamer, "input")
        Dim projector = account.CreateMessageSender(streamer, "views")

        Dim handler As New RedirectToCommand
        handler.WireToLambda(Of CreateCustomer)(Sub(customer) consume(customer, nuclear, sender))
        handler.WireToLambda(Of HelpCustomer)(Sub(customer)
                                                  Dim c = nuclear.GetEntity(Of Customer)(customer.CustomerID)
                                                  If c.HasValue Then
                                                      Dim times = c.Value.TimesHelped + 1
                                                      Stop
                                                  End If

                                                  sender.Send(New CustomerHelped() With {.CustomerID = New CustomerId(customer.CustomerID.Id)})
                                              End Sub)

        handler.WireToLambda(Of CustomerCreated)(Sub(m)
                                                     nuclear.AddEntity(m.CustomerID, m)
                                                     projector.Send(m)
                                                 End Sub)

        handler.WireToLambda(Of CustomerHelped)(Sub(m) nuclear.AddEntity(m.CustomerID, m))

        builder.Handle(inbox, Sub(envelope)
                                  Using tx As New TransactionScope(TransactionScopeOption.RequiresNew)
                                      handler.Invoke(envelope.Message)
                                      tx.Complete()
                                  End Using
                              End Sub)


        Dim events As New RedirectToDynamicEvent()

        Dim docs = nuclear.Container

        events.WireToWhen(New CustomerIndexProjection(docs.GetWriter(Of Integer, CustomerIndexLookUp)))
        builder.Handle(projectionsInbox, Sub(envelope)
                                             Dim content = envelope.Message
                                             events.InvokeEvent(content)
                                         End Sub)



        Using cts As New CancellationTokenSource()
            Using engine = builder.Build(cts.Token)
                Dim task = engine.Start(cts.Token)

                'Test Sending a batch that rollsback commands on failure
                sender.Send({New CreateCustomer With {.CustomerID = New CustomerId(1), .CustomerName = "Rinat Abdullin"},
                                  New CreateCustomer With {.CustomerID = New CustomerId(2), .CustomerName = "Jason Wyglendowski"}})

                'Test Sending a successfull item
                sender.Send(New CreateCustomer With {.CustomerID = New CustomerId(1),
                                                        .CustomerName = "Rinat Abdullin"})

                sender.Send(New HelpCustomer With {.CustomerID = New CustomerId(1)})

                Console.WriteLine("running")
                Console.WriteLine("Press enter to stop")
                Console.ReadLine()


                cts.Cancel()
                If Not task.Wait(5000) Then
                    Console.WriteLine("Terminiating")
                End If

            End Using
        End Using

        'Dim fileContainer As New TapeStorage.FileTapeContainer()
        'Dim fileStream = fileContainer.GetOrCreateStream("test-createcustomer")
        'Dim records = fileStream.ReadRecords(0, Integer.MaxValue)
        'Dim fileStream As New TapeStorage.FileTapeStream("..\..\Store\test-createcustomer")


    End Sub



    Public Sub consume(cmd As CreateCustomer, storage As NuclearStorage, sender As MessageSender)
        Dim tester As New TransactionTester() With {.OnCommit = Sub()
                                                                    Dim customer As New Customer(cmd.CustomerID, cmd.CustomerName)

                                                                    storage.AddEntity(cmd.CustomerID, customer)

                                                                    sender.Send(New CustomerCreated With {.CustomerID = cmd.CustomerID, .CustomerName = cmd.CustomerName})

                                                                End Sub}
        If cmd.CustomerID.Id.Equals(2) Then Throw New InvalidOperationException("Failed Requested")

    End Sub
End Module
