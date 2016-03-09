using System;

namespace VideoAggregator {
    class Episode {
        public int season;
        public int num;
        public String name;
        public int id;
        public String thumbURL;
        public String desc;

        public Episode(int season, int num, String name, int id) {
            this.season = season;
            this.num = num;
            this.name = name;
            this.id = id;
            thumbURL = null;
            desc = "";
        }
    }
}
