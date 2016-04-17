using System;
using Gtk;
using System.Collections.Generic;
using System.Net;

namespace VideoAggregator
{
	public partial class MainWindow: Gtk.Window
	{
		private Gtk.Widget embeddedWidget;
		private Stack<Gtk.Widget> previousWidgets;
		private Gtk.Label errorLabel;

		private Source activeSource {
			get {
				switch (this.sourceComboBox.Active) {
				case 0:
					return Source.All;
				case 1:
					return Source.Hulu;
				case 2:
					return Source.Amazon;
				case 3:
					return Source.YouTube;
				default:
					return Source.All;
				}
			}
		}

		public MainWindow () : base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
			previousWidgets = new Stack<Gtk.Widget> ();
			errorLabel = new Gtk.Label ();
			try{
				List<Show> shows = GuideBoxAPIWrapper.getTVShowIds (0, 25, Source.All);
				embeddedWidget = new ShowResultsWidget (this, shows);
				this.container.Add (embeddedWidget);

			}catch(WebException e){
				outputError (e.Message);
			}

			this.ShowAll ();
		}
			

		//clears whatever is in the lower part of the container
		//if it's the embeddedWidget then it pushes it to stack
		private void clearContainer(){
			if (container.Children.Length == 2) {
				if (container.Children [1] == embeddedWidget) {
					previousWidgets.Push (embeddedWidget);
				}
				container.Remove (container.Children [1]);
			} else if (container.Children.Length > 2) {
				Console.WriteLine ("Somethings wrong");
			}
		}

		//puts a label with error message in container
		public void outputError(string errorMessage){
			clearContainer ();

			errorLabel.Text = errorMessage;
			this.container.Add (errorLabel);
			this.ShowAll ();
		}

		public void showSelected(Show show){
			clearContainer ();
			try{
				if (show.isMovie){
					Dictionary<string, List<string> > sources = GuideBoxAPIWrapper.getMovieLinks (show.id);
					embeddedWidget = new SourcesWidget (this, sources);
				}
				else{
					show.numOfSeasons = GuideBoxAPIWrapper.getTVShowSeasons (show.id);
					embeddedWidget = new SeasonResultsWidget (this, show);
				}

				this.container.Add (embeddedWidget);

			}catch(WebException e){
				outputError (e.Message);
			}
		}


		public void seasonSelected(Show show, int s){
			clearContainer ();
			try{
				Season season = new Season(s.ToString());
				season.episodes = GuideBoxAPIWrapper.getTVShowEpisodes (show.id, s.ToString());
				embeddedWidget = new EpisodeResultsWidget (this, season);

				this.container.Add (embeddedWidget);

			}catch(WebException e){
				outputError (e.Message);
			}
		}


		public void episodeSelected(Episode episode){
			clearContainer ();
			try{
				Dictionary<string, List<string> > sources = GuideBoxAPIWrapper.getEpisodeLinks (episode.id);
				embeddedWidget = new SourcesWidget (this, sources);

				this.container.Add (embeddedWidget);

			}catch(WebException e){
				outputError (e.Message);
			}
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

		protected void OnDeleteEvent (object sender, DeleteEventArgs a){
			Application.Quit ();
			a.RetVal = true;
		}

		protected void OnBackButtonClicked (object sender, EventArgs e)
		{
			if (previousWidgets.Count != 0) {
				if (container.Children.Length == 2)
					container.Remove (container.Children[1]);

				embeddedWidget = previousWidgets.Pop ();

				container.Add (embeddedWidget);
			}
		}

		protected void OnSearchButtonClicked (object sender, EventArgs e)
		{
			string searchText = searchEntry.Text.Trim();

			if (searchText != null && searchText != "") {
				clearContainer ();
				try{
					if (showRadioButton.Active){
						List<Show> shows = GuideBoxAPIWrapper.getTVShowIds (searchText);
						embeddedWidget = new ShowResultsWidget (this, shows);
					}
					else{
						List<Show> shows = GuideBoxAPIWrapper.getMovieIds (searchText);
						embeddedWidget = new ShowResultsWidget (this, shows);
					}

					this.container.Add (embeddedWidget);

				}catch(WebException exception){
					outputError (exception.Message);
				}
			}
		}

		protected void OnPopButtonClicked (object sender, EventArgs e)
		{
			clearContainer ();
			try{
				if (showRadioButton.Active){
					List<Show> shows = GuideBoxAPIWrapper.getTVShowIds (0, 25, activeSource);
					embeddedWidget = new ShowResultsWidget (this, shows);
				}
				else{
					List<Show> shows = GuideBoxAPIWrapper.getMovieIds (0, 25, activeSource);
					embeddedWidget = new ShowResultsWidget (this, shows);
				}

				this.container.Add (embeddedWidget);

			}catch(WebException exception){
				outputError (exception.Message);
			}
		}
	}
}