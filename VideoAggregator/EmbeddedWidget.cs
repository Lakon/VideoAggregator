using System;

namespace VideoAggregator
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class EmbeddedWidget : Gtk.Bin
	{
		protected MainWindow parent;
		protected Gtk.Table table;

		public EmbeddedWidget ()
		{
			this.Build ();
			this.parent = null;
			//initTable ();
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
		protected void addToVBox(Gtk.Widget w){
			vbox.PackStart (w);
		}
		protected void addToScrolledWindow(Gtk.Viewport t){
			scrolledwindow.Add (t);
		}


		protected virtual void OnHoverEnter(object o, Gtk.EnterNotifyEventArgs args, Gtk.EventBox eBox){
			eBox.State = Gtk.StateType.Selected;
		}
		protected virtual void OnHoverLeave(object o, Gtk.LeaveNotifyEventArgs args, Gtk.EventBox eBox){
			eBox.State = Gtk.StateType.Normal;
		}
	}
}

