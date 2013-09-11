Perry’s ID3 Tag Library
=======================

A free open-source ID3v1 and ID3v2 tag parsing utility for MP3 files. Programmed in VB.NET, it has been tested to work with Visual Studio, VB.NET, ASP.NET, Visual Basic for Applications (VBA) and VBScript.

-Supports ID3v1, ID3v2.1, ID3v2.2, ID3v2.3, ID3v2.4.  
-Object oriented architecture - MP3File, ID3v2Tag, Frame, mp3.Artwork, etc.  
-Meticulously built from scratch in accordance with official ID3 spec.  
-Detects ALL ID3v2 frames such as those typically hidden or ignored - PRIV, etc.  
-In the works: tag writing. Insert/edit/delete ANY ID3v2 frame.  
-Future plans for: batch processing, MPEG data (duration, bitrate, etc), filemeta/online/catalog auto-tagging, custom frames.  

Perry's ID3 Tag Viewer implements the current version of the library:

![Perry's ID3 Tag Viewer](http://glassocean.net/media/id3-tag-viewer-1.jpg)

However, the Viewer is currently not available as an open-source project for now...

History
=======

This project originated as a sub-component for an older project I started in 2010, during which I only knew of a few other libraries, very few for .NET, even fewer for .NET open-source, and literally zero for VB.NET open-source.

Attempting to implement a full spec ID3 tag reader was a huge challenge for me (having no binary computer sciency knowledge at the time) but overall proved to be an incredible learning experience. Not being accustomed to open-source, it's initially difficult to share the results of this hard work so openly, but I've learned that it makes me happier to see just how useful my code might be to others. So please respect the license.

One thing that kept me working on this was a discovery made in part by myself and a russian blogger who published an article (in russian) pointing out hidden data found in MP3 files by using my ID3 Tag Viewer. It seems like some digital music retailers are embedding ownership/copyright data (e.g. your full name) in the ID3 tags using hidden PRIV or other types of encrypted frames. Sneaky and probably controversial. Read more about this here: https://plus.google.com/118266206851318889389/posts/2hscpA1fCNi
