Option Strict On
Option Explicit Off

Imports System.Threading
Imports System.Windows.Threading

Public Class Player
    Implements Box

    Friend Const SPEED As Integer = 15
    Friend Shared SHOOT_INTERVAL As Integer

    Enum MoveSet
        UP
        DOWN
        RIGHT
        LEFT
        SHOOT
    End Enum

    Friend Controls As Dictionary(Of MoveSet, Key)
    Friend Shoots As New List(Of Shoot)
    Friend Health As SByte = 5

    Public Property X As Double Implements Box.X
        Get
            Return Me.Left
        End Get
        Set(Value As Double)
            If Value < 0 Then Value = 0
            If Value + Width > ScreenWidth Then Value = ScreenWidth - Width
            Me.Left = Value
        End Set
    End Property
    Public Property Y As Double Implements Box.Y
        Get
            Return Me.Top
        End Get
        Set(Value As Double)
            If Value < 0 Then Value = 0
            If Value + Height > ScreenHeight Then Value = ScreenHeight - Height
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

    Private Movement As New Vector()
    Private CanShoot As Boolean = True
    Friend Program As String

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        AddHandler G.FastClock.Elapsed, AddressOf FastClock_Elapsed
        AddHandler G.KeyboardInput, AddressOf Global_KeyDown
    End Sub

    Private Sub FastClock_Elapsed(sender As Object, e As EventArgs)
        Application.Current.Dispatcher.BeginInvoke(Sub()
                                                       Movement.X = 0
                                                       Movement.Y = 0
                                                       CheckKeyboard()
                                                       X += Movement.X
                                                       Y += Movement.Y
                                                   End Sub, DispatcherPriority.Send)
    End Sub

    Private Sub CheckKeyboard()
        If Keyboard.IsKeyDown(Controls.Item(MoveSet.UP)) Then Movement.Y = -SPEED
        If Keyboard.IsKeyDown(Controls.Item(MoveSet.DOWN)) Then Movement.Y = SPEED
        If Keyboard.IsKeyDown(Controls.Item(MoveSet.RIGHT)) Then Movement.X = SPEED
        If Keyboard.IsKeyDown(Controls.Item(MoveSet.LEFT)) Then Movement.X = -SPEED
    End Sub

    Private Sub Global_KeyDown(sender As Object, e As KeyEventArgs)
        If e.Key = Controls.Item(MoveSet.SHOOT) Then Me.DoShoot()
    End Sub

    Friend Sub DoShoot()
        If CanShoot = False OrElse (Movement.X = 0 AndAlso Movement.Y = 0) Then Return
        CanShoot = False
        DoIn(SHOOT_INTERVAL, Sub()
                                 CanShoot = True
                             End Sub, Me.Dispatcher)
        Dim Shoot As New Shoot(
                    New Point(
                        X + CDbl(IIf(Movement.X < 0, -(Shoot.DefaultSize + Width * Shoot.SpacerModifier), IIf(Movement.X > 0, Width * (1 + Shoot.SpacerModifier), (Width - Shoot.DefaultSize) / 2))),
                        Y + CDbl(IIf(Movement.Y < 0, -(Shoot.DefaultSize + Height * Shoot.SpacerModifier), IIf(Movement.Y > 0, Height * (1 + Shoot.SpacerModifier), (Height - Shoot.DefaultSize) / 2)))),
                    New Vector(
                        Shoot.SPEED * CInt(IIf(Movement.X < 0, -1, IIf(Movement.X > 0, 1, 0))),
                        Shoot.SPEED * CInt(IIf(Movement.Y < 0, -1, IIf(Movement.Y > 0, 1, 0)))),
                    Me)
        Shoot.Show()
        Me.Focus()
    End Sub

    Friend Sub Hit()
        Health -= CSByte(1)
        G.Control.UpdateStats()
    End Sub

    Private Sub DoIn(Time As Integer, Action As Action, Dispatcher As Dispatcher)
        Dim T As New Thread(New ThreadStart(Sub()
                                                Thread.Sleep(Time)
                                                Dispatcher.Invoke(Action)
                                            End Sub))
        T.Start()
    End Sub

    Shadows Sub Close()
        While Shoots.Count > 0
            Shoots.Item(0).Close()
        End While
        MyBase.Close()
    End Sub

End Class
