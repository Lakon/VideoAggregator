/* GuideBoxAPIWrapper.cs
 * Static wrapper class for the GuideBox API
 * Each static function creates a url, downloads the JSON object at the url, parses the 
 * JSON object into Show/Season/Episode/source and returns it
*/

using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VideoAggregator{
	public static class GuideBoxAPIWrapper{
		private static string apiHome = "http://api-public.guidebox.com/v1.43/US/";		//GuideBox API url
		private static string apiKey = "rK4vt5WeKQvXmZgr8I0v7jbOVGCt9wtm/";				//Developer key
		private static string apiBaseURL{ get{ return apiHome + apiKey; } }

		//convert the enum Source into a string that guidebox api uses
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

		//Make a web request to the url and return a string of the result
		//Tries 10 times before throwing an exception
		private static string getAPIData(string url){
			string text = "";
			bool gotResult = false;
			int counter = 0;

			while (!gotResult){
				System.Diagnostics.Debug.WriteLine ("Web request " + counter.ToString());
				HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
				HttpWebResponse response = (HttpWebResponse) request.GetResponse();
				if (response.StatusCode == HttpStatusCode.OK && response.ContentLength > 0) {	//if we got something back
					TextReader reader = new StreamReader (response.GetResponseStream ());

					//read response
					text = reader.ReadToEnd ();
					reader.Close ();
					if (text != null)
						gotResult = true;
				}
				counter++;
				
				response.Close ();
				if (counter >= 10 && !gotResult)	//only 10 times
					throw new WebException ("Server response timed out");
			}

			//System.Diagnostics.Debug.WriteLine (url);
			return text;
		}

		//Get most popular tv shows
		//limit1 is where to start in list of results
		//limit2 is how many results to return from the api
		public static List<Show> getTVShowIds(int limit1, int limit2, Source source){
			//url is base/shows/all/{where to start}/{how many}/{source}/web
			string url = apiBaseURL + "shows/all/" + limit1.ToString() + "/" + limit2.ToString() + "/" + SourceToString(source) + "/web";

			JObject search = new JObject();

			//get JSON data
			string JsonData = getAPIData (url);
			search = JObject.Parse (JsonData);

			List<JToken> results = search["results"].Children().ToList();
			List<Show> shows = new List<Show>();

			foreach (var result in results){
				var definition = new {title = "", id = "", artwork_304x171 = ""};					//anonymous object definition
				var show_json = JsonConvert.DeserializeAnonymousType(result.ToString(), definition);

				//create show
				Show show = new Show (show_json.title, show_json.id); 
				show.thumbURL = show_json.artwork_304x171;
				shows.Add(show);
			}

			return shows;
		}

		//get tv shows from a query
		public static List<Show> getTVShowIds(string title){
			//they want the title triple url encoded
			//url is base/search/title/{url encoded query}/fuzzy
			string url = apiBaseURL + "search/title/" + WebUtility.UrlEncode(WebUtility.UrlEncode(WebUtility.UrlEncode(title))) + "/fuzzy";

			JObject search = new JObject();

			//get JSON data
			string JsonData = getAPIData (url);
			search = JObject.Parse (JsonData);

			List<JToken> results = search["results"].Children().ToList();
			List<Show> shows = new List<Show>();
			foreach (var result in results){
				var definition = new {title = "", id = "", artwork_304x171 = ""};					//anonymouse object definition
				var show_json = JsonConvert.DeserializeAnonymousType(result.ToString(), definition);

				//create show
				Show show = new Show (show_json.title, show_json.id);
				show.thumbURL = show_json.artwork_304x171;
				shows.Add(show);
			}
			return shows;
		}

		//get seasons from a tv show id
		//returns number of seasons
		public static int getTVShowSeasons(string id){
			//url is base/show/{show id}/seasons
			string url = apiBaseURL + "show/" + id + "/seasons";

			JObject search = new JObject();

			//get JSON data
			string JsonData = getAPIData (url);
			search = JObject.Parse (JsonData);

			return (int)search ["total_results"]; //just return number of seasons
		}

		//get the episodes from a show
		//Parameters: show id and season number
		public static List<Episode> getTVShowEpisodes(string id, string season){
			//url is base/show/{show id}/episodes/{season number}/(where to start {0})/(how many {100})/{source}/web/?reverse_ordering=true(most recent last)
			string url = apiBaseURL + "show/" + id + "/episodes/" + season + "/0/100/" + SourceToString(Source.All) + "/web/?reverse_ordering=true";
			JObject search = new JObject();

			//get JSON data
			string JsonData = getAPIData (url);
			search = JObject.Parse (JsonData);

			List<Episode> episodes = new List<Episode> ();
			List<JToken> results = search ["results"].Children ().ToList ();

			foreach (var result in results) {
				var definition = new {title = "", id = "", overview = "", episode_number = "", thumbnail_304x171 = ""};		//anonymous object definition
				var episode_json = JsonConvert.DeserializeAnonymousType (result.ToString (), definition);

				//create episode
				Episode episode = new Episode (season, episode_json.episode_number, episode_json.title, episode_json.id);
				episode.desc = episode_json.overview;
				episode.thumbURL = episode_json.thumbnail_304x171;

				episodes.Add (episode);

			}
			return episodes;
		}

		//get sources from an episode id
		public static Dictionary<string, List<string> > getEpisodeLinks(string id){
			//url is base/episode/{episode id}
			string url = apiBaseURL + "episode/" + id;

			//get JSON data
			string JsonData = getAPIData (url);
			JObject json_ep = JObject.Parse(JsonData);

			//a list of all the sources returned from GuideBox
			List< List<JToken> > sources = new List< List<JToken> >();
			sources.Add (json_ep["free_web_sources"].Children().ToList());
			sources.Add (json_ep["tv_everywhere_web_sources"].Children().ToList());
			sources.Add (json_ep["subscription_web_sources"].Children().ToList());
			sources.Add (json_ep["purchase_web_sources"].Children().ToList());

			Dictionary<string, List<string> > sourceLinks = new Dictionary<string, List<string> >();

			foreach (var sourceList in sources){
				foreach (var source in sourceList){
					var definition = new {source = "", id = "", display_name = "", link = ""};				//anonymous object definition
					var source_json = JsonConvert.DeserializeAnonymousType(source.ToString(), definition);
					string sourceName = source_json.display_name;

					if (sourceName == "Amazon Prime") //we don't make the distinction between amazon prime and amazon
						sourceName = "Amazon";
					
					if (sourceName == "Hulu" || sourceName == "Amazon" || sourceName == "YouTube") { //only care about these three

						//add to source links dictionary
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

		//get most popular movies
		//limit1 is where to start in list of results
		//limit2 is how many results to return from Guidebox
		public static List<Show> getMovieIds(int limit1, int limit2, Source source){
			//url is base/movies/all/{where to start}/{how many}/{source}/web
			string url = apiBaseURL + "movies/all/" + limit1.ToString() + "/" + limit2.ToString() + "/" + SourceToString(source) + "/web";
		
			JObject search = new JObject();

			//get JSON data
			string JsonData = getAPIData (url);
			search = JObject.Parse (JsonData);

			List<JToken> results = search["results"].Children().ToList();
			List<Show> shows = new List<Show>();
			foreach (var result in results){
				var definition = new {title = "", id = "", poster_240x342 = ""};
				var show_json = JsonConvert.DeserializeAnonymousType(result.ToString(), definition);

				//create show
				Show show = new Show (show_json.title, show_json.id, true);
				show.thumbURL = show_json.poster_240x342;
				shows.Add(show);
			}
			return shows;
		}

		//get movies from a query
		public static List<Show> getMovieIds(string title){
			//they want the title triple url encoded
			//url is base/search/movie/title/{url encoded query}/fuzzy
			string url = apiBaseURL + "search/movie/title/" + WebUtility.UrlEncode(WebUtility.UrlEncode(WebUtility.UrlEncode(title))) + "/fuzzy";
			JObject search = new JObject();

			//get JSON data
			string JsonData = getAPIData (url);
			search = JObject.Parse (JsonData);

			List<JToken> results = search["results"].Children().ToList();
			List<Show> shows = new List<Show>();
			foreach (var result in results){
				var definition = new {title = "", id = "", poster_240x342 = ""};					//anonymous object definition
				var show_json = JsonConvert.DeserializeAnonymousType(result.ToString(), definition);

				//create show
				Show show = new Show (show_json.title, show_json.id, true);
				show.thumbURL = show_json.poster_240x342;
				shows.Add(show);
			}
			return shows;
		}

		//get sources from a show(movie) id
		public static Dictionary<string, List<string> > getMovieLinks(string id){
			//url is base/movie/{show id}
			string url = apiBaseURL + "movie/" + id;

			//get JSON data
			string JsonData = getAPIData (url);
			JObject json_ep = JObject.Parse(JsonData);

			//create list of the sources GuideBox returned
			List< List<JToken> > sources = new List< List<JToken> >();
			sources.Add (json_ep["free_web_sources"].Children().ToList());
			sources.Add (json_ep["tv_everywhere_web_sources"].Children().ToList());
			sources.Add (json_ep["subscription_web_sources"].Children().ToList());
			sources.Add (json_ep["purchase_web_sources"].Children().ToList());

			Dictionary<string, List<string> > sourceLinks = new Dictionary<string, List<string> >();

			foreach (var sourceList in sources){
				foreach (var source in sourceList){
					var definition = new {source = "", id = "", display_name = "", link = ""};			//anonymous object definition
					var source_json = JsonConvert.DeserializeAnonymousType(source.ToString(), definition);
					string sourceName = source_json.display_name;

					if (sourceName == "Amazon Prime") //we don't make the distinction between amazon prime and amazon
						sourceName = "Amazon";

					if (sourceName == "Hulu" || sourceName == "Amazon" || sourceName == "YouTube") { //only care about these three

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

