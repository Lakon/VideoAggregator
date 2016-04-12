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

			foreach (var show in shows) {
				showListStore.AppendValues (show.title, show.thumbURL, show);
			}

			this.treeview.Model = showListStore;
			this.ShowAll ();
		}

		public ShowResultsWidget (ShowResultsWidget other)
		{
			this.Build ();
			this.parent = other.parent;
			this.treeview = other.treeview;
			this.ShowAll ();
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

