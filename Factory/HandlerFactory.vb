Imports Lokad.Cqrs.Envelope
Imports Lokad.Cqrs.Build
Imports Lokad.Cqrs
Imports Lokad.Cqrs.AtomicStorage
Imports System.Transactions
Imports System.Threading

Public Class HandlerFactory
    Private _nuclear As NuclearStorage
    Private _sender As MessageSender
    Private _projector As MessageSender

    Public Sub New(nuclear As NuclearStorage, sender As MessageSender, projector As MessageSender)
        _nuclear = nuclear
        _sender = sender
        _projector = projector
    End Sub
    Public Function Create() As RedirectToCommand
        Dim handler As New RedirectToCommand
        handler.WireToLambda(Of CreateCustomer)(Sub(customer)
                                                    consume(customer, _nuclear, _sender)
                                                End Sub)
        handler.WireToLambda(Of HelpCustomer)(Sub(customer)
                                                  Dim c = _nuclear.GetEntity(Of Customer)(customer.CustomerID)
                                                  If c.HasValue Then
                                                      Dim times = c.Value.TimesHelped + 1


                                                  End If

                                                  _sender.Send(New CustomerHelped() With {.CustomerID = New CustomerId(customer.CustomerID.Id)})
                                              End Sub)

        handler.WireToLambda(Of CustomerCreated)(Sub(m)
                                                     _nuclear.AddEntity(m.CustomerID, m)
                                                     _projector.Send(m)
                                                 End Sub)

        handler.WireToLambda(Of CustomerHelped)(Sub(m) _nuclear.AddEntity(m.CustomerID, m))

        Return handler
    End Function
    Public Sub consume(cmd As CreateCustomer, storage As NuclearStorage, sender As MessageSender)
        Dim tester As New TransactionTester() With {.OnCommit = Sub()
                                                                    Dim customer As New Customer(cmd.CustomerID, cmd.CustomerName)

                                                                    storage.AddEntity(cmd.CustomerID, customer)

                                                                    sender.Send(New CustomerCreated With {.CustomerID = cmd.CustomerID, .CustomerName = cmd.CustomerName})

                                                                End Sub}
        If cmd.CustomerID.Id.Equals(2) Then Throw New InvalidOperationException("Failed Requested")


    End Sub
End Class
