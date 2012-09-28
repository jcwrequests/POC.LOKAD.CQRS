Imports Lokad.Cqrs.Envelope
Imports Lokad.Cqrs.Build
Imports Lokad.Cqrs
Imports Lokad.Cqrs.AtomicStorage
Imports System.Transactions
Imports System.Threading

Module Program

    Sub Main()

        Dim service As New ServiceEnvironment()
        Dim builder = service.BuildEngine()



        Using cts As New CancellationTokenSource()
            Using engine = builder.Build(cts.Token)
                Dim task = engine.Start(cts.Token)

                'Test Sending a batch that rollsback commands on failure
                service.SendBatch({New CreateCustomer(CustomerId:=New CustomerId(1), CustomerName:="Rinat Abdullin"),
                                        New CreateCustomer(CustomerID:=New CustomerId(2), CustomerName:="Jason Wyglendowski")})


                'Test Sending a successfull item
                service.Send(New CreateCustomer(CustomerId:=New CustomerId(1), CustomerName:="Rinat Abdullin"))

                service.Send(New HelpCustomer With {.CustomerID = New CustomerId(1)})

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



   
End Module
