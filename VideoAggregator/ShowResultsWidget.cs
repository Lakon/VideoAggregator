/* ShowResultsWidget.cs
 * Subclass of EmbeddedWidget
 * Handles the logic for displaying shows.
 * Parameters are a parent (MainWindow), a list of shows, the start of the shows, and 
 * a boolean (search or not)
*/

using System;
using System.Collections.Generic;

namespace VideoAggregator
{
	//[System.ComponentModel.ToolboxItem (true)]
	public class ShowResultsWidget : EmbeddedWidget
	{
		private List<Show> shows;

		//This boolean determines where the results came from
		//Either a search or a popular request
		//Affects the logic of the loadmore button
		private bool isSearch;

		public ShowResultsWidget (MainWindow parent, List<Show> shows, int start, bool isSearch) : base()
		{
			this.parent = parent;
			this.shows = shows;
			this.start = start;
			this.isSearch = isSearch;

				

			this.Build ();
			this.ShowAll ();

			//only show loadmore button if there are more results to display
			if ((shows.Count <= start + 25 && isSearch) || start + 25 > MainWindow.maxShows)
				loadMoreButton.Destroy();
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


		//Add the shows to the table
		//Each one is stored in an eventbox
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

					//show item
					Gtk.Image img = new Gtk.Image();
					if (shows[curShow].thumb != null)
						img.Pixbuf = shows[curShow].thumb;

					Gtk.Label lbl = new Gtk.Label (shows[curShow].title);
					lbl.ModifyFont (Pango.FontDescription.FromString("12"));
					Gtk.VBox box = new Gtk.VBox ();
					box.Add (img);
					box.Add (lbl);
					Gtk.EventBox eventbox = new Gtk.EventBox ();
					eventbox.Add (box);

					//create event for clicking show
					Func<Show, Gtk.ButtonPressEventHandler> ButtonPressWrapper = ((show) => ((s, e) => { OnShowSelected(s, e, show); }));
					eventbox.ButtonPressEvent += ButtonPressWrapper(shows[curShow]);

					//create hover events
					Func<Gtk.EventBox, Gtk.EnterNotifyEventHandler> EnterNotifyWrapper = ((Gtk.EventBox eBox) => ((s, e) => {OnHoverEnter(s,e,eBox);}));
					eventbox.EnterNotifyEvent += EnterNotifyWrapper(eventbox);

					Func<Gtk.EventBox, Gtk.LeaveNotifyEventHandler> LeaveNotifyWrapper = ((Gtk.EventBox eBox) => ((s, e) => {OnHoverLeave(s,e,eBox);}));
					eventbox.LeaveNotifyEvent += LeaveNotifyWrapper(eventbox);

					table.Attach (eventbox, j, j + 1, i, i + 1);

					curShow++;
				}
			}

		}

		//Event handler for selecting a show. Calls the MainWindow method
		protected void OnShowSelected (object o, Gtk.ButtonPressEventArgs args, Show show)
		{
			parent.showSelected (show);
		}

		//Event handler for clicking the loadmore button. Calls the MainWindow method
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

