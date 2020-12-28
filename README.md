# BMS Burner

A stupid app that plays a stupid sound when you light your stupid afterburners.

## Why?

https://www.benchmarksims.org/forum/showthread.php?40697-Afterburner-sound-as-an-indication

## How do I use it?

1. Set an afterburner detent point in the
   [BMS Alternate Launcher](https://github.com/chihirobelmo/FalconBMS-Alternative-Launcher).
   (This stupid app currently reads settings from the Alt Launcher's XML files.
   Not depending on them is the #1 TODO.)

2. Open this piece of junk, browse to your BMS directory.
   It should read the Alt Launcher config filesr, find which device
   (and which axis) is bound to your throttle, and find your AB detent point.

3. Hear `blowers.ogg` whenever you cross from military power into afterburners.

## Contributing

Because .NET apparently only believes in .wav files
(https://docs.microsoft.com/en-us/dotnet/api/system.media.soundplayer)
and the author was too lazy to link up some ffmpeg bindings,
sounds are played with mpv. Grab a Windows build from
https://sourceforge.net/projects/mpv-player-windows/files/
and shove it in the working directory.

## TODO

If you want to make this slightly less stupid, you could

- [x] Read the throttle axis and detent settings out of BMS's `axismapping.dat`
      directly so this doesn't depend on the Alternate Launcher.

- [x] Read the BMS shared memory to only play sounds when you're in the pit.

- [x] Find the BMS directory automatically (registry entries?)

- [ ] Add options to the overlay window
  - Make it draggable
  - Add color customization

- [ ] Make the sound to play configurable (instead of a hardcoded `burners-on/off.ogg`)

- [ ] Add an option to play a sound continuously while the burners are on

- [ ] Come up with a less stupid way to play sound
