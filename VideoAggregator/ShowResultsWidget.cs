using System;
using System.Collections.Generic;

namespace VideoAggregator
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class ShowResultsWidget : EmbeddedWidget
	{
		public ShowResultsWidget (MainWindow parent, List<Show> shows)
		{
			this.Build ();
			this.parent = parent;

			Gtk.ListStore showListStore = new Gtk.ListStore (typeof (string), typeof (Gdk.Pixbuf), typeof(Show));

			this.treeview.AppendColumn ("Title", new Gtk.CellRendererText (), "text", 0);
			this.treeview.AppendColumn ("Thumb", new Gtk.CellRendererPixbuf (), "pixbuf", 1); //Adds a column for Thumbnails 

			foreach (var show in shows) {
				showListStore.AppendValues (show.title, show.thumb, show); 
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

