Option Strict On
Option Explicit Off

Class Shoot
    Implements Box

    Friend Const SpacerModifier As Double = 0.15
    Friend Const SPEED As Integer = CInt(Player.SPEED * 1.2)
    Friend Const DefaultSize As Integer = 40

    Public Property X As Double Implements Box.X
        Get
            Return Me.Left
        End Get
        Set(Value As Double)
            Me.Left = Value
        End Set
    End Property
    Public Property Y As Double Implements Box.Y
        Get
            Return Me.Top
        End Get
        Set(Value As Double)
            Me.Top = Value
        End Set
    End Property
    Public Shadows Property Width As Double Implements Box.Width
        Get
            Return MyBase.Width
        End Get
        Set(Value As Double)
            MyBase.Width = Value
        End Set
    End Property
    Public Shadows Property Height As Double Implements Box.Height
        Get
            Return MyBase.Height
        End Get
        Set(Value As Double)
            MyBase.Height = Value
        End Set
    End Property

    Private Movement As Vector

    Public ReadOnly From As Player

    Sub New(Position As Point, Direction As Vector, Owner As Player)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        X = Position.X
        Y = Position.Y
        Width = DefaultSize
        Height = DefaultSize

        Movement = Direction
        From = Owner
        From.Shoots.Add(Me)

        Me.Background.Source = New BitmapImage(New Uri(Cursors.Next(), UriKind.Absolute))

        AddHandler G.SlowClock.Elapsed, AddressOf SlowClock_Elapsed
        AddHandler G.FastClock.Elapsed, AddressOf FastClock_Elapsed
    End Sub

    Private Sub FastClock_Elapsed(sender As Object, e As EventArgs)
        Application.Current.Dispatcher.BeginInvoke(Sub()
                                                       X += Movement.X
                                                       Y += Movement.Y
                                                   End Sub, Threading.DispatcherPriority.Render)
    End Sub
    Private Sub SlowClock_Elapsed(sender As Object, e As EventArgs)
        Application.Current.Dispatcher.BeginInvoke(Sub()
                                                       If X < 0 OrElse X > G.ScreenWidth OrElse Y < 0 OrElse Y > G.ScreenHeight Then Me.Close()
                                                   End Sub, Threading.DispatcherPriority.Background)
    End Sub

    Shadows Sub Close()
        MyBase.Close()
        From.Shoots.Remove(Me)
    End Sub

End Class
