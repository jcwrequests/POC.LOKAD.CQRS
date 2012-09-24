Imports System
Imports System.Diagnostics
Imports System.Text
Imports Lokad.Cqrs
Imports Lokad.Cqrs.Envelope.Events
Imports ServiceStack.Text

Public NotInheritable Class ConsoleObserver
    Implements IObserver(Of ISystemEvent)

    ReadOnly _watch As Stopwatch = Stopwatch.StartNew

    Public Sub OnError([error] As Exception) Implements IObserver(Of ISystemEvent).OnError

    End Sub

    Public Sub OnNext(value As ISystemEvent) Implements IObserver(Of ISystemEvent).OnNext
        [When](value)
    End Sub

    Public Sub OnCompleted() Implements IObserver(Of ISystemEvent).OnCompleted

    End Sub
    Public Sub [When](ed As EnvelopeDispatched)
        Stop


    End Sub
    Public Sub [When](e As SystemObserver.MessageEvent)
        Stop
    End Sub
    Public Sub [When](e As EnvelopeQuarantined)
        Stop
    End Sub
    Public Sub [When](e As Object)

        Console.WriteLine(e.ToString)
    End Sub
End Class
