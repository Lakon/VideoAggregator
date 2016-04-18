using System;
using System.Collections.Generic;

namespace VideoAggregator
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class ShowResultsWidget : Gtk.Bin
	{
		private MainWindow parent;
		public ShowResultsWidget (MainWindow parent, List<Show> shows)
		{
			this.Build ();
			this.parent = parent;

			Gtk.ListStore showListStore = new Gtk.ListStore (typeof (string),  typeof (string), typeof(Show));

			this.treeview.AppendColumn ("Title", new Gtk.CellRendererText (), "text", 0);
			this.treeview.AppendColumn ("Thumb URL", new Gtk.CellRendererText (), "text", 1);
			this.treeview.AppendColumn ("Thumb", new Gtk.CellRendererPixbuf (), "pixbuf", 2); //Adds a column for Thumbnails 

				foreach (var show in shows) {
					showListStore.AppendValues (show.title, show.thumbURL, show, show.thumb); //added show.thumb
					//showListStore.AppendValues (show.title, show.thumbURL, show);
				//Icon Build Here
				//this.treeview.Image(show.thumbURL);
				//http://stackoverflow.com/questions/3887228/gtkbutton-just-shows-text-but-no-image
				}
			
			this.treeview.Model = showListStore;
			this.ShowAll ();
			Console.WriteLine ("ShowResultsWindow Created");
		}

		protected void OnShowSelected (object o, Gtk.RowActivatedArgs args)
		{
			Gtk.TreeIter iter;
			this.treeview.Model.GetIter (out iter, args.Path);
			Show show = (Show) this.treeview.Model.GetValue (iter, 2);
			parent.showSelected (show);
		}
	}
}

