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

		public static async Task TestOne(string userEmail)
		{
      
			GoogleCredential credential = await GoogleCredential.GetApplicationDefaultAsync();
			var baseClientService = new BaseClientService.Initializer()
				{
					HttpClientInitializer = credential
				};
			Console.WriteLine (JsonConvert.SerializeObject (baseClientService));
			
			if (credential.IsCreateScopedRequired)
			{
				credential = credential.CreateScoped(Scopes);
			}

            var compute = new ComputeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });

            Console.WriteLine ("##Key!!!:\n"+JsonConvert.SerializeObject (credential));
			var service = new CalendarService(new BaseClientService.Initializer()
				{
					HttpClientInitializer = credential,
					ApplicationName = "Yavsc"
				});
			// Define parameters of request.
			String pageToken = null;

			CalendarListResource.ListRequest calListReq = service.CalendarList.List ();
			do {
				calListReq.PageToken = pageToken;
				var list = calListReq.Execute ();
				foreach (var calEntry in list.Items) 
				{
					Console.WriteLine($"{calEntry.Id} : {calEntry.Summary}");
				}

				pageToken = list.NextPageToken;
			} while (pageToken != null);

			EventsResource.ListRequest request = service.Events.List(userEmail);
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
            var startDate = DateTime.Now.AddMinutes(5);
            var endDate = startDate.AddMinutes(15);

            Event ev = new Event {
                   Start = new EventDateTime() { DateTime = startDate },
                   End = new EventDateTime() { DateTime = endDate },
                   Summary = "Insert test",
                   Description = "Insert descrptX test"
            };
            var queryInsert = service.Events.Insert(ev, userEmail);
            var resultingEv = await queryInsert.ExecuteAsync();
        }
		public  static void Main (string[] args)
		{
			Environment.SetEnvironmentVariable ("GOOGLE_APPLICATION_CREDENTIALS", "google-secret.json");
            Console.WriteLine ("Hello World!\nPlease enter your calendar id:\n> ");
            string userEmail = Console.ReadLine(); 
			Task.Run (async () => await TestOne (userEmail)).Wait();

		}
	}
}
