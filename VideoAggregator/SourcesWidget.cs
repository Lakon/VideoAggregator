using System;
using System.Collections.Generic;
using System.Linq;

namespace VideoAggregator
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class SourcesWidget : EmbeddedWidget
	{
		private string desc;
		private Gdk.Pixbuf thumb;
		private Dictionary<string, List<string> > sources;
		public SourcesWidget (MainWindow parent, string desc, Gdk.Pixbuf thumb, Dictionary<string, List<string> > sources)
		{
			this.Build ();
			this.parent = parent;
			this.sources = sources;
			this.desc = desc;
			this.thumb = thumb;
			initTable ();

			List<string> srcs = sources.Keys.ToList();

			int curSource = 0;
			for (uint i = 0; i < 1; i++) {
				if (curSource >= sources.Keys.Count)
					break;
				
				for (uint j = 0; j < 3; j++) {
					if (curSource >= sources.Keys.Count)
						break;

					Gtk.Image img = new Gtk.Image();


					Gtk.Label lbl = new Gtk.Label (srcs[curSource]);
					Gtk.VBox box = new Gtk.VBox ();
					box.Add (img);
					box.Add (lbl);
					Gtk.EventBox eventbox = new Gtk.EventBox ();
					eventbox.Add (box);

					Func<string, Gtk.ButtonPressEventHandler> ButtonPressWrapper = ((src) => ((s, e) => { OnSourceSelected(s, e, src); }));
					eventbox.ButtonPressEvent += ButtonPressWrapper(srcs[curSource]);

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

			this.ShowAll ();
			Console.WriteLine ("SourcesWidget Created");
		}

		protected override void initTable(){
			Gtk.Image img = new Gtk.Image();
			if (thumb != null)
				img.Pixbuf = thumb;

			Gtk.Label lbl = new Gtk.Label (desc);
			Gtk.VBox box = new Gtk.VBox ();
			box.Add (img);
			box.Add (lbl);
			addToVBox (box);

			this.table = new Gtk.Table (((uint)(3)), ((uint)(1)), true);
			this.table.Name = "table";
			this.table.RowSpacing = ((uint)(1));
			this.table.ColumnSpacing = ((uint)(6));
			Gtk.Viewport w1 = new Gtk.Viewport ();
			w1.ShadowType = ((Gtk.ShadowType)(0));
			w1.Add (this.table);
			addToScrolledWindow (w1);
		}

		protected void OnSourceSelected (object o, Gtk.ButtonPressEventArgs args, string source)
		{
			List<string> urls = sources[source];
			parent.sourceSelected (source, urls);
		}
	}
}

