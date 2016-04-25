using System;

namespace VideoAggregator
{
	//[System.ComponentModel.ToolboxItem (true)]
	public class EmbeddedWidget : Gtk.Bin
	{
		protected MainWindow parent;
		protected Gtk.Table table;
		protected Gtk.VBox containerVbox;
		protected Gtk.VBox showContainer;
		protected Gtk.ScrolledWindow scrolledwindow;
		protected Gtk.Button loadMoreButton;

		protected int start;

		public EmbeddedWidget ()
		{
			this.Build ();
			this.parent = null;
			this.start = 0;
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

			global::Gtk.Viewport w1 = new global::Gtk.Viewport ();
			w1.ShadowType = ((global::Gtk.ShadowType)(0));

			this.showContainer = new global::Gtk.VBox ();
			this.showContainer.Name = "showContainer";
			this.showContainer.Spacing = 6;


			this.loadMoreButton = new Gtk.Button ();
			this.loadMoreButton.Hide();
			this.loadMoreButton.CanFocus = true;
			this.loadMoreButton.Name = "loadMoreButton";
			this.loadMoreButton.UseUnderline = true;
			this.loadMoreButton.Label = global::Mono.Unix.Catalog.GetString ("Load More");
			this.loadMoreButton.Clicked += new global::System.EventHandler (this.OnLoadMoreClicked);
			this.showContainer.PackEnd (this.loadMoreButton);
			Gtk.Box.BoxChild w2 = ((Gtk.Box.BoxChild)(this.showContainer [this.loadMoreButton]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			w1.Add (this.showContainer);
			this.scrolledwindow.Add (w1);
			this.containerVbox.PackEnd (this.scrolledwindow);
			Gtk.Box.BoxChild w3 = ((Gtk.Box.BoxChild)(this.containerVbox [this.scrolledwindow]));
			w3.Position = 0;

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
			//Gtk.Viewport w1 = new Gtk.Viewport ();
			//w1.ShadowType = ((Gtk.ShadowType)(0));
			//w1.Add (this.table);
			this.showContainer.PackStart (this.table);
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

		public virtual void loadMore(){
			//nothing by default
		}

		protected virtual void OnLoadMoreClicked (object sender, EventArgs e){
		}
	}
}

