Public Interface IIdentity
    Function GetId() As String
    Function GetTag() As String
End Interface

Public MustInherit Class AbstractIdentity(Of TKey)
    Implements IIdentity

    Private _id As TKey

    Public Overridable Property Id As TKey
        Get
            Return _id
        End Get
        Protected Set(value As TKey)
            _id = value
        End Set
    End Property


    Public Function GetId() As String Implements IIdentity.GetId
        Return Id.ToString
    End Function

    Public Overridable Function GetTag() As String Implements IIdentity.GetTag
        Return [GetType]().ToString.ToLowerInvariant
    End Function
    Public Overrides Function Equals(obj As Object) As Boolean
        If ReferenceEquals(Nothing, obj) Then Return False
        If ReferenceEquals(Me, obj) Then Return True

        Dim identity = CType(obj, AbstractIdentity(Of TKey))
        If Not identity Is Nothing Then
            Return identity.Id.Equals(Id) And String.Equals(identity.GetTag, GetTag)
        End If
        Return False
    End Function
    Public Overrides Function ToString() As String
        Return String.Format("{0}-{1}", [GetType].Name.Replace("Id", ""), Id)
    End Function
    Public Overrides Function GetHashCode() As Integer
        Return (Id.GetHashCode() * 397) Xor (GetTag().GetHashCode())
    End Function
End Class
Public Interface IApplicationService
    Sub Execute(command As ICommand)
End Interface