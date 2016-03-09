using System;
using System.Collections.Generic;

namespace VideoAggregator {
    class Show {
        public String name;
        public int id;
        public String thumbURL;
        public String desc;
        public List<Season> seasons;

        public Show(String name, int id) {
            this.name = name;
            this.id = id;
            thumbURL = null;
            desc = "";
			seasons = null;
        }
    }
}
