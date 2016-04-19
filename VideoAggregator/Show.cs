using System;
using System.Collections.Generic;

namespace VideoAggregator {
    public class Show {
        public string title;
		public string id;
		public bool isMovie;
		public string thumbURL;
		public string desc;
		public int numOfSeasons;
        public List<Season> seasons;
		Gdk.Pixbuf thumb; //added 

		public Show(string title, string id, Gdk.Pixbuf thumb, bool isMovie = false) {
			this.title = title;
            this.id = id;
			this.thumb = thumb; 
			this.isMovie = isMovie;
			numOfSeasons = 0;
            thumbURL = null;
			//thumb = null; //added 
            desc = "";
			seasons = null;
        }
    }
}
