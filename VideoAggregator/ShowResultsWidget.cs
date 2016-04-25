using System;
using System.Collections.Generic;

namespace VideoAggregator
{
	//[System.ComponentModel.ToolboxItem (true)]
	public class ShowResultsWidget : EmbeddedWidget
	{
		private List<Show> shows;
		private bool isSearch;
		public ShowResultsWidget (MainWindow parent, List<Show> shows, int start, bool isSearch) : base()
		{
			this.parent = parent;
			this.shows = shows;
			this.start = start;
			this.isSearch = isSearch;

				

			this.Build ();
			this.ShowAll ();

			if ((shows.Count <= start + 25 && isSearch) || start + 25 > MainWindow.maxShows)
				loadMoreButton.Destroy();

			Console.WriteLine ("ShowResultsWidget Created");
		}

		protected new void Build ()
		{
			this.Name = "ShowResultsWidget";
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			initTable ();
			populateTable ();
		}

		protected void populateTable(){
			int curShow;
			if (isSearch)
				curShow = start;
			else
				curShow = 0;
			
			for (uint i = 0; i < 5; i++) {
				if (curShow >= shows.Count)
					break;

				for (uint j = 0; j < 5; j++) {
					if (curShow >= shows.Count)
						break;

					Gtk.Image img = new Gtk.Image();
					if (shows[curShow].thumb != null)
						img.Pixbuf = shows[curShow].thumb;

					Gtk.Label lbl = new Gtk.Label (shows[curShow].title);
					Gtk.VBox box = new Gtk.VBox ();
					box.Add (img);
					box.Add (lbl);
					Gtk.EventBox eventbox = new Gtk.EventBox ();
					eventbox.Add (box);

					Func<Show, Gtk.ButtonPressEventHandler> ButtonPressWrapper = ((show) => ((s, e) => { OnShowSelected(s, e, show); }));
					eventbox.ButtonPressEvent += ButtonPressWrapper(shows[curShow]);

					Func<Gtk.EventBox, Gtk.EnterNotifyEventHandler> EnterNotifyWrapper = ((Gtk.EventBox eBox) => ((s, e) => {OnHoverEnter(s,e,eBox);}));
					eventbox.EnterNotifyEvent += EnterNotifyWrapper(eventbox);

					Func<Gtk.EventBox, Gtk.LeaveNotifyEventHandler> LeaveNotifyWrapper = ((Gtk.EventBox eBox) => ((s, e) => {OnHoverLeave(s,e,eBox);}));
					eventbox.LeaveNotifyEvent += LeaveNotifyWrapper(eventbox);

					table.Attach (eventbox, j, j + 1, i, i + 1);

					curShow++;
				}
			}

		}

		protected void OnShowSelected (object o, Gtk.ButtonPressEventArgs args, Show show)
		{
			parent.showSelected (show);
		}

		protected override void OnLoadMoreClicked (object sender, EventArgs e)
		{
			if (isSearch) {
				parent.loadMoreResults (new ShowResultsWidget (parent, shows, start + 25, true));
			} 
			else {
				parent.loadMorePopShows (start + 25);
			}
		}
	}
}

