# VA-Datatape for DCS F-16C Viper
Voice Attack Datatape for the DCS F-16C Viper

## New Features in 0.0.4
I have added a few new features and commands that will make things even better:
1) You can comment out individual rows in the datatape file be prepending it with a `#`
2) You can insert inline comments on the end of any line surrounded by `[]`
3) Multi file support - You can create and call up to 13 different files with semi-custom names in the format
`datatape_<alpha|bravo|charlie|delta|echo|foxtrot|golf|hotel|india|juliet|kilo|lima|mike>_<ANYTHING_YOU_WANT>.txt`
  * Examples
    * datatape_alpha_pattern_practice.txt
    * datatape_bravo_bombing_run.txt
    * datatape_charlie_ILS_practice.txt
4) You can dynamically edit the datatape text files and recall them without restarting VA or DCS

## Installation
All you need to do to use it is:

1) download the .vab file and import it into Voice Attack. If you already have a voice attack profile, you can either import this into that profile or you can import it as a separate profile and link them. (VoiceAttack README explains how to do this - https://voiceattack.com/howto.aspx)
2) Create a coordinate file in one of the following locations (example file included in repo)
  * C:\Users\YOUR_USER\Saved Games\DCS.Openbeta\Datatape\datatape.txt
  * C:\Users\YOUR_USER\Saved Games\DCS\Datatape\datatape.txt
  * C:\Program Files (x86)\VoiceAttack\Datatape\datatape.txt

3) If you are already using an existing profile. Configure your existing profile to include the DataTape profile by:
 * Click "Edit Profile" (Alt + E) on your existing profile
 * Next to the profile name, click on "Options"

4) Use one of the two voice commands:
  * "Load Datatape" - loads all steerpoints in the file
  * "Load Datatape waypoint X" - loads only steerpoint X from the file
  * In the popup window, click the "..." button next to "Include Commands from other Profiles"
  * In the next popup, click the plus sign
  * Select the "Datatype vX.X.X" profile
  * Click OK on all pop up windows
  * Click "Apply" in the bottom right of the options window
 
## Usage
There are 4 total voice commands:
* `Load Datatape` - This command will load all uncommented steerpoints the default `datatape_txt` file
* `Load Datatape Steerpoint <NUMBER>` - This command will only load steerpoint X (example 3) from the default `datatape_txt` file
* `Load Datatape file <alpha|bravo|charlie|delta|echo|foxtrot|golf|hotel|india|juliet|kilo|lima|mike>` - This command will load all steerpoints any file matching the name you called. For example if you say "load datatape file bravo" it will look for a file named `datatape_bravo_*.txt`. You can customize the name of the file as long as you leave the first part the way it needs to be
* `Load Datatape file <alpha|bravo|charlie|delta|echo|foxtrot|golf|hotel|india|juliet|kilo|lima|mike> Steerpoint <NUMBER>` - This command will load only the specific steerpoint from any file matching the name you called. 

  
## Notes
* inline.vb file listed here is for informational purposes to show you the inner workings of the script. When you import the .vab file, the inline file is pre-loaded. 
* This profiles only works for the F-16C Viper at this time. More aircraft can be added, time permitting. 
* **This profile can cause conflict with heavily modified profiles and will almost certainly have issues if you have remapped your ICP buttons to use anything other than "L Ctrl + NumPad X"**


## Credits/Shoutouts:
* This module uses components from "DCS F-16C Viper VoiceAttack by Bailey v2.0.0" found at https://www.digitalcombatsimulator.com/en/files/3306505/


## License 
Apache 2.0 License (do what you want with it, just make sure to give me and Bailey credit)
