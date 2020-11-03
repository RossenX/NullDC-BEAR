# NullDC Netplay Launcher "BEAR" (Beat Em All Raw)
A nullDC ALL-IN-ONE Launcher and optimizer to ensure easy setup for nullDC(Naomi/Atomidwave) and optimized settings for a smooth gameplay experiance.
Easy setup easy connection, minimal effort maximum performance.

## Setup Guide

### Downloading
Click "Releases" and download the newest NullDC.BEAR.exe You do not need any of the other files they're just the source code.

![Downlaoding](REARME%20Files/Downloading.gif?raw=true "Downlaoding")

### Installing
Put BEAR in an empty folder

![Setup](REARME%20Files/Setup.gif?raw=true "Setup")

### Controls
1. Click the Controls Button

![Controls01](REARME%20Files/Controls01.png?raw=true "Controls01")

2. Choose your Controller from the list

![Controller](REARME%20Files/Controller.png?raw=true "Controller")

3. Move your stick around and check which buttons light up GREEN

![GreenButtons](REARME%20Files/GreenButtons.gif?raw=true "GreenButtons")

4. If Non Light up Green or some buttons on your controller do not work. click "Remap Controller"

![GreenButtons](REARME%20Files/RemapController.png?raw=true "GreenButtons")
4a. If the bottom of the "Remap Controller" window says Buttons: (-1) or Buttons: (0 0). Cancel it and change the SDL version.

![SDL_Version](REARME%20Files/SDL_Version.png?raw=true "SDL_Version")

5. If they want to change any button just click the button you want to change, it'll turn RED. Then press the button on your controller you want to bind it to.

![ReBinding](REARME%20Files/ReBinding.gif?raw=true "ReBinding")

## VPN Setup
You can use any VPN software that simulates a LAN Hamachi/Zerotier/RadminVPN
Most people use RadminVPN because it's the easiest to setup so that's what i'll be showing you.
1. Download RadminVPN: https://www.radmin-vpn.com/
2. When it's installed you'll have a little blue shield icon on your desktop.
3. Join a Network in RadminVPN

![JoinNetwork01](REARME%20Files/JoinNetwork01.png?raw=true "JoinNetwork01")

4. Radmin has (As of right now) two nullDC public gaming networks. You can join any one of them.

![JoinNetwork02](REARME%20Files/JoinNetwork02.png?raw=true "JoinNetwork02")

4a. Optionally you can create your own network if you want to just play with your friend or if the Gaming Network seems laggy tha day.

![JoinNetwork02](REARME%20Files/CreateNetwork.png?raw=true "JoinNetwork02")

4b. Just create any network name/pass you want and give your friend the name/pass and they can join a private network.

## Usage Guide

### Getting Games
To get a game click the "FreeDLC" button and check which games are available right now.

### Playing Online
1. To Challange someone double click their name on the list of players or click their name and then the "Challange" button.
2. Choose the game you want to challange someone to from the list and click Challange. If they accept you'll be taken to the host panel.
2a. If it says "Romname or game mismatch" that means the other person does not have the game you're trying to challange them to. OR they have a different rom than the one you are using. Both players must have the identical rom for netplay to work. I suggest always getting them from the freeDLC panel.
3. When they accept you will be taken to the host panel.

![HostPanel](REARME%20Files/HostPanel.png?raw=true "HostPanel")

Here you'll be given a couple of options.
3a. "Delay" is how much delay you should have between you and the other person. The higher the ping the higher the delay required to play. If you get choppy gameplay or very low FPS, set the Delay Higher.
Usually the "Suggest" button will do a fair job at estimating how much the delay should be.
3b. You can also choose the game Region, some games require a specific region to work because they were never released outside of that Region. For example most of the Guilty Gear games will not work in the USA region since they were never released in the USA. Other games like (Marvel Vs Capcom 2 or (Naomi)Capvom Vs SNK 2 will change the in-game language depending on which region you choose.
If you get an error saying "Couldn't ping the player" that means they are being blocked by their Firewall. Ether Click the big green button in Options to add windows firewall settings or have them allow BEAR and nulldc through whatever firewall software they are using.

### Spectating Others
1. To Spectate someone double click their name on the list of players or click their name and then the "Challange" button.
2. Spectating will start from the begining of the player's session.
3. You can fastforward by pressing w.e button you have bound to "Right"(Naomi) or D-pad Right(Dreamcast Controller) or Stick Right(Dreamcast Arcade Stick)
3a. If you have software that limits FPS (If you set the FPS limit in Nvidia for example) it will not go faster than w.e you have set in it. For fast-forward to work you need to have an uncapped frame-rate.

### Playing Offline
Click "Play Offline" and Choose your game from the list. You can also simulate delay to practise for online while while still being offline.

## FAQ (Or more correctly things most frequently yelled at me)

Q: Where to i put this in Fightcade?  
A: BEAR and Fightcade are different programs, Don't mix their files and stuff or you might mess them both up.

Q: I only see myself in the list.  
A: Click the big green button in Options to add windows firewall exception for BEAR/NullDC. VPN software like nordVPN can also interfeer with Radmin and cause it to be unable to establish a connection to others.

Q: My <button> doesn't work.  
A: Click "Remap Controller" and follow the instructions. If your button is not recognized durring the Remapping window, change SDL version. (See Controls above)

Q: My buttons work but up left and right is down and they're all messed up WTF.  
A: Rebind them, check controls above.

Q: I can join other people's games but they can't join when i host.  
A: Firewall is blocking incoming connections. Click the big green button in Options to add windows firewall exception or check whatever firewall software you have.

Q: Game runs really slow Online  
A: The host has the delay set too low

Q: The emulator instantly crashes when i try to play.  
A: You're missing one of the rerequired files below. (Check below)

Q: <GAME NAME> Doesn't work!  
A: Not all games work but if you come across one that doens't work let me know i might be able to fix it in the next update.

Q: The freeDLC menu only has like 2 games.  
A: That might rarely happen since it has to get a list of hundreds of games, close the freeDLC window and open it again.

Q: <Game> says that i'm out of memory on my VMU what do i do?  
A: Press ABXY + Start to go into the dreamcast menu and then go to memory card and delete w.e games you don't want to play. BEAR comes with an almost full VMU(Memory card) with lot of saves for fighting games mostly, you can get rid of any you don't want to use.

Q: Oh man i love your project is there any way i can support you?  
A: Of course! Just click the patreon button and sign up, it only takes money at the start of the month so if you change your mind and cancel it early you won't be charged.

Q: I only use a keyboard how do i play?  
A: Just rebind the buttons (See controls above) and instead of clicking a controller, click a keyboard button.

Q: The game seems kinda stuttery not sure how to describe it, it's not lagging it just feels stuttery but the FPS is fine.  
A: Try turning on Vsync that works better on some systems. In NullDC got to options->powervr->vsync and make sure it's checked. That should lead to a more consistent experiance for your system.


### BEAR or NullDC Crashes with some error
You're probably missing one of these.

VC++ 2015 (If nullDC is giving errors/crashing)
https://www.microsoft.com/en-us/download/details.aspx?id=48145

VC++ 2010 (If NullDC is giving error/crashing)
https://www.microsoft.com/en-us/download/details.aspx?id=5555

Directx 9 (NullDC is closing/crashing)
https://www.microsoft.com/en-us/download/details.aspx?displaylang=en&id=35
or (If the first link doens't work, since some Win10 users report it not letting them download the first link)
https://www.microsoft.com/en-us/download/details.aspx?id=8109
