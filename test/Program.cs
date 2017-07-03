using System;
using Google.Apis.Auth.OAuth2;
using System.Threading.Tasks;
using Google.Apis.Services;
using Newtonsoft.Json;
using Google.Apis.Compute.v1;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

namespace test
{
	class MainClass
	{

		static string[] Scopes = { CalendarService.Scope.CalendarReadonly };

		public static async Task TestOne()
		{
			GoogleCredential credential = await GoogleCredential.GetApplicationDefaultAsync();
			var baseClientService = new BaseClientService.Initializer()
				{
					HttpClientInitializer = credential
				};
			Console.WriteLine (JsonConvert.SerializeObject (baseClientService));
			var compute = new ComputeService(new BaseClientService.Initializer()
				{
					HttpClientInitializer = credential
				});
			if (credential.IsCreateScopedRequired)
			{
				credential = credential.CreateScoped(Scopes);
			}
			Console.WriteLine ("##Key!!!:\n"+JsonConvert.SerializeObject (credential));
			var service = new CalendarService(new BaseClientService.Initializer()
				{
					HttpClientInitializer = credential,
					ApplicationName = "Yavsc"
				});
			// Define parameters of request.
			EventsResource.ListRequest request = service.Events.List("redienhcs.luap@gmail.com");
			request.TimeMin = DateTime.Now;
			request.ShowDeleted = false;
			request.SingleEvents = true;
			request.MaxResults = 10;
			request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
			// List events.
			Events events = request.Execute();
			Console.WriteLine("Upcoming events:");
			if (events.Items != null && events.Items.Count > 0)
			{
				foreach (var eventItem in events.Items)
				{
					string when = eventItem.Start.DateTime.ToString();
					if (String.IsNullOrEmpty(when))
					{
						when = eventItem.Start.Date;
					}
					Console.WriteLine("{0} ({1})", eventItem.Summary, when);
				}
			}
			else
			{
				Console.WriteLine("No upcoming events found.");
			}
			Console.Read();

			
		}
		public  static void Main (string[] args)
		{
			Environment.SetEnvironmentVariable ("GOOGLE_APPLICATION_CREDENTIALS", "google-secret.json");

			Task.Run (async () => await TestOne ()).Wait();
			Console.WriteLine ("Hello World!");

		}
	}
}
