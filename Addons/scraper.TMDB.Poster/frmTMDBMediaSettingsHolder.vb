' ################################################################################
' #                             EMBER MEDIA MANAGER                              #
' ################################################################################
' ################################################################################
' # This file is part of Ember Media Manager.                                    #
' #                                                                              #
' # Ember Media Manager is free software: you can redistribute it and/or modify  #
' # it under the terms of the GNU General Public License as published by         #
' # the Free Software Foundation, either version 3 of the License, or            #
' # (at your option) any later version.                                          #
' #                                                                              #
' # Ember Media Manager is distributed in the hope that it will be useful,       #
' # but WITHOUT ANY WARRANTY; without even the implied warranty of               #
' # MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the                #
' # GNU General Public License for more details.                                 #
' #                                                                              #
' # You should have received a copy of the GNU General Public License            #
' # along with Ember Media Manager.  If not, see <http://www.gnu.org/licenses/>. #
' ################################################################################

Imports System.IO
Imports EmberAPI

Public Class frmTMDBMediaSettingsHolder

#Region "Events"

    Public Event ModuleSettingsChanged()

    Public Event SetupScraperChanged(ByVal state As Boolean, ByVal difforder As Integer)

    Public Event SetupNeedsRestart()

#End Region 'Events

#Region "Fields"

    Private Api1 As String

#End Region 'Fields

#Region "Methods"

	Public Sub New()
		InitializeComponent()
		Me.SetUp()
	End Sub

	Private Sub btnDown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDown.Click
        Dim order As Integer = ModulesManager.Instance.externalPosterScrapersModules.FirstOrDefault(Function(p) p.AssemblyName = TMDB_Poster._AssemblyName).ScraperOrder
        If order < ModulesManager.Instance.externalPosterScrapersModules.Count - 1 Then
            ModulesManager.Instance.externalPosterScrapersModules.FirstOrDefault(Function(p) p.ScraperOrder = order + 1).ScraperOrder = order
            ModulesManager.Instance.externalPosterScrapersModules.FirstOrDefault(Function(p) p.AssemblyName = TMDB_Poster._AssemblyName).ScraperOrder = order + 1
            RaiseEvent SetupScraperChanged(cbEnabled.Checked, 1)
            orderChanged()
        End If
    End Sub

    Private Sub btnUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUp.Click
        Dim order As Integer = ModulesManager.Instance.externalPosterScrapersModules.FirstOrDefault(Function(p) p.AssemblyName = TMDB_Poster._AssemblyName).ScraperOrder
        If order > 0 Then
            ModulesManager.Instance.externalPosterScrapersModules.FirstOrDefault(Function(p) p.ScraperOrder = order - 1).ScraperOrder = order
            ModulesManager.Instance.externalPosterScrapersModules.FirstOrDefault(Function(p) p.AssemblyName = TMDB_Poster._AssemblyName).ScraperOrder = order - 1
            RaiseEvent SetupScraperChanged(cbEnabled.Checked, -1)
            orderChanged()
        End If
    End Sub

    Private Sub cbEnabled_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbEnabled.CheckedChanged
        RaiseEvent SetupScraperChanged(cbEnabled.Checked, 0)
    End Sub

    Private Sub chkScrapePoster_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkScrapePoster.CheckedChanged
        RaiseEvent ModuleSettingsChanged()
    End Sub

    Sub orderChanged()
        Dim order As Integer = ModulesManager.Instance.externalPosterScrapersModules.FirstOrDefault(Function(p) p.AssemblyName = TMDB_Poster._AssemblyName).ScraperOrder
        btnDown.Enabled = (order < ModulesManager.Instance.externalPosterScrapersModules.Count - 1)
        btnUp.Enabled = (order > 1)
    End Sub

	Sub SetUp()
        Me.Label3.Text = Master.eLang.GetString(168, "Scrape Order", True)
		Me.cbEnabled.Text = Master.eLang.GetString(774, "Enabled", True)
		Me.chkScrapePoster.Text = Master.eLang.GetString(101, "Get Posters")
		Me.chkScrapeFanart.Text = Master.eLang.GetString(102, "Get Fanart")
		Me.Label1.Text = String.Format(Master.eLang.GetString(103, "These settings are specific to this module.{0}Please refer to the global settings for more options."), vbCrLf)
        Me.Label18.Text = Master.eLang.GetString(854, "TMDB API Key:", True)
        Me.chkFallBackEng.Text = Master.eLang.GetString(114, "Fall back on english")
        Me.Label3.Text = Master.eLang.GetString(115, "Preferred Language:")
    End Sub

    Private Sub txtTMDBApiKey_TextEnter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtTMDBApiKey.Enter
        Api1 = txtTMDBApiKey.Text
    End Sub

    Private Sub txtTMDBApiKey_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtTMDBApiKey.TextChanged
        RaiseEvent ModuleSettingsChanged()
    End Sub

    Private Sub txtTMDBApiKey_TextValidated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtTMDBApiKey.Validated
        If Not (Api1 = txtTMDBApiKey.Text) Then
            RaiseEvent SetupNeedsRestart()
        End If
    End Sub

    Private Sub txtTimeout_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        RaiseEvent ModuleSettingsChanged()
    End Sub

    Private Sub optFanartFolderExtraFanart_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        RaiseEvent ModuleSettingsChanged()
    End Sub

    Private Sub optFanartFolderExtraThumbs_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        RaiseEvent ModuleSettingsChanged()
    End Sub

    Private Sub cbManualETSize_SelectedIndexChanged(ByVal sender As System.Object, e As System.EventArgs)
        RaiseEvent ModuleSettingsChanged()
    End Sub

    Private Sub cbTrailerTMDBPref_SelectedIndexChanged(ByVal sender As System.Object, e As System.EventArgs)
        RaiseEvent ModuleSettingsChanged()
    End Sub

#End Region	'Methods

End Class