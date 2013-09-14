Perry’s ID3 Tag Library
=======================

A free open-source ID3v1 and ID3v2 tag parsing utility for MP3 files. Programmed in VB.NET, it has been tested to work with Visual Studio, VB.NET, ASP.NET, Visual Basic for Applications (VBA) and VBScript.

* Supports ID3v1, ID3v2.1, ID3v2.2, ID3v2.3, ID3v2.4.  
* Object oriented architecture - MP3File, ID3v2Tag, Frame, mp3.Artwork, etc.  
* Meticulously built from scratch in accordance with official ID3 spec.  
* Detects ALL ID3v2 frames such as those typically hidden or ignored - PRIV, etc.  

![Class Diagrams](http://files.glassocean.net/github/id3taglibrary1.jpg)

Perry's ID3 Tag Viewer implements the current version of the library:

![Perry's ID3 Tag Viewer](http://glassocean.net/media/id3-tag-viewer-1.jpg)

However, the Viewer is currently not available as an open-source project for now...

Roadmap
-------

**Tag writing**

First prototype of direct ID3v2 frame data editing/saving has been partially implemented and tested, which involves  splitting and rejoining the file if the saved frame data is larger than the current frame size + padding. This should also include reading/writing custom frame types.

The Tag Viewer should also implement a simple interface for making these types of edits, preferably straight from the DataGrids.

**Batch processing**

A feature for the Tag Viewer; this would allow batch processing of mp3 files, such as changing the artwork for an entire album, removing hidden PRIV frames from several albums, or renaming a selection of files based on their filenames/tag data.

**MPEG header parsing**

First prototype has been developed. Will search the mp3 file for MPEG header frames starting AFTER the ID3v2 tag since many false sync signals can be detected in the ID3v2 tag which causes excessive processing since each sync signal must be evaluated as to whether or not they are true MPEG header frames.

Utilizes a well known algorithm[[1]](#references).

**Auto-tag & auto-fix**

Automatically fill tag data (artist, album, track, artwork, etc) from online sources (freedb, Amazon, etc).

**Windows Media Player album artwork fix**

Trying to maintain a song library in Windows Media Player can be frustrating.

References
----------

[1] [MPEG AUDIO HEADER FRAME](http://www.mpgedit.org/mpgedit/mpeg_format/mpeghdr.htm)
[2] [Help, I am going INSANE! WMP 12 album art.](http://social.technet.microsoft.com/Forums/windows/en-US/e6ee46cc-f088-4847-a9a2-58fac6888407/help-i-am-going-insane-wmp12-album-art)

[4] [](https://plus.google.com/118266206851318889389/posts/2hscpA1fCNi)

History
-------

This project originated as a sub-component for an older project I started in 2010, during which I only knew of a few other libraries, very few for .NET, even fewer for .NET open-source, and literally zero for VB.NET open-source.

Attempting to implement a full spec ID3 tag reader was a huge challenge for me (having no binary computer sciency knowledge at the time) but overall proved to be an incredible learning experience. Not being accustomed to open-source, it's initially difficult to share the results of this hard work so openly, but I've learned that it makes me happier to see just how useful my code might be to others. So please respect the license.

One thing that inspired me to keep me working on this project was an interesting discovery made in part by myself and a Russian blogger who published an article (in Russian) pointing out hidden data found in MP3 files by using my ID3 Tag Viewer. It seems like some digital music retailers like to embed ownership/copyright data (e.g. your full name) in the ID3 tags using hidden PRIV or other types of encrypted frames. Sneaky and probably very controversial [[4]](#references).
