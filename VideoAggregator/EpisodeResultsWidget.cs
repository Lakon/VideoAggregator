using System;

namespace VideoAggregator
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class EpisodeResultsWidget : Gtk.Bin
	{
		private MainWindow parent;
		public EpisodeResultsWidget (MainWindow parent, Season season)
		{
			this.Build ();
			this.parent = parent;

			Gtk.ListStore episodeListStore = new Gtk.ListStore (typeof (string),  typeof (string), typeof(Episode));

			this.treeview.AppendColumn ("Title", new Gtk.CellRendererText (), "text", 0);
			this.treeview.AppendColumn ("Thumb URL", new Gtk.CellRendererText (), "text", 1);

			foreach (var ep in season.episodes) {
				episodeListStore.AppendValues (ep.title, ep.thumbURL, ep);
				//Icon Build Here
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

