Imports System
Imports System.Transactions


Public NotInheritable Class TransactionTester
    Implements IEnlistmentNotification
    Public OnCommit As Action = Sub()

                                End Sub
    Public OnRollback As Action = Sub()

                                  End Sub
    Public Sub New()
        If Transaction.Current Is Nothing Then Throw New InvalidOperationException("Ambient transaction not present!")

        Transaction.Current.EnlistVolatile(Me, EnlistmentOptions.None)
    End Sub

    Public Sub Commit(enlistment As System.Transactions.Enlistment) Implements System.Transactions.IEnlistmentNotification.Commit
        OnCommit()
        enlistment.Done()
    End Sub

    Public Sub InDoubt(enlistment As System.Transactions.Enlistment) Implements System.Transactions.IEnlistmentNotification.InDoubt
        enlistment.Done()
    End Sub

    Public Sub Prepare(preparingEnlistment As System.Transactions.PreparingEnlistment) Implements System.Transactions.IEnlistmentNotification.Prepare
        preparingEnlistment.Prepared()
    End Sub

    Public Sub Rollback(enlistment As System.Transactions.Enlistment) Implements System.Transactions.IEnlistmentNotification.Rollback
        OnRollback()
        enlistment.Done()
    End Sub
End Class
