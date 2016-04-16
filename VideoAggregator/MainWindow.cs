using System;
using Gtk;
using System.Collections.Generic;

namespace VideoAggregator
{
	public partial class MainWindow: Gtk.Window
	{
		private Gtk.Widget embeddedWidget;
		private Stack<Gtk.Widget> previousWidgets;
		public MainWindow () : base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
			List<Show> shows = GuideBoxAPIWrapper.getTVShowIds (0, 25, Source.All);
			embeddedWidget = new ShowResultsWidget (this, shows);
			this.container.Add (embeddedWidget);
			this.ShowAll ();

			previousWidgets = new Stack<Gtk.Widget> ();

				
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{

			Application.Quit ();
			a.RetVal = true;
		}


		public void showSelected(Show show){
			this.container.Remove(embeddedWidget);

			previousWidgets.Push (embeddedWidget);

			show.numOfSeasons = GuideBoxAPIWrapper.getTVShowSeasons (show.id);
			embeddedWidget = new SeasonResultsWidget (this, show);

			this.container.Add (embeddedWidget);
		}


		public void seasonSelected(Show show, int s){
			this.container.Remove(embeddedWidget);

			previousWidgets.Push (embeddedWidget);

			Season season = new Season(s.ToString());
			season.episodes = GuideBoxAPIWrapper.getTVShowEpisodes (show.id, s.ToString());
			embeddedWidget = new EpisodeResultsWidget (this, season);


			this.container.Add (embeddedWidget);
		}


		public void episodeSelected(Episode episode){
			this.container.Remove(embeddedWidget);

			previousWidgets.Push (embeddedWidget);

			Dictionary<string, List<string> > sources = GuideBoxAPIWrapper.getEpisodeLinks (episode.id);
			embeddedWidget = new SourcesWidget (this, sources);

			this.container.Add (embeddedWidget);
		}

		public void sourceSelected(string source, List<string> urls){

			int counter = 0;
			Console.WriteLine (source);
			foreach (string url in urls) {
				Console.WriteLine (url);
				counter++;
			}
			switch(counter)
			{
			case 1:

				System.Diagnostics.Process.Start ("/Applications/Firefox.app/Contents/MacOS/Firefox", urls [counter -1]);
				break;
			case 2:
				
				System.Diagnostics.Process.Start ("/Applications/Firefox.app/Contents/MacOS/Firefox", urls [counter -1]);
				break;
			case 3:
				
				System.Diagnostics.Process.Start ("/Applications/Firefox.app/Contents/MacOS/Firefox", urls [counter -1]);
				break;
			}
		}

		protected void OnBackButtonClicked (object sender, EventArgs e)
		{
			if (previousWidgets.Count != 0) {
				this.container.Remove (embeddedWidget);

				embeddedWidget = previousWidgets.Pop ();

				this.container.Add (embeddedWidget);
			}
		}

		protected void OnSearchButtonClicked (object sender, EventArgs e)
		{
			this.container.Remove(embeddedWidget);

			previousWidgets.Push (embeddedWidget);

			string searchText = null;
			searchText = searchEntry.Text;

			//if (searchText != null) {
				List<Show> shows = GuideBoxAPIWrapper.getTVShowIds (searchText);
				embeddedWidget = new ShowResultsWidget (this, shows);
				this.container.Add (embeddedWidget);

			//} else 
			//{
			//	this.Build ();
			//	List<Show> shows = GuideBoxAPIWrapper.getTVShowIds (0, 25, Source.All);
			//	embeddedWidget = new ShowResultsWidget (this, shows);
			//	this.container.Add (embeddedWidget);
			//	this.ShowAll ();

			//}

		}
	}
}