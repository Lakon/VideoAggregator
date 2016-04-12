using System;
using System.Collections.Generic;

namespace VideoAggregator
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class SourcesWidget : Gtk.Bin
	{
		private MainWindow parent;
		public SourcesWidget (MainWindow parent, Dictionary<string, List<string> > sources)
		{
			this.Build ();
			this.parent = parent;

			Gtk.ListStore sourcesListStore = new Gtk.ListStore (typeof(string), typeof(List<string>));

			this.treeview.AppendColumn ("Source", new Gtk.CellRendererText (), "text", 0);

			foreach (var source in sources.Keys) {
				sourcesListStore.AppendValues (source, sources[source]);
			}

			this.treeview.Model = sourcesListStore;
			this.ShowAll ();
		}

		public SourcesWidget (SourcesWidget other)
		{
			this.Build ();
			this.parent = other.parent;
			this.treeview = other.treeview;
			this.ShowAll ();
		}

		protected void OnSourceSelected (object o, Gtk.RowActivatedArgs args)
		{
			Gtk.TreeIter iter;
			this.treeview.Model.GetIter (out iter, args.Path);
			string source = (string) this.treeview.Model.GetValue (iter, 0);
			List<string> urls = (List<string>) this.treeview.Model.GetValue (iter, 1);
			parent.sourceSelected (source, urls);
		}
	}
}

