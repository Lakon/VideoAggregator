using System;
using Json.Net;

namespace VideoAggregator
{
	public class GuideBoxApiClass
	{
		string apiHome = "http://api-public.guidebox.com/v1.43/1/";
		string apiKey = "rK4vt5WeKQvXmZgr8I0v7jbOVGCt9wtm/all/";



		public string getShowID(string showID)
		{
			int limit1 = 0;
			int limit2 = 0;

			showID = apiHome + apiKey + limit1.ToString () + "/" + limit2.ToString ();
			return showID;

		}

	}
}