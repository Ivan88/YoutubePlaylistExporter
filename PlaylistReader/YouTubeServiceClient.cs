using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlaylistReader
{
    public class YouTubeServiceClient
    {
		private static YouTubeServiceClient instance;
		public static YouTubeServiceClient Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new YouTubeServiceClient();
				}
				return instance;
			}
		}

		public async Task<YouTubeService> GetYouTubeService(string userEmail)
		{
			var token = new TokenResponse(); 
			UserCredential credential;
			using (var stream = new FileStream("client_id.json", FileMode.Open, FileAccess.Read))
			{
				credential = new UserCredential(new GoogleAuthorizationCodeFlow(
					new GoogleAuthorizationCodeFlow.Initializer
					{
						ClientSecretsStream = stream
					}),
					userEmail,
					token);
			}
			var youtubeService = new YouTubeService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = this.GetType().ToString()
			});
			return youtubeService;
		}
	}
}
