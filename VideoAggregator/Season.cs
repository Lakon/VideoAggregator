using System;
using System.Collections.Generic;

namespace VideoAggregator {
    public class Season {
        public string num;
        public string thumbURL;
        public string desc;
        public List<Episode> episodes;

        public Season(string season, List<Episode> episodes) {
            this.num = season;
            this.episodes = episodes;
            thumbURL = null;
            desc = "";
        }
    }
}
