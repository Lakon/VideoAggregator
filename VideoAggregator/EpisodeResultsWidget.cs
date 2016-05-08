/* EpisodeResultsWidget.cs
 * Subclass of EmbeddedWidget
 * Handles the logic for displaying episodes
 * Parameters are a parent (MainWindow), a season, and the start of the results
*/

using System;

namespace VideoAggregator
{
	//[System.ComponentModel.ToolboxItem (true)]
	public class EpisodeResultsWidget : EmbeddedWidget
	{
		private Season season; //holds the episodes

		public EpisodeResultsWidget (MainWindow parent, Season season, int start) : base()
		{
			this.parent = parent;
			this.season = season;
			this.start = start;

			this.Build ();
			this.ShowAll ();

			//only show loadmore button if there are more results to display
			if (season.episodes.Count <= start + 25)
				loadMoreButton.Destroy ();
		}

		protected new void Build ()
		{
			this.Name = "EpisodeResultsWidget";
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			initTable ();
			populateTable ();
		}

		//Add the seasons to the table
		//Each one is stored in an eventbox
		protected void populateTable(){
			int curEpisode = start;
			for (uint i = 0; i < 5; i++) {
				if (curEpisode >= season.episodes.Count)
					break;

				for (uint j = 0; j < 5; j++) {
					if (curEpisode >= season.episodes.Count)
						break;

					//episode item
					Gtk.Image img = new Gtk.Image();
					if (season.episodes[curEpisode].thumb != null)
						img.Pixbuf = season.episodes[curEpisode].thumb;

					Gtk.Label lbl = new Gtk.Label ((curEpisode+1).ToString() + ". " + season.episodes[curEpisode].title);
					lbl.ModifyFont (Pango.FontDescription.FromString("12"));
					Gtk.VBox box = new Gtk.VBox ();
					box.Add (img);
					box.Add (lbl);
					Gtk.EventBox eventbox = new Gtk.EventBox ();
					eventbox.Add (box);

					//create event for clicking an episode
					Func<Episode, Gtk.ButtonPressEventHandler> ButtonPressWrapper = ((ep) => ((s, e) => { OnEpisodeSelected(s, e, ep); }));
					eventbox.ButtonPressEvent += ButtonPressWrapper(season.episodes[curEpisode]);

					//create hover events
					Func<Gtk.EventBox, Gtk.EnterNotifyEventHandler> EnterNotifyWrapper = ((Gtk.EventBox eBox) => ((s, e) => {OnHoverEnter(s,e,eBox);}));
					eventbox.EnterNotifyEvent += EnterNotifyWrapper(eventbox);

					Func<Gtk.EventBox, Gtk.LeaveNotifyEventHandler> LeaveNotifyWrapper = ((Gtk.EventBox eBox) => ((s, e) => {OnHoverLeave(s,e,eBox);}));
					eventbox.LeaveNotifyEvent += LeaveNotifyWrapper(eventbox);

					table.Attach (eventbox, j, j + 1, i, i + 1);

					curEpisode++;
				}
			}
		}

		//Event handler for selecting an episode. Calls the MainWindow method
		protected void OnEpisodeSelected (object o, Gtk.ButtonPressEventArgs args, Episode ep)
		{
			parent.episodeSelected (ep);
		}

		//Event handler for clicking the loadmore button. Calls the MainWindow method
		protected override void OnLoadMoreClicked (object sender, EventArgs e)
		{
			parent.loadMoreResults (new EpisodeResultsWidget (parent, season, start + 25));
		}
	}
}

