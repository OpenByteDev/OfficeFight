Option Strict On
Option Explicit Off

Imports System.Collections.ObjectModel

Class Control

    Private Players(1) As Player

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Try
            Me.Width = ScreenWidth
            Me.Top = 0
            Me.Left = 0
            Me.TaskbarItemInfo = New Shell.TaskbarItemInfo()
            Me.TaskbarItemInfo.Description = "OfficeFight"

            Cursors.Initialize()

            InitPlayers()

            ArrangeWindows(New Window() {Players(0), Players(1), Me})

            G.SlowClock.Start()
            G.FastClock.Start()
            G.Control = Me

            AddHandler SlowClock.Elapsed, AddressOf SlowClock_Elapsed
        Catch e As Exception
            MsgBox("Could not start Application", MsgBoxStyle.Critical, "Error")
            Application.Current.Shutdown()
        End Try
    End Sub

    Private Sub InitPlayers()
        Players(0) = New Player()
        Players(1) = New Player()

        Players(0).Controls = New Dictionary(Of Player.MoveSet, Key) From {
                {Player.MoveSet.UP, DirectCast([Enum].Parse(GetType(Key), My.Settings.PLAYER_ONE_KEY_UP, True), Key)},
                {Player.MoveSet.DOWN, DirectCast([Enum].Parse(GetType(Key), My.Settings.PLAYER_ONE_KEY_DOWN, True), Key)},
                {Player.MoveSet.RIGHT, DirectCast([Enum].Parse(GetType(Key), My.Settings.PLAYER_ONE_KEY_RIGHT, True), Key)},
                {Player.MoveSet.LEFT, DirectCast([Enum].Parse(GetType(Key), My.Settings.PLAYER_ONE_KEY_LEFT, True), Key)},
                {Player.MoveSet.SHOOT, DirectCast([Enum].Parse(GetType(Key), My.Settings.PLAYER_ONE_KEY_SHOOT, True), Key)}}
        Players(1).Controls = New Dictionary(Of Player.MoveSet, Key) From {
                {Player.MoveSet.UP, DirectCast([Enum].Parse(GetType(Key), My.Settings.PLAYER_TWO_KEY_UP, True), Key)},
                {Player.MoveSet.DOWN, DirectCast([Enum].Parse(GetType(Key), My.Settings.PLAYER_TWO_KEY_DOWN, True), Key)},
                {Player.MoveSet.RIGHT, DirectCast([Enum].Parse(GetType(Key), My.Settings.PLAYER_TWO_KEY_RIGHT, True), Key)},
                {Player.MoveSet.LEFT, DirectCast([Enum].Parse(GetType(Key), My.Settings.PLAYER_TWO_KEY_LEFT, True), Key)},
                {Player.MoveSet.SHOOT, DirectCast([Enum].Parse(GetType(Key), My.Settings.PLAYER_TWO_KEY_SHOOT, True), Key)}}

        Players(0).Program = CType(RandomElement(New Collection(Of Object)(CType(Players(0).Resources.Keys, IList(Of Object)))), String)
        Do
            Players(1).Program = CType(RandomElement(New Collection(Of Object)(CType(Players(1).Resources.Keys, IList(Of Object)))), String)
        Loop While Players(0).Program.Equals(Players(1).Program)
        Players(0).Background = CType(Players(0).Resources.Item(Players(0).Program), ImageBrush)
        Players(1).Background = CType(Players(1).Resources.Item(Players(1).Program), ImageBrush)

        Players(0).Left = Players(0).Width / 2
        Players(0).Top = (ScreenHeight - Players(0).Height) / 2
        Players(1).Left = ScreenWidth - Players(1).Width * 1.5
        Players(1).Top = (ScreenHeight - Players(1).Height) / 2

        Players(0).Show()
        Players(1).Show()
    End Sub

    Private Function RandomElement(Elements As Collection(Of Object)) As Object
        Dim Random As New Random()
        Return Elements.Item(Random.Next(Elements.Count))
    End Function

    Private Sub SlowClock_Elapsed(sender As Object, e As EventArgs)
        Application.Current.Dispatcher.BeginInvoke(Sub()
                                                       CheckCollisions(0, 1)
                                                       CheckCollisions(1, 0)
                                                   End Sub, Threading.DispatcherPriority.Background)
    End Sub

    Private Sub CheckCollisions(Source As Integer, Target As Integer)
        Dim Rect As Rect = BoxToRectangle(Players(Target))
        For I = 0 To Players(Source).Shoots.Count - 1
            Dim Shoot As Shoot = Players(Source).Shoots.Item(I)
            If Rect.IntersectsWith(BoxToRectangle(Shoot)) Then
                Players(Source).Hit()
                Shoot.Close()
                Exit For
            End If
        Next
    End Sub

    Friend Sub UpdateStats()
        For I = 0 To Players.Length - 1
            Dim Stats As Grid = CType(FindName("Player" & IIf(I = 0, 2, 1).ToString() & "Stats"), Grid)
            Select Case Players(I).Health
                Case 5 : Stats.Background = CType(Me.Stats.Resources.Item("Firefox"), ImageBrush)
                    Exit Select
                Case 4 : Stats.Background = CType(Me.Stats.Resources.Item("Chrome"), ImageBrush)
                    Exit Select
                Case 3 : Stats.Background = CType(Me.Stats.Resources.Item("Opera"), ImageBrush)
                    Exit Select
                Case 2 : Stats.Background = CType(Me.Stats.Resources.Item("Safari"), ImageBrush)
                    Exit Select
                Case 1 : Stats.Background = CType(Me.Stats.Resources.Item("IE"), ImageBrush)
                    Exit Select
                Case 0
                    Players(0).Close()
                    Players(1).Close()
                    Me.Visibility = Visibility.Hidden
                    MessageBox.Show(Players(I).Program & " wins!", "Game Ended", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly)
                    Me.Close()
                    Application.Current.Shutdown()
                    Exit Select
            End Select
        Next
    End Sub

    Private Sub ArrangeWindows(Order As Window())
        For I As Integer = 1 To Order.Length - 1
            Order(I).Owner = Order(I - 1)
        Next
    End Sub

    Private Sub Shutdown() Handles Me.Unloaded, CloseBtn.MouseLeftButtonDown
        Try
            While (Application.Current.Windows.Count > 0)
                Application.Current.Windows.Item(0).Close()
            End While
            Application.Current.Dispatcher.InvokeShutdown()
        Finally
            Application.Current.Shutdown()
        End Try
    End Sub

End Class
