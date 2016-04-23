﻿using System;
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
		private Gtk.Image loadingAnimation;
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
			previousWidgets = new Stack<EmbeddedWidget> ();
			errorLabel = new Gtk.Label ();

			//load the loading animation
			using (Stream imgStream = GetType ().Assembly.GetManifestResourceStream ("loadingAnimation")) { 
				loadingAnimation = new Gtk.Image (new Gdk.PixbufAnimation(imgStream));
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

		private void showLoadingScreen(){
			clearContainer ();
			container.Add (loadingAnimation);
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
			showLoadingScreen();

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
			showLoadingScreen();

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
			showLoadingScreen();

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
				System.Diagnostics.Process.Start ("/Applications/Firefox.app/Contents/MacOS/Firefox", urls [0]);} 
			else {
				System.Diagnostics.Process.Start ("/Applications/Google Chrome.app/Contents/MacOS/Google Chrome", urls [0]);}
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
				if (container.Children.Length == 2)
					container.Remove (container.Children[1]);

				embeddedWidget = previousWidgets.Pop ();

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
			showLoadingScreen();

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
			showLoadingScreen();

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
	}
}