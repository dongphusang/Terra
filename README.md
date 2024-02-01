# Terra - Plant Management
OVERVIEW
-
- This system takes input from environment sensors(soil moisture, humidity, light sensor) and enables automation in watering.
- Users may set these automated tasks on scheduled, manual, or turn it off and enable only the data monitoring capability.
- Additionally, there will be a mobile app for users to interact with the embedded system.

DEFINITION
-
- Workspace: The defined physical space that encompasses monitored plants. (garden beds, pots,...)

USER NEEDS
-
- This system is used to ease some aspects of plant nurturing process (watering, ventilation, or lighting).
- Many people have workspaces where they take care of their plants. Some typical tasks can be adjusting light, watering plants, and checking soil. However, it is impossible to manually monitor your plants all the time since you may go on vacations or simply procrastination. This is where the system comes in as it shares some responsbilities of plant nurturing.

SPECIFICS
-
- The project includes an embedded system and a mobile app for users to interact with the system.
  
  EMBEDDED SYSTEM 
  - Microcontrollers:
    - ESP32: collects and uploads sensors data to the cloud
    - Raspberry Pi 4: facilitates plant's health emails and sends them to subscribed users on a daily basis.
  - Sensors & Actuators (non exhaustive list):
    - photoresistor
    - humidity sensor (DHT22)
    - waterproof ultrasonic water level measuring module (JSN-SR04T)
    - water pump
    - solid state relay (to power water pump)
    - soil moisture sensor
    
  MOBILE APP
  
  <img src="https://github.com/dongphusang/Terra/assets/45107557/14a624d1-1226-474b-be68-88f3a5d4aaaa"> <img src="https://github.com/dongphusang/Terra/assets/45107557/7bd87aea-07a0-4e3a-9a3c-7481524b82d9">
  <img src="https://github.com/dongphusang/Terra/assets/45107557/97eff82c-33cf-4bcb-800b-d4e1125b5622"> <img src="https://github.com/dongphusang/Terra/assets/45107557/1c82dec6-8c88-4dc6-9f20-00be3654acac">
  <img src="https://github.com/dongphusang/Terra/assets/45107557/75efa561-0b2f-42b5-b1a4-a0ec1b16a6bd"> <img src="https://github.com/dongphusang/Terra/assets/45107557/9c17c845-a433-439c-9456-072b987f2933">


  - Functionality:
    - Add, remove workspaces
    - Add plant for a workspace
    - View collected data in numeric format
    - View collected data in graphical format
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

# In Progress 
This section is for visitors to see how the project is going. Every task or sub-task once reaches 100%, they will have an image of their final version displayed.
Every task below if they are actively in progress, they will be displayed as hyperlinks.

UI Revamp (100%)      
-
- <a href="https://github.com/dongphusang/Terra/issues/44" target="_blank">Main page</a> (100%)


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

3D Printing (...)
-
- Water Tank prototype (100%)
<img src="https://github.com/dongphusang/Terra/assets/45107557/483644cd-9ab8-4531-a070-cc58f2f6bddc" width="500" height="375">
<img src="https://github.com/dongphusang/Terra/assets/45107557/39d21d51-ba47-4d4c-9e14-6b0a22a0555e" width="500" height="375">
<img src="https://github.com/dongphusang/Terra/assets/45107557/412f6a21-2b96-45d9-9895-a7a9f9851e4a" width="500" height="375">

Web App (0%) 
-

Plant API (0%)
-
