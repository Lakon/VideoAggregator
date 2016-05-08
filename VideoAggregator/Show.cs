/* Show.cs
 * Container class for the show result
 * Parameters are title, id, and boolean (movie or not)
*/

using System;
using System.Collections.Generic;

namespace VideoAggregator {
    public class Show {
        public string title;
		public string id;				//GuideBox api show id
		public bool isMovie;			//boolean determining if show is tv or movie
		public string thumbURL;			//url for the thumbnail
		public string desc;
		public int numOfSeasons;
        public List<Season> seasons;	//currently never used because season only consists of list of numbers
		public Gdk.Pixbuf thumb;		//actual thumbnail img

		public Show(string title, string id, bool isMovie = false) {
			this.title = title;
            this.id = id; 
			this.isMovie = isMovie;
			numOfSeasons = 0;
            thumbURL = null;
			thumb = null;
            desc = "";
			seasons = null;
        }
    }
}
