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

PRIV Frame Abuse
----------------



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

Trying to maintain a song library in Windows Media Player can be frustrating, and here's why...

The official ID3 spec treats artwork on a *per-song* basis. Every mp3 file in a folder or album can have it’s own unique artwork embedded in the mp3 file itself so that it’s portable without needing other jpg files, and software such as WinAmp will display the artwork for each mp3 file properly. However, Windows Media Player treats artwork differently, by storing the artwork as hidden jpg files in the same folder as the mp3 files.

Windows Media Player 7 names the hidden jpg files **AlbumArtSmall.jpg**, **AlbumArtLarge.jpg** and **Folder.jpg**. You can only have one set of these in a folder, hence one set of artwork per folder. What if you organized your music in such a way where you had a folder containing hit singles from several different artists? Windows Media Player ends up using the artwork from a single mp3 file for the entire set of mp3 files in that folder.

Windows Media Player 8/9/10/11/12 names the hidden jpg files **AlbumArt_{XXX}_Small.jpg**, **AlbumArt_{XXX}_Large.jpg** and **Folder.jpg**, where XXX is a GUID (long string of text). But what determines these GUIDs, and what is their purpose? Windows Media Player creates the GUIDs to give every single song or album a unique identity in their online database. When playing mp3 files, Windows Media Player can retrieve song information automatically from the online database, which is great if the mp3 files themselves don’t have complete song information stored in the embedded ID3 tags. However, Windows Media Player will also automatically update your mp3 files with this song information retrieved from the online database (an option that is enabled by default) and this data can be inaccurate.

Since the online content is submitted by users like yourself, a small portion of the content in this database contains bad GUIDs for certain songs or albums. The trouble happens when Windows Media Player retrieves a bad GUID from the database, particularly a GUID of {00000000-0000-0000-0000-000000000000}. When this happens, Windows Media Player will generate the album artwork (hidden jpg files) with names consisting of **AlbumArt_{00000000-0000-0000-0000-000000000000}_Small.jpg** and **AlbumArt_{00000000-0000-0000-0000-000000000000}_Large.jpg**, no longer a unique identity.

As was already explained in the above note about Windows Media Player 7, what ends up happening is that you can only have one set of artwork in a folder, thus Windows Media Player ends up using the artwork from a single song or album for the entire set of mp3 files in that folder, and it will overwrite the embedded artwork in those mp3 files, severely damaging your music collection.

To make matters worse, Windows Media Player seems to store the GUID inside of a PRIV frame in the ID3 tag embedded in the mp3 file, so even if you try fixing the embedded artwork, Windows Media Player will see the GUID in the PRIV frame and regenerate the hidden jpg files. The problem spreads around from person to person, likely finds its way back into the online database, and never truly gets fixed because users don’t have a way of correcting mp3 files with bad GUIDs stored in PRIV frames of the ID3 tags:

![Class Diagrams](http://files.glassocean.net/github/id3taglibrary1.jpg)

Users of Microsoft Windows XP/Vista/7 have no doubt seen these before:

![artwork as hidden jpg files](http://files.glassocean.net/github/id3taglibrary2.jpg)

You're bound for trouble when the GUID is all zeros!

Perry's ID3 Tag Library (and Tag Viewer) can detect this problem, and future versions with tag writing capabilities should allow users to fix it. This won't help fix the online database, but at least users will have a way to fix their own music collections by removing the PRIV frames so Windows Media Player stops generating the artwork (jpg files) with bad GUIDs [[2]](#references).

References
----------

[1] [MPEG AUDIO HEADER FRAME](http://www.mpgedit.org/mpgedit/mpeg_format/mpeghdr.htm)  
[2] [Help, I am going INSANE! WMP 12 album art.](http://social.technet.microsoft.com/Forums/windows/en-US/e6ee46cc-f088-4847-a9a2-58fac6888407/help-i-am-going-insane-wmp12-album-art)  
[3] [«Защита» mp3 файлов на amazon.com / Информационная безопасность / Хабрахабр](http://habrahabr.ru/post/134523/)  

History
-------

This project originated as a sub-component for an older project I started in 2010, during which I only knew of a few other libraries, very few for .NET, even fewer for .NET open-source, and literally zero for VB.NET open-source.

Attempting to implement a full spec ID3 tag reader was a huge challenge for me (having no binary computer sciency knowledge at the time) but overall proved to be an incredible learning experience. Not being accustomed to open-source, it's initially difficult to share the results of this hard work so openly, but I've learned that it makes me happier to see just how useful my code might be to others. So please respect the license.

One thing that inspired me to keep me working on this project was an interesting discovery made in part by myself and a Russian blogger who published an article (in Russian) pointing out how digital music retailers such as Amazon use a type of audio steganography to store unique purchase identifiers (information about who purchased the music, with a full name/user account id linked to a purchase id) in the mp3 files by embedding the data inside PRIV frames of the ID3 tags which are ignored by nearly all music software [[4]](#references).
