using System;
using Google.Apis.Auth.OAuth2;
using System.Threading.Tasks;
using Google.Apis.Services;
using Newtonsoft.Json;

namespace test
{
	class MainClass
	{

		public static async Task TestOne()
		{
			GoogleCredential credential = await GoogleCredential.GetApplicationDefaultAsync();
			var basClientService = new BaseClientService.Initializer()
				{
					HttpClientInitializer = credential
				};
			Console.WriteLine (JsonConvert.SerializeObject (basClientService));
		}
		public  static void Main (string[] args)
		{
			Environment.SetEnvironmentVariable ("GOOGLE_APPLICATION_CREDENTIALS", "google-secret.json");

			Task.Run (async () => await TestOne ()).Wait();
			Console.WriteLine ("Hello World!");

		}
	}
}
