using System;

namespace VideoAggregator
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class EmbeddedWidget : Gtk.Bin
	{
		protected MainWindow parent;
		public EmbeddedWidget ()
		{
			this.Build ();
			this.parent = null;
		}
	}
}

