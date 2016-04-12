using System;
using System.Collections.Generic;

namespace VideoAggregator {
    public class Show {
        public string title;
		public string id;
		public string thumbURL;
		public string desc;
		public int numOfSeasons;
        public List<Season> seasons;

		public Show(string title, string id) {
			this.title = title;
            this.id = id;
			numOfSeasons = 0;
            thumbURL = null;
            desc = "";
			seasons = null;
        }
    }
}
