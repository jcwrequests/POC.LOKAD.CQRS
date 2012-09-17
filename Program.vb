Imports Lokad.Cqrs.Envelope
Imports Lokad.Cqrs.Build
Imports Lokad.Cqrs
Imports Lokad.Cqrs.AtomicStorage
Imports System.Transactions
Imports System.Threading

Module Program

    Sub Main()
        Dim streamer = EnvelopeStreamer.CreateDefault(GetType(CreateCustomer), GetType(CustomerCreated))
        Dim builder As New CqrsEngineBuilder(streamer)

        Dim account = FileStorage.CreateConfig(New IO.DirectoryInfo("..\..\Store"))
        account.Wipe()
        account.EnsureDirectory()

        Dim nuclear = account.CreateNuclear(New TestStrategy)
        Dim inbox = account.CreateInbox("input")
        Dim projectionsInbox = account.CreateInbox("views")

        Dim sender = account.CreateSimpleSender(streamer, "input")

        Dim handler As New RedirectToCommand
        handler.WireToLambda(Of CreateCustomer)(Sub(customer) consume(customer, nuclear, sender))

        handler.WireToLambda(Of CustomerCreated)(Sub(m)
                                                     nuclear.AddEntity(m.CustomerID, m)
                                                 End Sub)

        builder.Handle(inbox, Sub(envelope)
                                  Using tx As New TransactionScope(TransactionScopeOption.RequiresNew)
                                      handler.InvokeMany(envelope.SelectContents)
                                      tx.Complete()
                                  End Using
                              End Sub)


        Dim events As New RedirectToDynamicEvent()
        Dim docs = nuclear.Container

        events.WireToWhen(New CustomerIndexProjection(docs.GetWriter(Of Integer, CustomerIndexLookUp)))
        builder.Handle(projectionsInbox, Sub(envelope)
                                             Dim content = envelope.Items(0).Content
                                             events.InvokeEvent(content)
                                         End Sub)



        Using cts As New CancellationTokenSource()
            Using engine = builder.Build
                Dim task = engine.Start(cts.Token)

                'Test Sending a batch that rollsback commands on failure
                sender.SendBatch({New CreateCustomer With {.CustomerID = 1, .CustomerName = "Rinat Abdullin"},
                                  New CreateCustomer With {.CustomerID = 2, .CustomerName = "Jason Wyglendowski"}})

                'Test Sending a successfull batch
                sender.SendBatch({New CreateCustomer With {.CustomerID = 1, .CustomerName = "Rinat Abdullin"}})

                Console.WriteLine("running")
                Console.WriteLine("Press enter to stop")
                Console.ReadLine()
                cts.Cancel()
                If Not task.Wait(5000) Then
                    Console.WriteLine("Terminiating")
                End If

            End Using
        End Using



    End Sub



    Public Sub consume(cmd As CreateCustomer, storage As NuclearStorage, sender As SimpleMessageSender)
        Dim tester As New TransactionTester() With {.OnCommit = Sub()
                                                                    
                                                                    storage.AddEntity(cmd.CustomerID, cmd)
                                                                    sender.SendOne(New CustomerCreated With {.CustomerID = cmd.CustomerID, .CustomerName = cmd.CustomerName})

                                                                End Sub}
        If cmd.CustomerID.Equals(2) Then Throw New InvalidOperationException("Failed Requested")

    End Sub
End Module
