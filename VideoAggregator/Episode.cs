using System;

namespace VideoAggregator {
    public class Episode {
        public string season;
        public string num;
        public string title;
        public string id;
        public string thumbURL;
        public string desc;

        public Episode(string season, string num, string title, string id) {
            this.season = season;
            this.num = num;
            this.title = title;
            this.id = id;
            thumbURL = null;
            desc = "";
        }
    }
}
