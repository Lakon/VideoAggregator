/* Season.cs
 * Container class for the season result
 * Parameter is the season number
*/

using System;
using System.Collections.Generic;

namespace VideoAggregator {
    public class Season {
        public string num;				//current season number
		public string thumbURL;			//url for the thumbnail (unused).
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
