# Pixelborne

This game has been programmed in the seminar Game Programming from the computer graphics research group from the HPI.
Our supervisor was [Willy Scheibel](https://github.com/scheibel). 


This computer game is a 2D souls-like side scroller with a multiplayer and singleplayer mode. 
The singleplayer involves a story.

The fighting system is inspired by [Nidhogg](http://nidhogggame.com/).

## Game Programming
The seminar was about programming a game and integrating a critical component into the game.
A critical component is something that does something morally wrong by abusing technology.
This reaches from spying on the user to discrimination of any kind.
This component should somehow point it out to the user and make him aware of what it is doing.
Thus the user will hopefully learn something about technology and what it is actually capable of.

We decided to spy on the user in various ways and show him what we found in a subtle way:
- We are searching the users music folder for mp3 files and play them silently 
over the game music in a random order.
- We are looking for pictures in the pictures folder of the user and blend them over 
the existing images in the game.
- We are taking a photo with the webcam and record 10 seconds of audio
when the player accidentally hits certain wrong keys or buttons depending on the used controller.


## Requirements and Setup
The game is made with [Unity](https://unity3d.com/get-unity/download/archive) 2018.4.12f1.

We are using a VS-Plugin [NAudio](https://github.com/naudio/NAudio) in order to read mp3 files.
The plugin should be in the plugin folder and thus should not need further setup.

You can build the game by yourself in Unity or directly take one of the compiled versions from the bin folder.

To build the project yourself, open Unity in the root directory of this project 
and click File->Build And Run.

## Documentation
Find the start page - *index.html* - of the documentation in Documentation/html/.

Or read the *refman.pdf* which can be found under Documentation/latex.

Compilation of the latex version can be done by executing the *Makefile* or the *make.bat* depending on your operation system.

## Scripts
Our scripts can be found in Assets/Scripts.

## Links to 3rd-Party Assets and Scripts that we use
Throughout the project we are using some 3rd party scripts and free assets. 
We manipulated the assets to our needs.

##### Scripts
- [SavWav](http://forum.unity3d.com/threads/119295-Writing-AudioListener.GetOutputData-to-wav-problem?p=806734&viewfull=1#post806734)
for converting a Unity AudioClip to wav and save it to the disk.
- [NAudioPlayer](https://gamedev.stackexchange.com/questions/114885/how-do-i-play-mp3-files-in-unity-standalone)
- We used small code snippets from other sources as well. Please read the docs.

##### Assets
- [Background / Stage](https://assetstore.unity.com/packages/2d/environments/pixel-dark-forest-136825)
- [Enemies](https://assetstore.unity.com/packages/2d/characters/hero-nad-opponents-animation-140776)
- [Dark King](https://assetstore.unity.com/packages/2d/characters/bandits-pixel-art-104130)
- [Player](https://ramirov.itch.io/vai-drogul)
- [Crystal](https://assetstore.unity.com/packages/2d/gui/icons/crystals-collection-42748)

## Known Issues
- possible sharing violation on AudioFilePaths.txt
- possible sharing violation on ImportantDocuments.txt