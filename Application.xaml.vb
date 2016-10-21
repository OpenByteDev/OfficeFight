Class Application

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

    Sub New()
        EventManager.RegisterClassHandler(GetType(Window), Keyboard.KeyDownEvent, New KeyEventHandler(Sub(sender As Object, e As KeyEventArgs)
                                                                                                          G.RaiseKeyboardInputEvent(sender, e)
                                                                                                      End Sub), False)
        Player.SHOOT_INTERVAL = My.Settings.SHOOT_INTERVAL
    End Sub

End Class
