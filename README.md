# CtrlVolume
Adjust non-default audio output via Ctrl + Volume keys

<img src = "./readme_assets/animation.gif" style="width:300px">

## Usage instructions
* Run the program for the first time. You'll select your second device, and it'll generate a save file. Close the program
* Place the program directory where windows task scheduler can access it. Configure it to start every time the pc starts.
* Start the program and you're good to go. 
* Hold down the ctrl key and adjust the volume. The 2nd device's volume should increase, but your main device volume will stay the same. This works extra nicely if your keyboard has volume buttons or a dial.

### If things get funky
* Stop the program and delete the "CtrlVolume_settings.json" file and start over setting up the program.


## Known quirks
* Your main device volume cannot be 0% if you want to lower the volume of the secondary device as the windows volume system does not go below 0%

## Update Notes
* Fixed memory leak
* Program no longer crashes if the main and secondary audio out is disconnected/disabled
* Fixed accidental muting of main audio device
* Lowered cpu usage during idle (3%-9% to 0%)
* Resolved an issue where activating the program after adjusting the volume on the default audio device would inadvertanly trigger a volume change on the secondary audio output
* Added in some safety measures to application setup to prevent the user from selecting the default audio out as the secondary too