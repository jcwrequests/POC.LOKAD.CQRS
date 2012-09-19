Imports Lokad.Cqrs.AtomicStorage
Imports System.Runtime.Serialization
Imports ProtoBuf
Public Class TestStrategy
    Implements IDocumentStrategy

    Public Function Deserialize(Of TEntity)(stream As System.IO.Stream) As TEntity Implements IDocumentStrategy.Deserialize
        Return Serializer.Deserialize(Of TEntity)(stream)
    End Function

    Public Function GetEntityBucket(Of TEntity)() As String Implements IDocumentStrategy.GetEntityBucket
        Return String.Format("test-{0}", GetType(TEntity).Name.ToLowerInvariant)
    End Function

    Public Sub Serialize(Of TEntity)(entity As TEntity, stream As System.IO.Stream) Implements IDocumentStrategy.Serialize
        ProtoBuf.Serializer.Serialize(Of TEntity)(stream, entity)
    End Sub

    Public Function GetEntityLocation(Of TEntity)(key As Object) As String Implements IDocumentStrategy.GetEntityLocation
        Return key.ToString
    End Function
End Class
