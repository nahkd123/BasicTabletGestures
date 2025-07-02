# Basic Tablet Gestures

Basic gesture plugin for touch-enabled pen tablets, such as PTH-x60, CTH-x80
and the likes.

## Installation

### From source

1. Clone this repository.
1. Build plugin: `dotnet build --configuration=Release`.
1. Copy DLL from `./bin/Release/net8.0/nahkd123.BasicTabletGestures.dll` to OTD
   plugin folder.
1. Restart OpenTabletDriver.

### Prebuilt binaries

1. Download `nahkd123.BasicTabletGestures.dll` in [Releases tab][releases].
1. Create a new folder inside OTD's plugins folder and move the downloaded file
   in that folder.

   - For Windows: Create a folder in `%localappdata%\OpenTabletDriver\Plugins\`
	 and move the `.dll` file to that newly created folder.

1. Restart OpenTabletDriver.

## Configuring filters

### Basic Tablet Tap Gestures

This filter add basic tap gestures to your tablet that perform certain key
binding when tapping on tablet surface with specific number of fingers.

- Touch deadline (ms): How long does it takes for filter to register your tap.
  When tapping on your pen tablet, the tap will only be registered when one of
  the following happened:

  1. You released all fingers before deadline, or;
  1. The filter reached deadline before releasing all fingers.

- Pen ignore time (ms): How long should the filter ignore all touch inputs
  after the pen leave the tablet's sensing area. If you keep triggering
  gesture, try increasing this value. Negative value will disable this feature
  (which allow you to trigger touch while the pen is in tablet's sensing area)

- N-finger tap action: A concatenated sequence of keys, each separated by a
  plus (`+`) character. `Ctrl` or `ctrl` will be translated to `Control`,
  `shift` will be translated to `Shift` and `alt` to `Alt`. The keys are
  **case-sensitive**. The list of all possible keys can be found in
  OpenTabletDriver's keybind editor (when setting up binding for pen/aux
  buttons). Examples are:

  - `Control + Z`: Undo;
  - `Ctrl + Shift + Z`: Redo;
  - `D1 + D2 + D3`: Press 1, 2 and 3 in number key row at the same time;
  - `Numpad5`: Press 5 in numpad;

[releases]: https://github.com/nahkd123/BasicTabletGestures/releases