using System;

namespace VideoAggregator
{
	//[System.ComponentModel.ToolboxItem (true)]
	public class SeasonResultsWidget : EmbeddedWidget
	{
		private Show show;
		public SeasonResultsWidget (MainWindow parent, Show show, int start) : base()
		{
			this.parent = parent;
			this.show = show;
			this.start = start;

			this.Build ();
			this.ShowAll ();

			if (show.numOfSeasons <= start + 25)
				loadMoreButton.Destroy ();

		}

		protected new void Build ()
		{
			this.Name = "SeasonResultsWidget";
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			initTable ();
			populateTable ();
		}

		protected void populateTable(){
			int curSeason = start;
			for (uint i = 0; i < 5; i++) {
				if (curSeason >= show.numOfSeasons)
					break;

				for (uint j = 0; j < 5; j++) {
					if (curSeason >= show.numOfSeasons)
						break;

					Gtk.Image img = new Gtk.Image();
					if (show.thumb != null)
						img.Pixbuf = show.thumb;

					Gtk.Label lbl = new Gtk.Label ("Season " + (curSeason+1).ToString());
					lbl.ModifyFont (Pango.FontDescription.FromString("12"));

					Gtk.VBox box = new Gtk.VBox ();
					box.Add (img);
					box.Add (lbl);
					Gtk.EventBox eventbox = new Gtk.EventBox ();
					eventbox.Add (box);

					Func<int, Gtk.ButtonPressEventHandler> ButtonPressWrapper = ((season) => ((s, e) => { OnSeasonSelected(s, e, season); }));
					eventbox.ButtonPressEvent += ButtonPressWrapper(curSeason);

					Func<Gtk.EventBox, Gtk.EnterNotifyEventHandler> EnterNotifyWrapper = ((Gtk.EventBox eBox) => ((s, e) => {OnHoverEnter(s,e,eBox);}));
					eventbox.EnterNotifyEvent += EnterNotifyWrapper(eventbox);

					Func<Gtk.EventBox, Gtk.LeaveNotifyEventHandler> LeaveNotifyWrapper = ((Gtk.EventBox eBox) => ((s, e) => {OnHoverLeave(s,e,eBox);}));
					eventbox.LeaveNotifyEvent += LeaveNotifyWrapper(eventbox);

					table.Attach (eventbox, j, j + 1, i, i + 1);

					curSeason++;

				}
			}
		}

		protected void OnSeasonSelected (object o, Gtk.ButtonPressEventArgs args, int season)
		{
			parent.seasonSelected (this.show, season);
		}

		protected override void OnLoadMoreClicked (object sender, EventArgs e)
		{
			parent.loadMoreResults (new SeasonResultsWidget (parent, show, start + 25));
		}
	}
}

