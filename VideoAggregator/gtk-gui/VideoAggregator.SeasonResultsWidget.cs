
// This file has been generated by the GUI designer. Do not modify.
namespace VideoAggregator
{
	public partial class SeasonResultsWidget
	{
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		
		private global::Gtk.TreeView treeview;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget VideoAggregator.SeasonResultsWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "VideoAggregator.SeasonResultsWidget";
			// Container child VideoAggregator.SeasonResultsWidget.Gtk.Container+ContainerChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.treeview = new global::Gtk.TreeView ();
			this.treeview.CanFocus = true;
			this.treeview.Name = "treeview";
			this.GtkScrolledWindow.Add (this.treeview);
			this.Add (this.GtkScrolledWindow);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
			this.treeview.RowActivated += new global::Gtk.RowActivatedHandler (this.OnSeasonSelected);
		}
	}
}