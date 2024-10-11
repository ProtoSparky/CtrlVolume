# <img src="/readme_assets/icon.png" style="width:50px"> CtrlVolume
Adjust non-default audio output via Ctrl + Volume keys

<img src = "./readme_assets/animation.gif" style="width:300px">

## Usage instructions
* Place the program directory where windows task scheduler can access it. Configure it to start every time the pc starts.
* Start the program, go through the setup, and you're good to go. 
* Hold down the Ctrl key and adjust the volume. The 2nd device's volume should increase, but your main device volume will stay the same. This works extra nicely if your keyboard has volume buttons or a dial.

### If things get funky
* Stop the program and delete the "CtrlVolume_settings.json" file and start over setting up the program.


## Known quirks
* You cannot lower the volume if the main device volume is at 0%. The same goes for the opposite, you cannot increase the secondary device volume if the main one is above 99%. 

## Update Notes
#### v1.1.0-beta
* Added icons
* Rewrote the setup to use GUI environment
* Switched from command line to GUI environment
* Setup now checks if there are any permission errors when creating settings file
* Added notification icon, so background program can now be shutdown safely without the use of task manager
* Improved code readability
* Removed unused functions -> decreased memory and storage footprint
* Added feature to view and edit settings file
* Added error handling for when settings file exists but has no contents

#### v1.0.1-alpha
* Fixed memory leak
* Program no longer crashes if the main and secondary audio out is disconnected/disabled
* Fixed accidental muting of main audio device
* Lowered CPU usage during idle (3%-9% to 0%)
* Resolved an issue where activating the program after adjusting the volume on the default audio device would inadvertently trigger a volume change on the secondary audio output
* Added in some safety measures to application setup to prevent the user from selecting the default audio out as the secondary too

#### v1.0.0-alpha
* added the base software