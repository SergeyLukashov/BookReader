Imports System.ComponentModel

Public Class Team
    Implements INotifyPropertyChanged
    'здесь придется прописывать свойства явно
    Private _country As String
    Public Property country() As String
        Get
            Return _country
        End Get
        Set(ByVal value As String)
            _country = value
            'здесь свойство уведомляет об изменении
            OnPropertyChanged(New PropertyChangedEventArgs("country"))
        End Set
    End Property

    Private _place As Byte
    Public Property place() As Byte
        Get
            Return _place
        End Get
        Set(ByVal value As Byte)
            _place = value
            OnPropertyChanged(New PropertyChangedEventArgs("place"))
        End Set
    End Property

    Private _score As Byte
    Public Property score() As Byte
        Get
            Return _score
        End Get
        Set(ByVal value As Byte)
            _score = value
            OnPropertyChanged(New PropertyChangedEventArgs("score"))
        End Set
    End Property

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

    Public Sub OnPropertyChanged(ByVal e As System.ComponentModel.PropertyChangedEventArgs)
        If Not PropertyChangedEvent Is Nothing Then
            RaiseEvent PropertyChanged(Me, e)
        End If
    End Sub
End Class
