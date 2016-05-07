using System;
using System.Collections.Generic;
using System.Linq;

namespace VideoAggregator
{
	//[System.ComponentModel.ToolboxItem (true)]
	public class SourcesWidget : EmbeddedWidget
	{
		private string desc;
		private Gdk.Pixbuf thumb;
		private Dictionary<string, List<string> > sources;
		private Source activeSource;
		public SourcesWidget (MainWindow parent, string desc, Gdk.Pixbuf thumb, Dictionary<string, List<string> > sources, Source activeSource) : base()
		{
			this.parent = parent;
			this.sources = sources;
			this.desc = desc;
			this.thumb = thumb;
			this.activeSource = activeSource;

			this.Build ();
			this.ShowAll ();
			this.loadMoreButton.Destroy ();

		}


		protected new void Build ()
		{
			this.Name = "SourcesWidget";
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}

			makeDescriptionBox ();
			initTable ();
			populateTable ();

		}

		protected void makeDescriptionBox(){
			Gtk.Image img = new Gtk.Image();
			if (thumb != null)
				img.Pixbuf = thumb;

			Gtk.Label lbl = new Gtk.Label (desc);
			lbl.LineWrap = true;
			lbl.ModifyFont (Pango.FontDescription.FromString("12"));
			lbl.WidthRequest = 600;
			Gtk.VBox box = new Gtk.VBox ();
			box.Add (img);
			box.Add (lbl);
			containerVbox.PackStart(box);
		}

		protected override void initTable(){
			this.table = new Gtk.Table (((uint)(3)), ((uint)(1)), true);
			this.table.Name = "table";
			this.table.RowSpacing = ((uint)(1));
			this.table.ColumnSpacing = ((uint)(6));
			this.showContainer.PackStart (this.table);
		}

		protected void populateTable(){
			List<string> srcs = sources.Keys.ToList();

			switch (activeSource) {
			case Source.All:
				break;
			case Source.Hulu:
				if (srcs.Contains ("Hulu"))
					srcs = new List<string>{ "Hulu" };
				else
					srcs = new List<string> ();
				break;
			case Source.Amazon:
				if (srcs.Contains ("Amazon"))
					srcs = new List<string>{ "Amazon" };
				else
					srcs = new List<string> ();
				break;
			case Source.YouTube:
				if (srcs.Contains ("YouTube"))
					srcs = new List<string>{ "YouTube" };
				else
					srcs = new List<string> ();
				break;
			}

			int curSource = 0;
			for (uint i = 0; i < 1; i++) {
				if (curSource >= srcs.Count)
					break;

				for (uint j = 0; j < 3; j++) {
					if (curSource >= srcs.Count)
						break;

					Gtk.Image img = new Gtk.Image();
					if (srcs [curSource] == "Hulu")
						img.Pixbuf = MainWindow.huluLogo;
					else if ((srcs[curSource] == "Amazon"))
						img.Pixbuf = MainWindow.amazonLogo;
					else if ((srcs[curSource] == "YouTube"))
						img.Pixbuf = MainWindow.youtubeLogo;


					Gtk.Label lbl = new Gtk.Label (srcs[curSource]);
					lbl.ModifyFont (Pango.FontDescription.FromString("12"));
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
		}

		protected void OnSourceSelected (object o, Gtk.ButtonPressEventArgs args, string source)
		{
			List<string> urls = sources[source];
			parent.sourceSelected (source, urls);
		}

		public override void OnSourceChanged(Source activeSource){
			this.activeSource = activeSource;
			showContainer.Remove (table);
			initTable ();
			populateTable ();
			this.ShowAll ();
		}
	}
}

