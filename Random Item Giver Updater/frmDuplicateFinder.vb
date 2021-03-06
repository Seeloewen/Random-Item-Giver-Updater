Imports System.IO

Public Class frmDuplicateFinder

    Dim DatapackPath As String = "NONE"
    Dim PathAmount As String = "NONE"
    Dim DatapackVersion As String = "NONE"
    Dim QuM As String
    Dim WithDuplicates As String()
    Dim WithDuplicatesArray As String()
    Dim DuplicatesOnly As String()
    Dim DuplicateFinderResult As String
    Dim DuplicateFinderProgress As Double
    Dim BackGroundWorkerProgress As Double

    Private Sub btnCheck_Click(sender As Object, e As EventArgs) Handles btnCheck.Click
        If String.IsNullOrEmpty(tbDatapackPath.Text) = False Then
            If My.Computer.FileSystem.DirectoryExists(tbDatapackPath.Text) Then
                BackGroundWorkerProgress = 0
                DuplicateFinderProgress = 0
                DuplicateFinderResult = "success"
                btnCheck.Enabled = False
                btnBrowse.Enabled = False
                tbDatapackPath.Enabled = False
                lblDuplicatesAmount.Hide()
                lblChecking.Show()
                pbProgress.Show()
                pbProgress.Value = 0
                btnCheck.Text = "Checking..."
                MsgBox("Duplicate finder will now search for duplicates in the selected datapack. This may take a while.", MsgBoxStyle.Information, "Duplicate Finder")
                frmMain.WriteToLog("-- Checking for duplicates --", "Info")
                lvDuplicates.Clear()
                lvDuplicates.Columns.Add("Item")
                lvDuplicates.Columns(0).Width = 256
                lvDuplicates.Columns.Add("Loot Table")
                lvDuplicates.Columns(1).Width = 266
                bgwSearchForDuplicates.RunWorkerAsync()
            Else
                MsgBox("The datapack path you have entered is not valid.", MsgBoxStyle.Critical, "Error")
            End If
        Else
            MsgBox("Please enter a datapack path!", MsgBoxStyle.Critical, "Error")
        End If
    End Sub


    Private Sub CheckLootTable(ItemAmount As Integer, LootTable As String)
        'Setup variables
        If DatapackVersion = "1.17" Then
            PathAmount = ""
        Else
            If ItemAmount = 1 Then
                PathAmount = "1item/"
            ElseIf ItemAmount = 2 Then
                PathAmount = "2sameitems/"
            ElseIf ItemAmount = 3 Then
                PathAmount = "3sameitems/"
            ElseIf ItemAmount = 5 Then
                PathAmount = "5sameitems/"
            ElseIf ItemAmount = 10 Then
                PathAmount = "10sameitems/"
            ElseIf ItemAmount = 32 Then
                PathAmount = "32sameitems/"
            ElseIf ItemAmount = 64 Then
                PathAmount = "64sameitems/"
            End If
        End If

        'Load text from file into Array
        WithDuplicates = File.ReadAllLines(DatapackPath + PathAmount + LootTable + ".json")

        'Remove unnessecary characters from WithDuplicates
        For x As Integer = 0 To WithDuplicates.Length - 1
            If WithDuplicates(x).Contains("" + QuM + "type" + QuM + ": " + QuM + "minecraft:item" + QuM + ",") Then
                WithDuplicates(x) = WithDuplicates(x).Replace("" + QuM + "type" + QuM + ": " + QuM + "minecraft:item" + QuM + ",", "")
            End If
            If WithDuplicates(x).Contains("" + QuM + "entries" + QuM + ": [") Then
                WithDuplicates(x) = WithDuplicates(x).Replace("" + QuM + "entries" + QuM + ": [", "")
            End If
            If WithDuplicates(x).Contains("" + QuM + "rolls" + QuM + ": 1,") Then
                WithDuplicates(x) = WithDuplicates(x).Replace("" + QuM + "rolls" + QuM + ": 1,", "")
            End If
            If WithDuplicates(x).Contains("" + QuM + "pools" + QuM + ": [") Then
                WithDuplicates(x) = WithDuplicates(x).Replace("" + QuM + "pools" + QuM + ": [", "")
            End If
            If WithDuplicates(x).Contains("" + QuM + "name" + QuM + ": ") Then
                WithDuplicates(x) = WithDuplicates(x).Replace("" + QuM + "name" + QuM + ": ", "")
            End If
            If WithDuplicates(x).Contains("" + QuM + "functions" + QuM + ": [") Then
                WithDuplicates(x) = WithDuplicates(x).Replace("" + QuM + "functions" + QuM + ": [", "")
            End If
            If WithDuplicates(x).Contains("" + QuM + "function" + QuM + ": " + QuM + "minecraft:set_count" + QuM + ",") Then
                WithDuplicates(x) = WithDuplicates(x).Replace("" + QuM + "function" + QuM + ": " + QuM + "minecraft:set_count" + QuM + ",", "")
            End If
            If WithDuplicates(x).Contains("" + QuM + "count" + QuM + ": 10") Then
                WithDuplicates(x) = WithDuplicates(x).Replace("" + QuM + "count" + QuM + ": 10", "")
            End If
            If WithDuplicates(x).Contains("" + QuM + "count" + QuM + ": 32") Then
                WithDuplicates(x) = WithDuplicates(x).Replace("" + QuM + "count" + QuM + ": 32", "")
            End If
            If WithDuplicates(x).Contains("" + QuM + "count" + QuM + ": 64") Then
                WithDuplicates(x) = WithDuplicates(x).Replace("" + QuM + "count" + QuM + ": 64", "")
            End If
            If WithDuplicates(x).Contains("" + QuM + "count" + QuM + ": 2") Then
                WithDuplicates(x) = WithDuplicates(x).Replace("" + QuM + "count" + QuM + ": 2", "")
            End If
            If WithDuplicates(x).Contains("" + QuM + "count" + QuM + ": 3") Then
                WithDuplicates(x) = WithDuplicates(x).Replace("" + QuM + "count" + QuM + ": 3", "")
            End If
            If WithDuplicates(x).Contains("" + QuM + "count" + QuM + ": 5") Then
                WithDuplicates(x) = WithDuplicates(x).Replace("" + QuM + "count" + QuM + ": 5", "")
            End If
            If WithDuplicates(x).Contains("minecraft:") Then
                WithDuplicates(x) = WithDuplicates(x).Replace("minecraft:", "")
            End If
            If WithDuplicates(x).Contains(" ") Then
                WithDuplicates(x) = WithDuplicates(x).Replace(" ", "")
            End If
            If WithDuplicates(x).Contains("{") Then
                WithDuplicates(x) = WithDuplicates(x).Replace("{", "")
            End If
            If WithDuplicates(x).Contains("}") Then
                WithDuplicates(x) = WithDuplicates(x).Replace("}", "")
            End If
            If WithDuplicates(x).Contains(",") Then
                WithDuplicates(x) = WithDuplicates(x).Replace(",", "")
            End If
            If WithDuplicates(x).Contains("]") Then
                WithDuplicates(x) = WithDuplicates(x).Replace("]", "")
            End If
        Next

        'Remove empty lines from WithDuplicates
        Dim WithoutEmptyLines As New List(Of String)
        For Each line As String In WithDuplicates
            If Not line.Trim.Equals(String.Empty) Then
                WithoutEmptyLines.Add(line)
            End If
        Next
        WithDuplicates = WithoutEmptyLines.ToArray

        'Find duplicate lines and put them into a list
        Dim duplicates As List(Of String) =
                   WithDuplicates.GroupBy(Function(n) n) _
                   .Where(Function(g) g.Count() > 1) _
                   .Select(Function(g) g.First) _
                   .ToList()

        'Convert duplicate list to array and load into richtextbox
        DuplicatesOnly = duplicates.ToArray

        'Remove quotationmarks from item names in the duplicates richtextbox
        For x As Integer = 0 To DuplicatesOnly.Length - 1
            If DuplicatesOnly(x).Contains(QuM) Then
                DuplicatesOnly(x) = DuplicatesOnly(x).Replace(QuM, "")
            End If
        Next

        'Log duplicates into listview
        Dim NumLinesOnlyDups As Integer = DuplicatesOnly.Length
        Dim DoLoopNum As Integer
        Dim str(1) As String
        Dim itm As ListViewItem

        While DoLoopNum < NumLinesOnlyDups
            str(0) = DuplicatesOnly(DoLoopNum)
            str(1) = PathAmount + LootTable
            itm = New ListViewItem(str)
            Invoke(Sub() lvDuplicates.Items.Add(itm))
            DoLoopNum = DoLoopNum + 1
        End While

        BackGroundWorkerProgress = BackGroundWorkerProgress + DuplicateFinderProgress
        bgwSearchForDuplicates.ReportProgress(BackGroundWorkerProgress)
        frmMain.WriteToLog("Completed checking " + LootTable, "Info")
    End Sub

    Private Sub btnBrowse_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
        fbdMainFolderPath.ShowDialog()
        tbDatapackPath.Text = fbdMainFolderPath.SelectedPath
    End Sub

    Private Sub tbDatapackPath_TextChanged(sender As Object, e As EventArgs) Handles tbDatapackPath.TextChanged
        Try
            DatapackPath = tbDatapackPath.Text + "/data/randomitemgiver/loot_tables/"

            Dim VersionString As String = System.IO.File.ReadAllLines(tbDatapackPath.Text + "/pack.mcmeta")(2)
            Dim ParseVersion As String = Replace(VersionString, "    " + QuM + "pack_format" + QuM + ": ", "")
            Dim Version As String = Replace(ParseVersion, ",", "")

            If Version = "10" Then
                DatapackVersion = "1.19"
            ElseIf Version = "9" Then
                DatapackVersion = "1.18"
            ElseIf Version = "8" Then
                DatapackVersion = "1.18"
            ElseIf Version = "7" Then
                DatapackVersion = "1.17"
            ElseIf Version = "6" Then
                DatapackVersion = "1.16"
            Else
                DatapackVersion = "Null"
            End If

            frmMain.WriteToLog("Detected datapack as version " + DatapackVersion + " in duplicate finder", "Info")
        Catch ex As Exception
            MsgBox("Error: " + ex.Message + vbNewLine + vbNewLine + "The datapack path is not detected as valid and therefor the duplicate finder might fail.", MsgBoxStyle.Critical, "Error")
            frmMain.WriteToLog("Selected datapack path in duplicate finder is invalid.", "Error")
        End Try

    End Sub

    Private Sub bgwSearchForDuplicates_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles bgwSearchForDuplicates.DoWork
        If DatapackVersion = "1.19" Or DatapackVersion = "1.18" Or DatapackVersion = "1.16" Then
            DuplicateFinderProgress = 1.78
            Try
                '1 item
                CheckLootTable(1, "main")
                CheckLootTable(1, "main_without_creative-only")
                CheckLootTable(1, "special_vvx")
                CheckLootTable(1, "special_vxv")
                CheckLootTable(1, "special_vxx")
                CheckLootTable(1, "special_xvv")
                CheckLootTable(1, "special_xvx")
                CheckLootTable(1, "special_xxv")

                '2 items
                CheckLootTable(2, "main")
                CheckLootTable(2, "main_without_creative-only")
                CheckLootTable(2, "special_vvx")
                CheckLootTable(2, "special_vxv")
                CheckLootTable(2, "special_vxx")
                CheckLootTable(2, "special_xvv")
                CheckLootTable(2, "special_xvx")
                CheckLootTable(2, "special_xxv")

                '3 items
                CheckLootTable(3, "main")
                CheckLootTable(3, "main_without_creative-only")
                CheckLootTable(3, "special_vvx")
                CheckLootTable(3, "special_vxv")
                CheckLootTable(3, "special_vxx")
                CheckLootTable(3, "special_xvv")
                CheckLootTable(3, "special_xvx")
                CheckLootTable(3, "special_xxv")

                '5 items
                CheckLootTable(5, "main")
                CheckLootTable(5, "main_without_creative-only")
                CheckLootTable(5, "special_vvx")
                CheckLootTable(5, "special_vxv")
                CheckLootTable(5, "special_vxx")
                CheckLootTable(5, "special_xvv")
                CheckLootTable(5, "special_xvx")
                CheckLootTable(5, "special_xxv")

                '10 items
                CheckLootTable(10, "main")
                CheckLootTable(10, "main_without_creative-only")
                CheckLootTable(10, "special_vvx")
                CheckLootTable(10, "special_vxv")
                CheckLootTable(10, "special_vxx")
                CheckLootTable(10, "special_xvv")
                CheckLootTable(10, "special_xvx")
                CheckLootTable(10, "special_xxv")

                '32 items
                CheckLootTable(32, "main")
                CheckLootTable(32, "main_without_creative-only")
                CheckLootTable(32, "special_vvx")
                CheckLootTable(32, "special_vxv")
                CheckLootTable(32, "special_vxx")
                CheckLootTable(32, "special_xvv")
                CheckLootTable(32, "special_xvx")
                CheckLootTable(32, "special_xxv")

                '64 items
                CheckLootTable(64, "main")
                CheckLootTable(64, "main_without_creative-only")
                CheckLootTable(64, "special_vvx")
                CheckLootTable(64, "special_vxv")
                CheckLootTable(64, "special_vxx")
                CheckLootTable(64, "special_xvv")
                CheckLootTable(64, "special_xvx")
                CheckLootTable(64, "special_xxv")
            Catch ex As Exception
                MsgBox("Error: " + ex.Message, MsgBoxStyle.Critical, "Error")
                DuplicateFinderResult = "failed"
            End Try
        ElseIf DatapackVersion = "1.17" Then
            DuplicateFinderProgress = 12.5
            Try
                CheckLootTable(1, "main")
                CheckLootTable(1, "main_without_creative-only")
                CheckLootTable(1, "special_vvx")
                CheckLootTable(1, "special_vxv")
                CheckLootTable(1, "special_vxx")
                CheckLootTable(1, "special_xvv")
                CheckLootTable(1, "special_xvx")
                CheckLootTable(1, "special_xxv")
            Catch ex As Exception
                MsgBox("Error: " + ex.Message, MsgBoxStyle.Critical, "Error")
                DuplicateFinderResult = "failed"
            End Try
        ElseIf DatapackVersion = "None" Then
        Else
            DuplicateFinderResult = "failed"
            MsgBox("An unknown error occured." + vbNewLine + "Cannot search for duplicates.", MsgBoxStyle.Critical, "Error")
        End If
    End Sub

    Private Sub frmDuplicateFinder_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        QuM = Quotationmark.Text
    End Sub

    Private Sub bgwSearchForDuplicates_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bgwSearchForDuplicates.RunWorkerCompleted
        If DuplicateFinderResult = "success" Then
            lblDuplicatesAmount.Text = "Found " + lvDuplicates.Items.Count.ToString + " duplicates totally."
            frmMain.WriteToLog("Checking for duplicates completed. Found " + lvDuplicates.Items.Count.ToString + " duplicates totally.", "Info")
            MsgBox("Checking for duplicates is complete." + vbNewLine + "You can see the results in the list behind this message." + vbNewLine + "If the list is empty then there aren't any duplicates.", MsgBoxStyle.Information, "Duplicate checker")
        Else
            lblDuplicatesAmount.Text = "Searching for duplicates failed."
            frmMain.WriteToLog("Searching for duplicates failed with 1 or more errors.", "Error")
        End If
        lblDuplicatesAmount.Show()
        btnCheck.Enabled = True
        btnBrowse.Enabled = True
        tbDatapackPath.Enabled = True
        lblChecking.Hide()
        pbProgress.Value = 100
        pbProgress.Hide()
        btnCheck.Text = "Check"
    End Sub

    Private Sub bgwSearchForDuplicates_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles bgwSearchForDuplicates.ProgressChanged
        pbProgress.Value = e.ProgressPercentage
    End Sub
End Class