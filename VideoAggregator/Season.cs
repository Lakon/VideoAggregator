using System;
using System.Collections.Generic;

namespace VideoAggregator {
    class Season {
        public int num;
        public String thumbURL;
        public String desc;
        public List<Episode> episodes;

        public Season(int season, List<Episode> episodes) {
            this.num = season;
            this.episodes = episodes;
            thumbURL = null;
            desc = "";
        }
    }
}
