using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VideoAggregator{
	public static class GuideBoxAPIWrapper{
		private static string apiHome = "http://api-public.guidebox.com/v1.43/US/";
		private static string apiKey = "rK4vt5WeKQvXmZgr8I0v7jbOVGCt9wtm/";
		private static string apiBaseURL{ get{ return apiHome + apiKey; } }

		private static string SourceToString(Source source){
			string sourceStr = "";
			switch (source){
			case Source.All:
				sourceStr = "hulu_free,hulu_plus,hulu_with_showtime,amazon_prime_free,amazon_prime,amazon_buy,youtube,youtube_purchase";
				break;
			case Source.Hulu:
				sourceStr = "hulu_free,hulu_plus,hulu_with_showtime";
				break;
			case Source.Amazon:
				sourceStr = "amazon_prime_free,amazon_prime,amazon_buy";
				break;
			case Source.YouTube:
				sourceStr = "youtube,youtube_purchase";
				break;
			}
			return sourceStr;
		}

		private static string getAPIData(string url){
			string text = "";
			bool gotResult = false;
			int counter = 0;

			while (!gotResult){
				Console.WriteLine ("Web request " + counter.ToString());
				HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
				HttpWebResponse response = (HttpWebResponse) request.GetResponse();
				if (response.StatusCode == HttpStatusCode.OK && response.ContentLength > 0) {
					TextReader reader = new StreamReader (response.GetResponseStream ());
					text = reader.ReadToEnd ();
					reader.Close ();
					if (text != null)
						gotResult = true;
				}
				counter++;
				
				response.Close ();
				if (counter >= 10 && !gotResult)
					throw new WebException ("Server response timed out");
			}

			//Console.WriteLine (url);
			return text;
		}

		public static List<Show> getTVShowIds(int limit1, int limit2, Source source){
			string url = apiBaseURL + "shows/all/" + limit1.ToString() + "/" + limit2.ToString() + "/" + SourceToString(source) + "/web";

			JObject search = new JObject();

			string JsonData = getAPIData (url);
			search = JObject.Parse (JsonData);

			List<JToken> results = search["results"].Children().ToList();
			List<Show> shows = new List<Show>();

			foreach (var result in results){
				var definition = new {title = "", id = "", artwork_304x171 = ""};
				var show_json = JsonConvert.DeserializeAnonymousType(result.ToString(), definition);

				Show show = new Show (show_json.title, show_json.id); 
				show.thumbURL = show_json.artwork_304x171;
				shows.Add(show);
			}

			return shows;
		}

		public static List<Show> getTVShowIds(string title){
			//they want it triple url encoded
			string url = apiBaseURL + "search/title/" + WebUtility.UrlEncode(WebUtility.UrlEncode(WebUtility.UrlEncode(title))) + "/fuzzy";

			JObject search = new JObject();

			string JsonData = getAPIData (url);
			search = JObject.Parse (JsonData);

			List<JToken> results = search["results"].Children().ToList();
			List<Show> shows = new List<Show>();
			foreach (var result in results){
				var definition = new {title = "", id = "", artwork_304x171 = ""};
				var show_json = JsonConvert.DeserializeAnonymousType(result.ToString(), definition);
				Show show = new Show (show_json.title, show_json.id);
				show.thumbURL = show_json.artwork_304x171;
				shows.Add(show);
			}
			return shows;
		}

		public static int getTVShowSeasons(string id){
			string url = apiBaseURL + "show/" + id + "/seasons";

			JObject search = new JObject();

			string JsonData = getAPIData (url);
			search = JObject.Parse (JsonData);

			return (int)search ["total_results"];
		}

		public static List<Episode> getTVShowEpisodes(string id, string season){
			string url = apiBaseURL + "show/" + id + "/episodes/" + season + "/0/100/" + SourceToString(Source.All) + "/web/?reverse_ordering=true";
			JObject search = new JObject();

			string JsonData = getAPIData (url);
			search = JObject.Parse (JsonData);

			List<Episode> episodes = new List<Episode> ();
			List<JToken> results = search ["results"].Children ().ToList ();

			foreach (var result in results) {
				var definition = new {title = "", id = "", overview = "", episode_number = "", thumbnail_304x171 = ""};
				var episode_json = JsonConvert.DeserializeAnonymousType (result.ToString (), definition);
				Episode episode = new Episode (season, episode_json.episode_number, episode_json.title, episode_json.id);
				episode.desc = episode_json.overview;
				episode.thumbURL = episode_json.thumbnail_304x171;

				episodes.Add (episode);

			}
			return episodes;
		}

		public static Dictionary<string, List<string> > getEpisodeLinks(string id){
			string url = apiBaseURL + "episode/" + id;

			string JsonData = getAPIData (url);
			JObject json_ep = JObject.Parse(JsonData);

			List< List<JToken> > sources = new List< List<JToken> >();
			sources.Add (json_ep["free_web_sources"].Children().ToList());
			sources.Add (json_ep["tv_everywhere_web_sources"].Children().ToList());
			sources.Add (json_ep["subscription_web_sources"].Children().ToList());
			sources.Add (json_ep["purchase_web_sources"].Children().ToList());

			Dictionary<string, List<string> > sourceLinks = new Dictionary<string, List<string> >();

			foreach (var sourceList in sources){
				foreach (var source in sourceList){
					var definition = new {source = "", id = "", display_name = "", link = ""};
					var source_json = JsonConvert.DeserializeAnonymousType(source.ToString(), definition);
					string sourceName = source_json.display_name;

					if (sourceName == "Amazon Prime")
						sourceName = "Amazon";
					
					if (sourceName == "Hulu" || sourceName == "Amazon" || sourceName == "YouTube") {
						
						if (sourceLinks.ContainsKey (sourceName)) {
							sourceLinks [sourceName].Add (source_json.link);
						} else {
							sourceLinks.Add (sourceName, new List<string>{source_json.link});
						}
					}
				}
			}
			return sourceLinks;
		}

		public static List<Show> getMovieIds(int limit1, int limit2, Source source){
			string url = apiBaseURL + "movies/all/" + limit1.ToString() + "/" + limit2.ToString() + "/" + SourceToString(source) + "/web";

			JObject search = new JObject();

			string JsonData = getAPIData (url);
			search = JObject.Parse (JsonData);

			List<JToken> results = search["results"].Children().ToList();
			List<Show> shows = new List<Show>();
			foreach (var result in results){
				var definition = new {title = "", id = "", poster_240x342 = ""};
				var show_json = JsonConvert.DeserializeAnonymousType(result.ToString(), definition);
				Show show = new Show (show_json.title, show_json.id, true);
				show.thumbURL = show_json.poster_240x342;
				shows.Add(show);
			}
			return shows;
		}

		public static List<Show> getMovieIds(string title){
			string url = apiBaseURL + "search/movie/title/" + WebUtility.UrlEncode(WebUtility.UrlEncode(WebUtility.UrlEncode(title))) + "/fuzzy";
			JObject search = new JObject();

			string JsonData = getAPIData (url);
			search = JObject.Parse (JsonData);

			List<JToken> results = search["results"].Children().ToList();
			List<Show> shows = new List<Show>();
			foreach (var result in results){
				var definition = new {title = "", id = "", poster_240x342 = ""};
				var show_json = JsonConvert.DeserializeAnonymousType(result.ToString(), definition);
				Show show = new Show (show_json.title, show_json.id, true);
				show.thumbURL = show_json.poster_240x342;
				shows.Add(show);
			}
			return shows;
		}

		public static Dictionary<string, List<string> > getMovieLinks(string id){
			string url = apiBaseURL + "movie/" + id;

			string JsonData = getAPIData (url);
			JObject json_ep = JObject.Parse(JsonData);

			List< List<JToken> > sources = new List< List<JToken> >();
			sources.Add (json_ep["free_web_sources"].Children().ToList());
			sources.Add (json_ep["tv_everywhere_web_sources"].Children().ToList());
			sources.Add (json_ep["subscription_web_sources"].Children().ToList());
			sources.Add (json_ep["purchase_web_sources"].Children().ToList());

			Dictionary<string, List<string> > sourceLinks = new Dictionary<string, List<string> >();

			foreach (var sourceList in sources){
				foreach (var source in sourceList){
					var definition = new {source = "", id = "", display_name = "", link = ""};
					var source_json = JsonConvert.DeserializeAnonymousType(source.ToString(), definition);
					string sourceName = source_json.display_name;

					if (sourceName == "Amazon Prime")
						sourceName = "Amazon";

					if (sourceName == "Hulu" || sourceName == "Amazon" || sourceName == "YouTube") {

						if (sourceLinks.ContainsKey (sourceName)) {
							sourceLinks [sourceName].Add (source_json.link);
						} else {
							sourceLinks.Add (sourceName, new List<string>{source_json.link});
						}
					}
				}
			}
			return sourceLinks;
		}
	}
}