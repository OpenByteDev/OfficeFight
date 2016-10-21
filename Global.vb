Imports System.Timers

Module G

    Public WithEvents SlowClock As New Timer(40)
    Public WithEvents FastClock As New Timer(10)

    Public ScreenWidth As Integer = SystemParameters.WorkArea.Width
    Public ScreenHeight As Integer = SystemParameters.WorkArea.Height
    Friend Control As Control

    Public Event KeyboardInput(sender As Object, e As KeyEventArgs)

    Public Sub RaiseKeyboardInputEvent(sender As Object, e As KeyEventArgs)
        RaiseEvent KeyboardInput(sender, e)
    End Sub

    Public Function BoxToRectangle(Box As Box) As Rect
        Return New Rect(Box.X, Box.Y, Box.Width, Box.Height)
    End Function

End Module
