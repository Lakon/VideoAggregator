/* SourcesWidget.cs
 * Subclass of EmbeddedWidget
 * Handles the logic for displaying sources
 * Parameters are a parent (MainWindow), a description, an image, a dictionary of sources, and
 * the currently active source.
 * SourcesWidget is a little different from the other subclasses. It has a description box, which holds
 * the image for the episode and a description of the episode. It's table is also smaller with only one 
 * row and three columns. It is the only widget to override OnSourcesChanged and it always hides the loadmore button.
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace VideoAggregator
{
	//[System.ComponentModel.ToolboxItem (true)]
	public class SourcesWidget : EmbeddedWidget
	{
		private string desc;
		private Gdk.Pixbuf thumb;

		//keys are the name of the source
		//values is a list of urls
		private Dictionary<string, List<string> > sources;

		//the currently active source in the SourceComboBox in the MainWindow
		private Source activeSource;

		public SourcesWidget (MainWindow parent, string desc, Gdk.Pixbuf thumb, Dictionary<string, List<string> > sources, Source activeSource) : base()
		{
			this.parent = parent;
			this.sources = sources;
			this.desc = desc;
			this.thumb = thumb;
			this.activeSource = activeSource;

			this.Build ();
			this.ShowAll ();
			this.loadMoreButton.Destroy (); //never more results to display

		}


		protected new void Build ()
		{
			this.Name = "SourcesWidget";
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}

			makeDescriptionBox ();
			initTable ();
			populateTable ();

		}

		//Makes the box that holds the image and description of episode/movie
		//puts it above the table
		protected void makeDescriptionBox(){
			Gtk.Image img = new Gtk.Image();
			if (thumb != null)
				img.Pixbuf = thumb;

			Gtk.Label lbl = new Gtk.Label (desc);
			lbl.LineWrap = true;
			lbl.ModifyFont (Pango.FontDescription.FromString("12"));
			lbl.WidthRequest = 600;
			Gtk.VBox box = new Gtk.VBox ();
			box.Add (img);
			box.Add (lbl);
			containerVbox.PackStart(box);
		}

		//overrides initTable to make a table with only 1 row and 3 columns
		protected override void initTable(){
			this.table = new Gtk.Table (((uint)(3)), ((uint)(1)), true);
			this.table.Name = "table";
			this.table.RowSpacing = ((uint)(1));
			this.table.ColumnSpacing = ((uint)(6));
			this.showContainer.PackStart (this.table);
		}

		//populate the table with the sources
		//each source is in an eventbox
		protected void populateTable(){
			List<string> srcs = sources.Keys.ToList();

			//change what will be displayed based on the active source
			switch (activeSource) {
			case Source.All:
				break;
			case Source.Hulu:
				if (srcs.Contains ("Hulu"))
					srcs = new List<string>{ "Hulu" };
				else
					srcs = new List<string> ();
				break;
			case Source.Amazon:
				if (srcs.Contains ("Amazon"))
					srcs = new List<string>{ "Amazon" };
				else
					srcs = new List<string> ();
				break;
			case Source.YouTube:
				if (srcs.Contains ("YouTube"))
					srcs = new List<string>{ "YouTube" };
				else
					srcs = new List<string> ();
				break;
			}

			//loop through table
			int curSource = 0;
			for (uint i = 0; i < 1; i++) {
				if (curSource >= srcs.Count)
					break;

				for (uint j = 0; j < 3; j++) {
					if (curSource >= srcs.Count)
						break;

					//image for the sources are stored in the MainWindow
					Gtk.Image img = new Gtk.Image();
					if (srcs [curSource] == "Hulu")
						img.Pixbuf = MainWindow.huluLogo;
					else if ((srcs[curSource] == "Amazon"))
						img.Pixbuf = MainWindow.amazonLogo;
					else if ((srcs[curSource] == "YouTube"))
						img.Pixbuf = MainWindow.youtubeLogo;


					//source item
					Gtk.Label lbl = new Gtk.Label (srcs[curSource]);
					lbl.ModifyFont (Pango.FontDescription.FromString("12"));
					Gtk.VBox box = new Gtk.VBox ();
					box.Add (img);
					box.Add (lbl);
					Gtk.EventBox eventbox = new Gtk.EventBox ();
					eventbox.Add (box);

					//create event for clicking a source
					Func<string, Gtk.ButtonPressEventHandler> ButtonPressWrapper = ((src) => ((s, e) => { OnSourceSelected(s, e, src); }));
					eventbox.ButtonPressEvent += ButtonPressWrapper(srcs[curSource]);

					//create hover events
					Func<Gtk.EventBox, Gtk.EnterNotifyEventHandler> EnterNotifyWrapper = ((Gtk.EventBox eBox) => ((s, e) => {OnHoverEnter(s,e,eBox);}));
					eventbox.EnterNotifyEvent += EnterNotifyWrapper(eventbox);

					Func<Gtk.EventBox, Gtk.LeaveNotifyEventHandler> LeaveNotifyWrapper = ((Gtk.EventBox eBox) => ((s, e) => {OnHoverLeave(s,e,eBox);}));
					eventbox.LeaveNotifyEvent += LeaveNotifyWrapper(eventbox);


					table.Attach (eventbox, j, j + 1, i, i + 1);

					curSource++;
					if (curSource >= sources.Keys.Count)
						break;
				}
			}
		}

		//Event handler for selecting a source. Calls the MainWindow method
		protected void OnSourceSelected (object o, Gtk.ButtonPressEventArgs args, string source)
		{
			List<string> urls = sources[source];
			parent.sourceSelected (source, urls);
		}

		//Updates active source and then the table. Called from MainWindow
		public override void OnSourceChanged(Source activeSource){
			this.activeSource = activeSource;
			showContainer.Remove (table);
			initTable ();
			populateTable ();
			this.ShowAll ();
		}
	}
}

