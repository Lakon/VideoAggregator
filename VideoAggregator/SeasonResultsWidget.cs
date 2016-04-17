using System;

namespace VideoAggregator
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class SeasonResultsWidget : Gtk.Bin
	{
		private MainWindow parent;
		private Show show;
		public SeasonResultsWidget (MainWindow parent, Show show)
		{
			this.Build ();
			this.parent = parent;
			this.show = show;

			Gtk.ListStore seasonListStore = new Gtk.ListStore (typeof (int));

			this.treeview.AppendColumn ("Season", new Gtk.CellRendererText (), "text", 0);

			for(int i = 1; i <= show.numOfSeasons; i++) {
				seasonListStore.AppendValues (i);
			}

			this.treeview.Model = seasonListStore;
			this.ShowAll ();
			Console.WriteLine ("SeasonResultsWindow Created");
		}

		protected void OnSeasonSelected (object o, Gtk.RowActivatedArgs args)
		{
			Gtk.TreeIter iter;
			this.treeview.Model.GetIter (out iter, args.Path);
			int season = (int) this.treeview.Model.GetValue (iter, 0);
			parent.seasonSelected (this.show, season);
		}
	}
}

