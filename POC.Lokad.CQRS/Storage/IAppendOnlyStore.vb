Imports System
Imports System.Collections.Generic
Imports System.Runtime.Serialization

Public Interface IAppendOnlyStore
    Inherits IDisposable
    Sub Append(streamName As String, data() As Byte, Optional expectedStreamVersion As Long = -1)
    Function ReadRecords(streamName As String, afterVersion As Long, maxCount As Integer) As IEnumerable(Of DataWithVersion)
    Function ReadRecords(afterVersion As Long, maxCount As Integer) As IEnumerable(Of DataWithName)
    Sub Close()
End Interface
