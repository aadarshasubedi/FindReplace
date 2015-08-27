Imports System
Imports System.IO
Imports System.Collections
Imports Microsoft.Office.Interop

Public Class Form1

    'When user clicks the finad and replace button'
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim subDirectories As String()
        Dim outputDirectory As String = TextBox1.Text

        'Should sub directories also be checked?
        If CheckBox1.Checked Then
            subDirectories = Directory.GetDirectories(outputDirectory)

            'Checking in files in given root directory'
            FindAndReplace(outputDirectory)

            'Checking in files in sub directories directory'
            For Each subDirectory As String In subDirectories
                FindAndReplace(subDirectory)
            Next

        Else
            'Checking in files in given root directory only'
            FindAndReplace(outputDirectory)

        End If

        MessageBox.Show("Replacement Complete")
    End Sub

    'Browse to required Directory and shoe path in text box
    Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click
        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            TextBox1.Text = FolderBrowserDialog1.SelectedPath
        End If
    End Sub

    'Does find and replace in all files in the given directory'
    Private Sub FindAndReplace(ByVal targetDirectory As String)
        Dim objWord As Object
        Dim objDoc As Object
        Dim wasReadOnly As Boolean = False

        objWord = CreateObject("Word.Application")
        objDoc = objWord.Documents.Add

        'Get all Word files in the directory
        Dim filesInDirectory As String() = Directory.GetFiles(targetDirectory, "*.docx")

        'Analyze each file in the directory
        For Each file As String In filesInDirectory
            Dim fileInfo As FileInfo = New FileInfo(file)

            'Make a "read only" file editable
            If fileInfo.IsReadOnly Then
                wasReadOnly = True
                fileInfo.IsReadOnly = False
            End If

            'Opening the Word file
            objDoc = objWord.Documents.Open(file)

            'Searching for text
            With objWord.Selection.Find
                .Text = TextBox2.Text
                .Replacement.Text = TextBox3.Text
                .Forward = True
                .Wrap = Microsoft.Office.Interop.Word.WdFindWrap.wdFindContinue
                .Format = False
                .MatchCase = False
                .MatchWholeWord = True
                .MatchWildcards = False
                .MatchSoundsLike = False
                .MatchAllWordForms = False
            End With

            'Replacing found text
            objWord.Selection.Find.Execute(Replace:=Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll)

            'Save and Close Document
            objDoc.SaveAs(file)
            objDoc.Close()

            'If the file was a read only file then make it read only again
            If wasReadOnly Then
                fileInfo.IsReadOnly = True
                wasReadOnly = False
            End If
        Next

        objWord.Quit()
        objDoc = Nothing
        objWord = Nothing
    End Sub

End Class
