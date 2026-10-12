# **2026.62.3**

Thanks to Ave for the secruity report.

**Secruity**
- Removed the VRCN+ admin commands from the app
- Removed automatic crash reports. Crash logs are still saved locally and can be sent manually on GitHub or Discord
- Removed the avatar database spam reporter
- Startup apps must be local .exe files
- VRChat path must point to launch.exe or VRChat.exe
- Photo folders on network paths or drive roots are rejected
- FFmpeg, VRCVideoCacher and the local AI runtimes are checked by SHA-256 after download

**Removed**
- Removed the world comments as after 4 weeks of testing it wasn't a highly used feature.
- Removed the old VRCN+ profile themes. VRCN+ only shows the supporter badge now

**Changes**
- VRCN+ switched server from shinyflvres.com to db.vrcnext.com