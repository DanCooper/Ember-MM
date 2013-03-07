﻿' ################################################################################
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
Imports System.IO.Compression
Imports System.Text
Imports System.Text.RegularExpressions
Imports EmberAPI
Imports FanartTVAPI

Namespace FANARTTVs

	Public Class Scraper

#Region "Fields"

		Private _MySettings As EmberTMDBScraperModule.sMySettings
		Private _FanartTV As FanartTV.V1.FanartTV
		Friend WithEvents bwFANARTTV As New System.ComponentModel.BackgroundWorker
		Private _APIInvalid As Boolean = False
#End Region	'Fields

#Region "Events"

		Public Event PostersDownloaded(ByVal Posters As List(Of MediaContainers.Image))

		Public Event ProgressUpdated(ByVal iPercent As Integer)

#End Region	'Events

#Region "Methods"

		Public Sub New(ByRef tMySettings As EmberTMDBScraperModule.sMySettings)
			_MySettings = tMySettings
			_FanartTV = New FanartTV.V1.FanartTV(_MySettings.FANARTTVApiKey)
			Dim Result As FanartTV.V1.FanartTVMovie = _FanartTV.GetMovieInfo(New FanartTV.V1.FanartTVRequest("1", "JSON", "all", 1, 1))
			If IsNothing(Result) Then
				If Not IsNothing(_FanartTV.Error) Then
					Master.eLog.WriteToErrorLog(_FanartTV.Error, "", "Error")
				End If
				_APIInvalid = True
			End If
		End Sub

		Public Sub Cancel()
			If Me.bwFANARTTV.IsBusy Then Me.bwFANARTTV.CancelAsync()

			While Me.bwFANARTTV.IsBusy
				Application.DoEvents()
				Threading.Thread.Sleep(50)
			End While
		End Sub

		Public Sub GetImagesAsync(ByVal sURL As String)
			Try
				If Not bwFANARTTV.IsBusy Then
					bwFANARTTV.WorkerSupportsCancellation = True
					bwFANARTTV.WorkerReportsProgress = True
					bwFANARTTV.RunWorkerAsync(New Arguments With {.Parameter = sURL})
				End If
			Catch ex As Exception
				Master.eLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Error")
			End Try
		End Sub

		Public Function GetFANARTTVImages(ByVal imdbID As String) As List(Of MediaContainers.Image)
			Dim alPoster As New List(Of MediaContainers.Image)

			If _APIInvalid Then
				Return alPoster
			End If
			Try
				If bwFANARTTV.CancellationPending Then Return Nothing
				For Each mPoster As Match In mcPoster
					If bwFANARTTV.CancellationPending Then Return Nothing
					PosterURL = Strings.Replace(String.Format("{0}/{1}", sURL.Substring(0, sURL.LastIndexOf("/")), mPoster.Value.ToString()).Replace("thumbs", "posters"), "imp_", String.Empty)

					alPoster.Add(New MediaContainers.Image With {.Description = "poster", .URL = PosterURL})

					PosterURL = PosterURL.Insert(PosterURL.LastIndexOf("."), "_xlg")
					alPoster.Add(New MediaContainers.Image With {.Description = "original", .URL = PosterURL})
				Next
			Catch ex As Exception
				Master.eLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Error")
			End Try

			Return alPoster
		End Function

		Private Sub bwFANARTTVA_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bwFANARTTV.DoWork
			Dim Args As Arguments = DirectCast(e.Argument, Arguments)
			Try
				e.Result = GetIMPAPosters(Args.Parameter)
			Catch ex As Exception
				Master.eLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Error")
				e.Result = Nothing
			End Try
		End Sub

		Private Sub bwFANARTTV_ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles bwFANARTTV.ProgressChanged
			If Not bwFANARTTV.CancellationPending Then
				RaiseEvent ProgressUpdated(e.ProgressPercentage)
			End If
		End Sub

		Private Sub bwFANARTTV_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bwFANARTTV.RunWorkerCompleted
			If Not IsNothing(e.Result) Then
				RaiseEvent PostersDownloaded(DirectCast(e.Result, List(Of MediaContainers.Image)))
			End If
		End Sub

		Private Function GetLink(ByVal IMDBID As String) As String
			Try

				Dim sHTTP As New HTTP
				Dim HTML As String = sHTTP.DownloadData(String.Concat("http://www.imdb.com/title/tt", IMDBID, "/posters"))
				sHTTP = Nothing

				Dim mcIMPA As MatchCollection = Regex.Matches(HTML, "http://([^""]*)impawards.com/([^""]*)")
				If mcIMPA.Count > 0 Then
					'just use the first one if more are found
					Return mcIMPA(0).Value.ToString
				Else
					Return String.Empty
				End If
			Catch ex As Exception
				Master.eLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Error")
				Return String.Empty
			End Try
		End Function

#End Region	'Methods

#Region "Nested Types"

		Private Structure Arguments

#Region "Fields"

			Dim Parameter As String
			Dim sType As String

#End Region	'Fields

		End Structure

		Private Structure Results

#Region "Fields"

			Dim Result As Object
			Dim ResultList As List(Of MediaContainers.Image)

#End Region	'Fields

		End Structure

#End Region	'Nested Types

	End Class

End Namespace

