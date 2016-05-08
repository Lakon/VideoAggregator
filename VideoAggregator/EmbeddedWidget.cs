/* EmbeddedWidget.cs
 * Base class for the classes handling the logic for displaying results
*/


using System;

namespace VideoAggregator
{
	//[System.ComponentModel.ToolboxItem (true)]
	public class EmbeddedWidget : Gtk.Bin
	{
		//gui variables
		protected Gtk.Table table;
		protected Gtk.VBox containerVbox;
		protected Gtk.VBox showContainer;
		protected Gtk.ScrolledWindow scrolledwindow;
		protected Gtk.Button loadMoreButton;

		//each embedded widget needs a reference to the mainWindow
		protected MainWindow parent;

		//this keeps track of where to start in the list of results
		protected int start;

		public EmbeddedWidget ()
		{
			this.Build ();
			this.parent = null;
			this.start = 0;
			//initTable ();
		}

		//creating gui elements
		//most of this is copied straight out of the generated code from the designer
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

		//create a 5 by 5 table and add it to gui
		protected virtual void initTable(){
			this.table = new Gtk.Table (((uint)(5)), ((uint)(5)), true);
			this.table.Name = "table";
			this.table.RowSpacing = ((uint)(6));
			this.table.ColumnSpacing = ((uint)(6));
			this.showContainer.PackStart (this.table);
		}

		//required by base class Gtk.Bin
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			if (this.Child != null)
			{
				this.Child.Allocation = allocation;
			}
		}

		//required by base class Gtk.Bin
		protected override void OnSizeRequested (ref Gtk.Requisition requisition)
		{
			if (this.Child != null)
			{
				requisition = this.Child.SizeRequest ();
			}
		}

		//Event Handlers for the hover animations
		//basically just activates an eventbox when the mouse enters an eventbox
		//and makes it normal once it leaves again
		protected virtual void OnHoverEnter(object o, Gtk.EnterNotifyEventArgs args, Gtk.EventBox eBox){
			eBox.State = Gtk.StateType.Selected;
		}
		protected virtual void OnHoverLeave(object o, Gtk.LeaveNotifyEventArgs args, Gtk.EventBox eBox){
			eBox.State = Gtk.StateType.Normal;
		}

		//Needed for polymorphism
		//MainWindow calls this from the embeddedWidget and the subclasses of this class
		//handle the logic if any
		public virtual void OnSourceChanged(Source activeSource){
			//nothing by default
		}

		//Event handler for clicking the load more button
		//logic handled by subclasses
		protected virtual void OnLoadMoreClicked (object sender, EventArgs e){
		}
	}
}

