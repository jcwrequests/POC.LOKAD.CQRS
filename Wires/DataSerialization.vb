Imports Lokad.Cqrs.Envelope
Imports ProtoBuf.Meta
Imports Lokad.Cqrs.Evil
Imports ServiceStack.Text
Imports System.IO


Public Class DataSerialization
    Inherits AbstractMessageSerializer

    Public Sub New(knownTypes As ICollection(Of Type))
        MyBase.New(knownTypes)
        RuntimeTypeModel.Default(GetType(DateTimeOffset)).Add("m_dateTime", "m_offsetMinutes")
    End Sub

    Protected Overrides Function PrepareFormatter(type As Type) As Formatter
        Dim name = ContractEvil.GetContractReference(type)

        Return New Formatter(name,
                             type,
                             Function(s) JsonSerializer.DeserializeFromStream(type, s),
                             Sub(o, s)
                                 Using writer As New StreamWriter(s)

                                     writer.WriteLine()
                                     writer.WriteLine(JsvFormatter.Format(JsonSerializer.SerializeToString(o, type)))
                                 End Using
                             End Sub)
    End Function
End Class
