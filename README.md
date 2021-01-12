# Nanoswarm-Hive
This is a collection of fixes for RA3, also includes cnc-online connection functionality

## Setup
The solution contains 2 projects: NanoswarmHive and Nanocore. NanoswarmHive is the main executable, it launches the game and injects Nanocore via EasyHook. The launcher executable will pass any parameters to the game.

### Nanocore
The Nanohook class is the main entry point of the injection. Fixes will be placed in the RA3 folder.

### Current fixes

* ContainFix: There's a nullpointer derefence if an object with a weapon has a contain and the player is trying to force fire outside the weapon range.
