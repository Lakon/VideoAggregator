using System;
//using Json.Net;

namespace VideoAggregator
{
	public class GuideBoxApiClass
	{
		string apiHome = "http://api-public.guidebox.com/v1.43/1/";
		string apiKey = "rK4vt5WeKQvXmZgr8I0v7jbOVGCt9wtm/all/";



		public string getSearchID(string searchID)
		{
			int limit1 = 1;
			int limit2 = 25;



			searchID = apiHome + apiKey + limit1.ToString () + "/" + limit2.ToString () + "/";


			return searchID;

		}

	}
}