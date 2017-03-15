using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaylistReader;
using Google.Apis.YouTube.v3.Data;
using System.Collections.Generic;
using Google.Apis.YouTube.v3;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Tests
{
	[TestClass]
	public class UnitTest1
	{
		private YouTubeServiceClient serviceClient;
		List<YouTubePlayList> playLists = new List<YouTubePlayList>();
		List<YouTubeSong> playListSongs = new List<YouTubeSong>();

		[TestInitialize]
		public void Init()
		{
			serviceClient = YouTubeServiceClient.Instance;
		}

		[TestMethod]
		public void TestMethod1()
		{
			var service = serviceClient.GetYouTubeService("oduvanchik22@gmail.com").Result;

			var channelsListRequest = service.Channels.List("contentDetails");
			channelsListRequest.Mine = true;
			var playlists = service.Playlists.List("snippet");
			playlists.PageToken = "";
			playlists.MaxResults = 50;
			playlists.Mine = true;
			PlaylistListResponse presponse = playlists.Execute();
			foreach (var currentPlayList in presponse.Items)
			{
				//playLists.Add(new YouTubePlayList(currentPlayList.Snippet.Title, currentPlayList.Id));
				var nextPageToken = "";
				while (nextPageToken != null)
				{
					PlaylistItemsResource.ListRequest listRequest = service.PlaylistItems.List("contentDetails");
					listRequest.MaxResults = 50;
					listRequest.PlaylistId = currentPlayList.Id;
					listRequest.PageToken = nextPageToken;
					var response = listRequest.Execute();
					if (playListSongs == null)
					{
						playListSongs = new List<YouTubeSong>();
					}
					foreach (var playlistItem in response.Items)
					{
						VideosResource.ListRequest videoR = service.Videos.List("snippet,contentDetails,status");
						videoR.Id = playlistItem.ContentDetails.VideoId;
						var responseV = videoR.Execute();
						if (responseV.Items.Count > 0)
						{
							KeyValuePair<string, string> parsedSong = SongTitleParser.ParseTitle(responseV.Items[0].Snippet.Title);
							ulong? duration = new DurationParser().GetDuration(responseV.Items[0].ContentDetails.Duration);
							YouTubeSong currentSong = new YouTubeSong(parsedSong.Key, parsedSong.Value, responseV.Items[0].Snippet.Title, responseV.Items[0].Id, playlistItem.Id, duration);
							playListSongs.Add(currentSong);
							Debug.WriteLine(currentSong.Title);
						}
					}
					nextPageToken = response.NextPageToken;
				}
			}
		}


		[TestMethod]
		public void MyTestMethod()
		{
			object value = "02/22/2017";
			DateTime dateTime;
			bool i = DateTime.TryParseExact(value.ToString(), "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out dateTime);

		}
	}


	public class DurationParser
	{
		private readonly string durationRegexExpression = @"PT(?<minutes>[0-9]{0,})M(?<seconds>[0-9]{0,})S";
		public ulong? GetDuration(string durationStr)
		{
			ulong? durationResult = default(ulong?);
			Regex regexNamespaceInitializations = new Regex(durationRegexExpression, RegexOptions.None);
			Match m = regexNamespaceInitializations.Match(durationStr);
			if (m.Success)
			{
				string minutesStr = m.Groups["minutes"].Value;
				string secondsStr = m.Groups["seconds"].Value;
				int minutes = int.Parse(minutesStr);
				int seconds = int.Parse(secondsStr);
				TimeSpan duration = new TimeSpan(0, minutes, seconds);
				durationResult = (ulong)duration.Ticks;
			}
			return durationResult;
		}
	}

	public static class SongTitleParser
	{
		private static readonly string RegexSongPattern = @"\s*(?<Artist>[a-zA-Z1-9\s\w]{1,})-(?<Name>[a-zA-Z1-9\-\s\w""']{1,})";

		public static KeyValuePair<string, string> ParseTitle(string text)
		{
			Regex regexNamespaceInitializations = new Regex(RegexSongPattern, RegexOptions.None);

			Match m = regexNamespaceInitializations.Match(text);
			KeyValuePair<string, string> currentSong = default(KeyValuePair<string, string>);
			if (m.Success)
			{
				currentSong = new KeyValuePair<string, string>(m.Groups["Artist"].ToString(), m.Groups["Name"].ToString());
			}
			else
			{
				currentSong = new KeyValuePair<string, string>(text, string.Empty);
			}

			return currentSong;
		}
	}
}
