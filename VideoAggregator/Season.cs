using System;
using System.Collections.Generic;

namespace VideoAggregator {
    public class Season {
        public string num;
        public string thumbURL;
        public string desc;
        public List<Episode> episodes;

        public Season(string num) {
            this.num = num;
            this.episodes = null;
            thumbURL = null;
            desc = "";
        }
    }
}
