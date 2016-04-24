using System;

namespace VideoAggregator
{
	//[System.ComponentModel.ToolboxItem (true)]
	public class EmbeddedWidget : Gtk.Bin
	{
		protected MainWindow parent;
		protected Gtk.Table table;
		protected Gtk.VBox containerVbox;
		protected Gtk.ScrolledWindow scrolledwindow;

		public EmbeddedWidget ()
		{
			this.Build ();
			this.parent = null;
			//initTable ();
		}



		protected virtual void Build ()
		{
			this.Name = "EmbeddedWidget";

			// Container child VideoAggregator.EmbeddedWidget.Gtk.Container+ContainerChild
			this.containerVbox = new Gtk.VBox ();
			this.containerVbox.Name = "containerVbox";
			this.containerVbox.Spacing = 6;

			// Container child vbox.Gtk.Box+BoxChild
			this.scrolledwindow = new Gtk.ScrolledWindow ();
			this.scrolledwindow.CanFocus = true;
			this.scrolledwindow.Name = "scrolledwindow";
			this.scrolledwindow.ShadowType = ((Gtk.ShadowType)(1));
			this.containerVbox.Add (this.scrolledwindow);
			Gtk.Box.BoxChild w1 = ((Gtk.Box.BoxChild)(this.containerVbox [this.scrolledwindow]));
			w1.PackType = ((Gtk.PackType)(1));
			w1.Position = 0;
			this.Add (this.containerVbox);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
		}

		protected virtual void initTable(){
			this.table = new Gtk.Table (((uint)(5)), ((uint)(5)), true);
			this.table.Name = "table";
			this.table.RowSpacing = ((uint)(6));
			this.table.ColumnSpacing = ((uint)(6));
			Gtk.Viewport w1 = new Gtk.Viewport ();
			w1.ShadowType = ((Gtk.ShadowType)(0));
			w1.Add (this.table);
			this.scrolledwindow.Add (w1);
		}

		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			if (this.Child != null)
			{
				this.Child.Allocation = allocation;
			}
		}

		protected override void OnSizeRequested (ref Gtk.Requisition requisition)
		{
			if (this.Child != null)
			{
				requisition = this.Child.SizeRequest ();
			}
		}


		protected virtual void OnHoverEnter(object o, Gtk.EnterNotifyEventArgs args, Gtk.EventBox eBox){
			eBox.State = Gtk.StateType.Selected;
		}
		protected virtual void OnHoverLeave(object o, Gtk.LeaveNotifyEventArgs args, Gtk.EventBox eBox){
			eBox.State = Gtk.StateType.Normal;
		}

		public virtual void OnSourceChanged(Source activeSource){
			//nothing by default
		}
	}
}

