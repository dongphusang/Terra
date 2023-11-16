# Terra - Plant Management
OVERVIEW
-
- This system takes input from environment sensors(soil moisture, humidity, light sensor) and enables automation in watering, lighting, and ventilation. For example, if air humidity and temperature are higher than plant requirements, ventilation is increased by allowing more air to ventilate through the space(via vent, fans,..).
- Users may set these automated tasks on scheduled, manual, or turn it off and enable only the data monitoring capability.
- Additionally, there will be a mobile app for users to interact with the embedded system.

DEFINITION
-
- Workspace: The defined physical space that encompasses monitored plants. (garden beds, pots,...)

USER NEEDS
-
- This system is used to ease some aspects of plant nurturing process (watering, ventilation, or lighting).
- Many people have workspaces where they take care of their plants. Some typical tasks can be adjusting light, watering plants, and checking soil. However, it is impossible to manually monitor your plants all the time since you may go on vacations or simply procrastination. This is where the system comes in, it shares some of the responsbilities to take of the plants.

SPECIFICS
-
- The project includes an embedded system and a mobile app for users to interact with the system.
  
  EMBEDDED SYSTEM 
  - Microcontrollers:
    - ESP32: collects and uploads sensors data to the cloud
    - Raspberry Pi 4: facilitates plant's health emails and sends them to subscribed users on a daily basis.
  - Sensors & Actuators (non exhaustive list):
    - photoresistor
    - thermistor
    - humidity sensor(DHT22)
    - servo motor(for vent opening)
    - water level sensor
    - water pump
    - fan
    - relay (to power water pump)
    - soil moisture sensor
    
  MOBILE APP
  
  <img src="https://github.com/dongphusang/Terra/assets/45107557/26046321-7ce2-4183-9aaa-4986448a9eff" width="360" height="760"> <img src="https://github.com/dongphusang/Terra/assets/45107557/18247e78-8309-467a-a5e6-74e0ef1e9599" width="360" height="760">
  <img src="https://github.com/dongphusang/Terra/assets/45107557/0aa20e90-e21c-469c-a9b0-1d370d58cbcf" width="360" height="760"> <img src="https://github.com/dongphusang/Terra/assets/45107557/afde57ff-2a89-4e2d-bfbe-6df32c390fb2" width="360" height="760">

  - Functionality:
    - Adds, removes workspaces
    - Adds, removes, modifies plant entries within workspaces
    - Views collected data in numeric format
    - Views collected data in graphical format
    - Opt-in to receive emails regarding your plants health. This email report includes stats from yesterday, year-to-date, and recommendations on what to do
    - Selecting between modes:
      - Auto: system takes care of your plant depending on what modules are enabled (watering, lighting, ventilation) and decision making is based on sensors input.
      - Scheduled: system still performs automated tasks, however, it is now on demand. Users schedule these automated tasks.
      - Disabled: no automation tasks. Users still receive sensors data.
    - A built-in plant wiki for references
  
Others
-
- C# .NET MAUI is used for mobile app development
- sqlite3 is used to store local workspace, plant, email information
- InfluxDB is used for storing time-series data
- Firestore is used as a common database for mobile app and ESP32, storing configuration info
- Python facilitates plants report email to users
- C, C++ are for embedded development
- ReactJS is for web app development
- Plant API: 
- Web App: 

# In Progress 
This section is for visitors to see how the project is going. Every task or sub-task once reaches 100%, they will have an image of their final version displayed.
Every task below if they are actively in progress, they will be displayed as hyperlinks.

Total UI Revamp Series #1 (100%)      
-
- <a href="https://github.com/dongphusang/Terra/issues/18" target="_blank">Main page</a> (100%)
- <a href="https://github.com/dongphusang/Terra/issues/19" target="_blank">Add Workspace page</a> (100%)
- <a href="https://github.com/dongphusang/Terra/issues/19" target="_blank">WorkspaceList Page</a> (100%)
- <a href="https://github.com/dongphusang/Terra/issues/21" target="_blank">Adding Plant page</a> (100%)
- <a href="https://github.com/dongphusang/Terra/issues/22" target="_blank">Plant Status page</a> (100%)
- <a href="https://github.com/dongphusang/Terra/issues/23" target="_blank">Email Subscribe page</a> (100%)
- <a href="https://github.com/dongphusang/Terra/issues/24" target="_blank">Plant Info page</a> (100%)

First Unit Testing (0%)
- 
- Email Sub view model
- Graphical Plant view model
- Plant view model
- Main Page view model 
- Workspace view model
- EmailListDBService class
- EmailService class
- InfluxService class
- WorkspaceService class

Embedded System (...)
- 
- Draft code of sensors and micro-controllers: Collecting data from sensors and upload data onto InfluxDB. (100%)
- ESP32 script (30%)
- Raspberry Pi script (0%)

3D Printing (100%)
-
The purpose of 3D printing is to make the final product looks a little more aesthetic, we don't have to see as many electrical components.
- Water Tank prototype (100%)
<img src="https://github.com/dongphusang/Terra/assets/45107557/483644cd-9ab8-4531-a070-cc58f2f6bddc" width="500" height="375">
<img src="https://github.com/dongphusang/Terra/assets/45107557/39d21d51-ba47-4d4c-9e14-6b0a22a0555e" width="500" height="375">
<img src="https://github.com/dongphusang/Terra/assets/45107557/412f6a21-2b96-45d9-9895-a7a9f9851e4a" width="500" height="375">

Web App (0%) 
-

Plant API (0%)
-
