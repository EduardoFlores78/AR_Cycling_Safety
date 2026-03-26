# AR_Cycling_Safety
This is my 4th year project, it involves enhancing cyclist safety using an AR headset. Project consists of the implementation of IP camera streamed video to simulate rearview mirrors and haptic/sound feedback when car is approaching from behind. 

For version 1.0 go to commit 250558434393634017cbc15730a266b879511611

# Hardware & Software requirements
## Hardware 
- Meta Quest 3
- Meta Quest controller (for version 2.0 haptic feedback)
- Physical bike
- IP webcam device. A smartphone with IP webcam app or dedicated camera mounted to the bike
- PC to open unity engine

## Software
- Unity Editor version 6000.2.8f1
- Android build support
- Meta quest developer headset must be enabled.

#Steps to run
1) Clone the repository to local machine
2) Open Unity Hub, open the cloned folder
3) Launch the project using the correct Editor version.
4) Ensure IP camera device and headset are connected to the same network
5) Start the server on the device and note the IP adress provided (for example 192.168.1.X:8080)
6) Open the rearview-both scene
7) Expand OVRCameraRig>TrackingSpace>CenterEyeAnchor>MirrorSystem>TestMirror
8) In the inspector, in the wifi Mirror Stream script update the 'Cam Url' to the IP adress noted on step 5
9) Repeat for all rearview-* scenes.
10) Open SideMirror Both
11) Expand Expand OVRCameraRig>TrackingSpace>CenterEyeAnchor>Mirror_left>TestMirror
12) In the inspector, in the wifi Mirror Stream script update the 'Cam Url' to the IP adress noted on step 5
13) Expand Expand OVRCameraRig>TrackingSpace>CenterEyeAnchor>Mirror_right>TestMirror
14) In the inspector, in the wifi Mirror Stream script update the 'Cam Url' to the IP adress noted on step 5
15) Repeat for all SideMirror-* scenes
16) Connect Meta Quest to pc
17) Go to File->Build profiles
18) Ensure the platform is set to android
19) Build and Run (Ctrl+B)
20) Wear the headset
21) If on version 2.0 strap one controller to arm. 
22) Ride the bike
