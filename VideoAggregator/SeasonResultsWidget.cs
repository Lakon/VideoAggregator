using System;

namespace VideoAggregator
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class SeasonResultsWidget : EmbeddedWidget
	{
		private Show show;
		public SeasonResultsWidget (MainWindow parent, Show show)
		{
			this.Build ();
			this.parent = parent;
			this.show = show;
			initTable ();

			int curSeason = 1;
			for (uint i = 0; i < 5; i++) {
				if (curSeason > show.numOfSeasons)
					break;
				
				for (uint j = 0; j < 5; j++) {
					if (curSeason > show.numOfSeasons)
						break;

					Gtk.Image img = new Gtk.Image();
					if (show.thumb != null)
						img.Pixbuf = show.thumb;

					Gtk.Label lbl = new Gtk.Label ("Season " + curSeason.ToString());
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
			this.ShowAll ();
			Console.WriteLine ("SeasonResultsWidget Created");
		}

		protected void OnSeasonSelected (object o, Gtk.ButtonPressEventArgs args, int season)
		{
			parent.seasonSelected (this.show, season);
		}
	}
}

