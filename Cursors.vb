Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.InteropServices

Class Cursors

    Private Shared WinPath As String = Environment.ExpandEnvironmentVariables("%WinDir%") & IO.Path.DirectorySeparatorChar & "Cursors"
    Private Shared Path As String = IO.Path.GetTempPath() & "DesktopGame" & IO.Path.ChangeExtension(IO.Path.GetRandomFileName(), "").TrimEnd(".")
    Private Shared Random As New Random()

    Friend Shared Function [Next]() As String
        Dim Files() As String = Directory.GetFiles(Path, "*.png")
        Return Files(Random.Next(0, Files.Length - 1))
    End Function

    Friend Shared Sub Initialize()
        RunCmdCommand("mkdir " & Path & " && xcopy " & WinPath & "\*.* " & Path & "\")

        Debug.WriteLine("INITIALIZE")
        Dim Files() As String = Directory.GetFiles(Path, "*.cur")
        For Each File As String In Files
            Debug.WriteLine(File)
            Dim Icon As Icon = Icon.ExtractAssociatedIcon(File)
            Dim Bmp As Bitmap = Icon.ToBitmap()
            Dim CroppedBmp As Bitmap = TrimBitmap(Bmp)
            CroppedBmp.Save(IO.Path.ChangeExtension(File, "png"), ImageFormat.Png)
        Next
    End Sub

    Private Shared Sub RunCmdCommand(Command As String, Optional RunAsAdministrator As Boolean = False)
        Dim Process As Process = New Process()
        Dim ProcInfo As ProcessStartInfo = New ProcessStartInfo()
        ProcInfo.WindowStyle = ProcessWindowStyle.Hidden
        ProcInfo.RedirectStandardInput = False
        ProcInfo.Arguments = "/C " & Command
        ProcInfo.FileName = "cmd.exe"
        If RunAsAdministrator Then ProcInfo.Verb = "runas"
        Process.StartInfo = ProcInfo
        Process.Start()

        Process.WaitForExit()
    End Sub

    Private Shared Function TrimBitmap(source As Bitmap) As Bitmap
        Dim srcRect As Rectangle = Nothing
        Dim data As BitmapData = Nothing
        Try
            data = source.LockBits(New Rectangle(0, 0, source.Width, source.Height), ImageLockMode.[ReadOnly], PixelFormat.Format32bppArgb)
            Dim buffer As Byte() = New Byte(data.Height * data.Stride - 1) {}
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length)

            Dim xMin As Integer = Integer.MaxValue, xMax As Integer = Integer.MinValue, yMin As Integer = Integer.MaxValue, yMax As Integer = Integer.MinValue

            Dim foundPixel As Boolean = False

            ' Find xMin
            For x As Integer = 0 To data.Width - 1
                Dim [stop] As Boolean = False
                For y As Integer = 0 To data.Height - 1
                    Dim alpha As Byte = buffer(y * data.Stride + 4 * x + 3)
                    If alpha <> 0 Then
                        xMin = x
                        [stop] = True
                        foundPixel = True
                        Exit For
                    End If
                Next
                If [stop] Then
                    Exit For
                End If
            Next

            ' Image is empty...
            If Not foundPixel Then
                Return Nothing
            End If

            ' Find yMin
            For y As Integer = 0 To data.Height - 1
                Dim [stop] As Boolean = False
                For x As Integer = xMin To data.Width - 1
                    Dim alpha As Byte = buffer(y * data.Stride + 4 * x + 3)
                    If alpha <> 0 Then
                        yMin = y
                        [stop] = True
                        Exit For
                    End If
                Next
                If [stop] Then
                    Exit For
                End If
            Next

            ' Find xMax
            For x As Integer = data.Width - 1 To xMin Step -1
                Dim [stop] As Boolean = False
                For y As Integer = yMin To data.Height - 1
                    Dim alpha As Byte = buffer(y * data.Stride + 4 * x + 3)
                    If alpha <> 0 Then
                        xMax = x
                        [stop] = True
                        Exit For
                    End If
                Next
                If [stop] Then
                    Exit For
                End If
            Next

            ' Find yMax
            For y As Integer = data.Height - 1 To yMin Step -1
                Dim [stop] As Boolean = False
                For x As Integer = xMin To xMax
                    Dim alpha As Byte = buffer(y * data.Stride + 4 * x + 3)
                    If alpha <> 0 Then
                        yMax = y
                        [stop] = True
                        Exit For
                    End If
                Next
                If [stop] Then
                    Exit For
                End If
            Next

            srcRect = Rectangle.FromLTRB(xMin, yMin, xMax + 1, yMax + 1)
        Finally
            If data IsNot Nothing Then
                source.UnlockBits(data)
            End If
        End Try

        Dim dest As New Bitmap(srcRect.Width, srcRect.Height)
        Dim destRect As New Rectangle(0, 0, srcRect.Width, srcRect.Height)
        Using graphics__1 As Graphics = Graphics.FromImage(dest)
            graphics__1.DrawImage(source, destRect, srcRect, GraphicsUnit.Pixel)
        End Using
        Return dest
    End Function

End Class
