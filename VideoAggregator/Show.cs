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

		public Show(string title, string id, bool isMovie = false) {
			this.title = title;
            this.id = id;
			this.isMovie = isMovie;
			numOfSeasons = 0;
            thumbURL = null;
            desc = "";
			seasons = null;
        }
    }
}
