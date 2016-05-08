/* Episode.cs
 * Container class for the episode result
 * Parameters are season number, episode number, title, and id
*/

using System;

namespace VideoAggregator {
    public class Episode {
        public string season;		//season number
        public string num;			//episode number
        public string title;		
        public string id;			//GuideBox API episode id
        public string thumbURL;		//url for thumbnail
        public string desc;
		public Gdk.Pixbuf thumb;	//actual thumbnail img


        public Episode(string season, string num, string title, string id) {
            this.season = season;
            this.num = num;
            this.title = title;
            this.id = id;
            thumbURL = null;
			thumb = null;
            desc = "";
        }
    }
}
