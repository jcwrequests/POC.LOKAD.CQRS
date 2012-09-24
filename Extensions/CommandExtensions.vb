Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Lokad.Cqrs
Imports System.Transactions

Module CommandExtensions
    <Extension> Public Sub SendBatch(sender As MessageSender, WithTransation As Boolean, ParamArray commands() As ICommand)
        Dim scopeOption As TransactionScopeOption = TransactionScopeOption.Suppress

        If WithTransation Then scopeOption = TransactionScopeOption.RequiresNew

        Using tx As New TransactionScope(scopeOption)
            For Each cmd In commands
                sender.Send(cmd)
            Next
            tx.Complete()
        End Using

    End Sub
End Module
