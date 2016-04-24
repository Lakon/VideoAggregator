using System;
//using Gtk;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace VideoAggregator
{
	public partial class MainWindow: Gtk.Window
	{
		private EmbeddedWidget embeddedWidget;
		private Stack<EmbeddedWidget> previousWidgets;
		private Gtk.Label errorLabel;
		private Gdk.PixbufAnimation loadingAnimation;
		private CancellationTokenSource loadingResultsCancellationSource;

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
			backButton.Sensitive = false;
			searchButton.Sensitive = false;
			previousWidgets = new Stack<EmbeddedWidget> ();
			errorLabel = new Gtk.Label ();

			//load the loading animation
			using (Stream imgStream = GetType ().Assembly.GetManifestResourceStream ("loadingAnimation")) { 
				loadingAnimation = new Gdk.PixbufAnimation(imgStream);
			}

			//make a new CancellationSource for a new task
			loadingResultsCancellationSource = new CancellationTokenSource ();

			showLoadingScreen();
			var cancelToken = loadingResultsCancellationSource.Token;

			//Create the task that gets the results from guidebox
			Task task = new Task(() => {
				cancelToken.ThrowIfCancellationRequested();

				//get the data from guidebox
				List<Show> shows = GuideBoxAPIWrapper.getTVShowIds (0, 25, Source.All);

				cancelToken.ThrowIfCancellationRequested();

				//populate the thumbnails
				foreach(var show in shows){
					byte[] thumbNail = getThumbNail(show.thumbURL);
					if (thumbNail != null)
						show.thumb = new Gdk.Pixbuf(thumbNail);
				}

				cancelToken.ThrowIfCancellationRequested();

				//show the results
				Gtk.Application.Invoke (delegate {
					embeddedWidget = new ShowResultsWidget (this, shows);
					clearContainer();
					this.container.Add (embeddedWidget);
				} );
			},cancelToken);

			//create the delegate to handle exceptions
			task.ContinueWith((t) => {
				Gtk.Application.Invoke (delegate {
					outputError (t.Exception.InnerException.Message);
				});
			}, TaskContinuationOptions.OnlyOnFaulted);

			task.Start();

			this.ShowAll ();
		}

		private void updateBackButton(){
			if (previousWidgets.Count <= 0)
				backButton.Sensitive = false;
			else
				backButton.Sensitive = true;
		}

		//This shows a loading graphic and a message
		//it uses a table to center both items
		private void showLoadingScreen(string message = "Loading"){
			clearContainer ();

			Gtk.Table table = new Gtk.Table (((uint)(6)), ((uint)(6)), true);
			table.BorderWidth = 2;
			Gtk.Image img = new Gtk.Image (loadingAnimation);
			table.Attach (img, 2, 4, 2, 3);

			table.Attach (new Gtk.Label (message), 2, 4, 3, 4);

			container.Add(table);
			this.ShowAll ();
		}


		private byte[] getThumbNail(string url){
			byte[] imageBytes = null;
			try{
				using (var webClient = new WebClient ()) {
					imageBytes = webClient.DownloadData (url);
				}
			}catch(WebException e){
				Console.WriteLine(url);
				Console.WriteLine (e);
				return null;
			}

			return imageBytes;
		}
			

		//clears whatever is in the lower part of the container
		//if it's the embeddedWidget then it pushes it to stack
		private void clearContainer(){
			if (container.Children.Length == 3) {
				if (container.Children [2] == embeddedWidget) {
					previousWidgets.Push (embeddedWidget);
					updateBackButton ();
				}
				container.Remove (container.Children [2]);
			} else if (container.Children.Length > 3) {
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
			showLoadingScreen("Loading " + show.title);

			//make a new CancellationSource for a new task
			loadingResultsCancellationSource = new CancellationTokenSource ();
			var cancelToken = loadingResultsCancellationSource.Token;

			//Create the task that gets the results from guidebox
			Task task = new Task(() => {
				cancelToken.ThrowIfCancellationRequested();
				if (show.isMovie){
					
					//get the results from guidebox
					Dictionary<string, List<string> > sources = GuideBoxAPIWrapper.getMovieLinks (show.id);

					cancelToken.ThrowIfCancellationRequested();

					//show the results
					Gtk.Application.Invoke (delegate {
						embeddedWidget = new SourcesWidget (this, show.desc, show.thumb, sources, activeSource);
						clearContainer ();
						this.container.Add (embeddedWidget);
					});
				}
				else{
					//get the results from guidebox
					show.numOfSeasons = GuideBoxAPIWrapper.getTVShowSeasons (show.id);

					cancelToken.ThrowIfCancellationRequested();

					//show the results
					Gtk.Application.Invoke (delegate {
						embeddedWidget = new SeasonResultsWidget (this, show);
						clearContainer ();
						this.container.Add (embeddedWidget);
					});
				}
			}, cancelToken);

			//create the delegate to handle exceptions
			task.ContinueWith((t) => {
				Gtk.Application.Invoke (delegate {
					outputError (t.Exception.InnerException.Message);
				});
			}, TaskContinuationOptions.OnlyOnFaulted);

			task.Start();
				
		}


		public void seasonSelected(Show show, int s){
			showLoadingScreen ("Loading Season " + s.ToString() + " of " + show.title);

			//make a new CancellationSource for a new task
			loadingResultsCancellationSource = new CancellationTokenSource ();
			var cancelToken = loadingResultsCancellationSource.Token;

			//Create the task that gets the results from guidebox
			Task task = new Task(() => {
				cancelToken.ThrowIfCancellationRequested();

				//get the results from guidebox
				Season season = new Season(s.ToString());
				season.episodes = GuideBoxAPIWrapper.getTVShowEpisodes (show.id, s.ToString());

				cancelToken.ThrowIfCancellationRequested();

				//populate the thumbnails
				foreach(var ep in season.episodes){
					byte[] thumbNail = getThumbNail(ep.thumbURL);
					if (thumbNail != null)
						ep.thumb = new Gdk.Pixbuf(thumbNail);
				}

				cancelToken.ThrowIfCancellationRequested();

				//show the results
				Gtk.Application.Invoke (delegate {
					if (season.episodes.Count == 0){
						outputError("No available episodes");
						return;
					}
					embeddedWidget = new EpisodeResultsWidget (this, season);
					clearContainer ();
					this.container.Add (embeddedWidget);
				});
			}, cancelToken);

			//create the delegate to handle exceptions
			task.ContinueWith((t) => {
				Gtk.Application.Invoke (delegate {
					outputError (t.Exception.InnerException.Message);
				});
			}, TaskContinuationOptions.OnlyOnFaulted);

			task.Start();
		}


		public void episodeSelected(Episode episode){
			showLoadingScreen("Loading Episode " + episode.num.ToString() + ". " + episode.title);

			//make a new CancellationSource for a new task
			loadingResultsCancellationSource = new CancellationTokenSource ();
			var cancelToken = loadingResultsCancellationSource.Token;

			//Create the task that gets the results from guidebox
			Task task = new Task(() => {
				cancelToken.ThrowIfCancellationRequested();

				//get the results from guidebox
				Dictionary<string, List<string> > sources = GuideBoxAPIWrapper.getEpisodeLinks (episode.id);

				cancelToken.ThrowIfCancellationRequested();

				//show the results
				Gtk.Application.Invoke (delegate {
					embeddedWidget = new SourcesWidget (this, episode.desc, episode.thumb, sources, activeSource);

					clearContainer ();
					this.container.Add (embeddedWidget);
				});
			}, cancelToken);

			//create the delegate to handle exceptions
			task.ContinueWith((t) => {
				Gtk.Application.Invoke (delegate {
					outputError (t.Exception.InnerException.Message);
				});
			}, TaskContinuationOptions.OnlyOnFaulted);

			task.Start();
		}

		public void sourceSelected(string source, List<string> urls){


			Console.WriteLine (source);
			foreach (string url in urls) {
				Console.WriteLine (url);
			}
			if (source == "Hulu") {

				//minimize and launch Firefox
				this.Iconify ();
				//
				System.Diagnostics.Process.Start ("firefox.exe", urls [0]);
				System.Diagnostics.Process.Start ("/Applications/Firefox.app/Contents/MacOS/Firefox", urls [0]);
				System.Diagnostics.Process.Start("/usr/bin/firefox" , urls [0]);
			} 
			else {

				//minimize and launch Chrome
				this.Iconify();
				System.Diagnostics.Process.Start ("firefox.exe", urls [0]);
				System.Diagnostics.Process.Start ("/Applications/Google Chrome.app/Contents/MacOS/Google Chrome", urls [0]);
				System.Diagnostics.Process.Start("/usr/bin/firefox" , urls [0]);

			}
		}

		protected void OnDeleteEvent (object sender, Gtk.DeleteEventArgs a){
			//cancel any task that might be running
			loadingResultsCancellationSource.Cancel ();

			Gtk.Application.Quit ();
			a.RetVal = true;
		}

		protected void OnBackButtonClicked (object sender, EventArgs e)
		{
			//cancel any task that might be running
			loadingResultsCancellationSource.Cancel ();

			if (previousWidgets.Count != 0) {
				if (container.Children.Length == 3)
					container.Remove (container.Children[2]);

				embeddedWidget = previousWidgets.Pop ();

				updateBackButton ();

				container.Add (embeddedWidget);
			}
		}

		protected void OnSearchButtonClicked (object sender, EventArgs e)
		{
			string searchText = searchEntry.Text.Trim();
			if (searchText == null || searchText == "")
				return;

			//cancel any task that might be running
			loadingResultsCancellationSource.Cancel ();

			string msg = "";
			if (showRadioButton.Active)
				msg = "Searching Shows for " + searchText;
			else
				msg = "Searching Movies for " + searchText;
			showLoadingScreen(msg);

			//make a new CancellationSource for a new task
			loadingResultsCancellationSource = new CancellationTokenSource ();
			var cancelToken = loadingResultsCancellationSource.Token;

			//Create the task that gets the results from guidebox
			Task task = new Task(() => {
				cancelToken.ThrowIfCancellationRequested();
				List<Show> shows = new List<Show>();

				if (showRadioButton.Active){
					//get the results from guidebox
					shows = GuideBoxAPIWrapper.getTVShowIds (searchText);

					cancelToken.ThrowIfCancellationRequested();

					//populate the thumbnails
					foreach(var show in shows){
						byte[] thumbNail = getThumbNail(show.thumbURL);
						if (thumbNail != null)
							show.thumb = new Gdk.Pixbuf(thumbNail);
					}
				}
				else{
					//get the results from guidebox
					shows = GuideBoxAPIWrapper.getMovieIds (searchText);

					cancelToken.ThrowIfCancellationRequested();

					//populate the thumbnails
					foreach(var show in shows){
						byte[] thumbNail = getThumbNail(show.thumbURL);
						if (thumbNail != null)
							show.thumb = new Gdk.Pixbuf(thumbNail);
					}
				}

				cancelToken.ThrowIfCancellationRequested();

				//show the results
				Gtk.Application.Invoke (delegate {
					embeddedWidget = new ShowResultsWidget (this, shows);
					clearContainer ();
					this.container.Add (embeddedWidget);
				});
			}, cancelToken);


			//create the delegate to handle exceptions
			task.ContinueWith((t) => {
				Gtk.Application.Invoke (delegate {
					outputError (t.Exception.InnerException.Message);
				});
			}, TaskContinuationOptions.OnlyOnFaulted);

			task.Start();
		}

		protected void OnPopButtonClicked (object sender, EventArgs e)
		{
			//cancel any task that might be running
			loadingResultsCancellationSource.Cancel ();
			string msg = "";
			if (showRadioButton.Active)
				msg = "Searching for Popular Shows";
			else
				msg = "Searching for Popular Movies";
			showLoadingScreen(msg);

			//make a new CancellationSource for a new task
			loadingResultsCancellationSource = new CancellationTokenSource ();
			var cancelToken = loadingResultsCancellationSource.Token;

			//Create the task that gets the results from guidebox
			Task task = new Task(() => {
				cancelToken.ThrowIfCancellationRequested();
				List<Show> shows = new List<Show>();
				if (showRadioButton.Active){
					//get the results from guidebox
					shows = GuideBoxAPIWrapper.getTVShowIds (0, 25, activeSource);

					cancelToken.ThrowIfCancellationRequested();

					//populate the thumbnails
					foreach(var show in shows){
						byte[] thumbNail = getThumbNail(show.thumbURL);
						if (thumbNail != null)
							show.thumb = new Gdk.Pixbuf(thumbNail);
					}
				}
				else{
					//get the results from guidebox
					shows = GuideBoxAPIWrapper.getMovieIds (0, 25, activeSource);

					cancelToken.ThrowIfCancellationRequested();

					//populate the thumbnails
					foreach(var show in shows){
						byte[] thumbNail = getThumbNail(show.thumbURL);
						if (thumbNail != null)
							show.thumb = new Gdk.Pixbuf(thumbNail);
					}
				}
				cancelToken.ThrowIfCancellationRequested();

				//show the results
				Gtk.Application.Invoke (delegate {
					embeddedWidget = new ShowResultsWidget (this, shows);
					clearContainer ();
					this.container.Add (embeddedWidget);
				});
			}, cancelToken);

			//create the delegate to handle exceptions
			task.ContinueWith((t) => {
				Gtk.Application.Invoke (delegate {
					outputError (t.Exception.InnerException.Message);
				});
			}, TaskContinuationOptions.OnlyOnFaulted);

			task.Start();
		}

		protected void OnSourceChanged (object sender, EventArgs e)
		{
			embeddedWidget.OnSourceChanged (activeSource);
		}

		protected void OnSearchEntryChanged (object sender, EventArgs e)
		{
			string searchText = searchEntry.Text.Trim();
			if (searchText != null && searchText != "")
				searchButton.Sensitive = true;
			else
				searchButton.Sensitive = false;
		}

		protected void OnAboutSelected (object sender, EventArgs e)
		{
			// Create a new About dialog
			Gtk.AboutDialog about = new Gtk.AboutDialog();

			// Change the Dialog's properties to the appropriate values.
			about.ProgramName = "Team Raspberry's Video Aggregator";
			about.Authors = new string[]{ "Michael Hendrick", "Thomas Martin", "Elizabeth Razo" };
			about.Comments = "UHCL Senior Project Spring 2016.\nPowered by Gtk# and Guidebox.";
			about.Version = "1.0.0";

			// Show the Dialog and pass it control
			about.Run();

			// Destroy the dialog
			about.Destroy();
		}

		protected void OnBrowserSetUpSelected (object sender, EventArgs e)
		{

		}
	}
}