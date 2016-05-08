# VideoAggregator
## UHCL Spring 2016 Senior Project
### Team Members:
* Thomas Martin
* Michael Hendrick
* Elizabeth Razo

### Description
The video aggregator is a GUI application that gets show and movie information/links from the sources YouTube, Hulu, 
and Amazon. It simply links through to the source site and doesn't handle any streaming or video playback.

The video aggregator is powered by [Mono](http://www.mono-project.com/), an open source implementation of Microsoft's .NET 
Framework, and [GTK#](http://www.mono-project.com/docs/gui/gtksharp/), a Graphical User Interface Toolkit for mono and .Net.
These make the video aggregator cross-platform.

It uses the [GuideBox API](http://www.guidebox.com/) to get show information/links.

![VideoAggregatorExample](https://github.com/Lakon/VideoAggregator/blob/master/VideoAggregatorExample.png)

### Installation/Build Instructions
The video aggregator requires [Mono](http://www.mono-project.com/) and [GTK#](http://www.mono-project.com/docs/gui/gtksharp/) 
to run. We used [Xamarin Studio](https://www.xamarin.com/studio) to compile, though [MonoDevelop](http://www.monodevelop.com/) 
works as well. All the information needed to install one of these IDEs is [here](http://www.monodevelop.com/download/).

Note that just recently Xamarin Studio was bought by Microsoft and the download page directs you to Visual Studio.

### Usage
The video aggregator was designed to be easy to use. You simply click on buttons and results and the application will load new
results. There is a search function and a back function.

When you click on a source a browser will open at the url of the source

