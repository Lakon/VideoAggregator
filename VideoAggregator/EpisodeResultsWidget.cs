using System;

namespace VideoAggregator
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class EpisodeResultsWidget : EmbeddedWidget
	{
		public EpisodeResultsWidget (MainWindow parent, Season season)
		{
			this.Build ();
			this.parent = parent;

			Gtk.ListStore episodeListStore = new Gtk.ListStore (typeof (string), typeof (Gdk.Pixbuf), typeof(Episode));

			this.treeview.AppendColumn ("Title", new Gtk.CellRendererText (), "text", 0);
			this.treeview.AppendColumn ("Thumb", new Gtk.CellRendererPixbuf (), "pixbuf", 1); //Adds a column for Thumbnails 

			foreach (var ep in season.episodes) {
				episodeListStore.AppendValues (ep.title, ep.thumb, ep);
			}

			this.treeview.Model = episodeListStore;
			this.ShowAll ();
			Console.WriteLine ("EpisodeResultsWindow Created");
		}

		protected void OnEpisodeSelected (object o, Gtk.RowActivatedArgs args)
		{
			Gtk.TreeIter iter;
			this.treeview.Model.GetIter (out iter, args.Path);
			Episode ep = (Episode) this.treeview.Model.GetValue (iter, 2);
			parent.episodeSelected (ep);
		}
	}
}

