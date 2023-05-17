# Terra
Plant Management

PRODUCT
- This system enables automation in watering, ventilation, and lighting. Moreover, the way these modules work depends on input from environment sensors(soil moisture, humidity, light sensor). For example, if air humidity and temperature are higher than plant requirements, ventilation is increased by allowing more air to ventilate through the space(via vent, fans,..).
- Users may also choose for these automated tasks to be on schedule, manual, or turn it off and enable only the monitoring capability.
- Additionally, there will be a mobile app for users to interact with the embedded system.

DEFINITION
- Workspace: The defined physical space that encompasses monitored plants. (garden, backyard, living room,...)

USER NEEDS
- This system can be used by many people who want to automate some aspect of plant nurturing process (watering, ventilation, or lighting).
- Many people have workspaces where they take care of their plants. Some typical tasks can be adjusting light, watering plants, and checking soil. However, it is impossible to monitor your plants all the time since you may go on vacations or simply procrastination. This is where the system comes in, it shares some of the responsbilities to take of the plants.

SPECIFICS
- The project includes an embedded system and a mobile app for users to interact with the system.
  
  EMBEDDED SYSTEM 
  - Microcontrollers:
    - Arduino Uno (possibly Nano)
    - Raspberry Pi 4
    - Connected to eachother using serial connection. Python (possibly Rust) is used to setup communication between the two controllers.
  - Sensors & Actuators (non exhaustive list):
    - photoresistor
    - thermistor
    - humidity sensor(DHT11)
    - servo motor(for vent opening)
    - water level sensor
    - water pump
    - fan
    - relay (to power water pump)
    - soil moisture sensor
  - Database:
    - Raspberry Pi 4 pushes data received from Arduino onto InfluxDB
    
  MOBILE APP
  
  <img src="https://github.com/dongphusang/Terra/assets/45107557/37116616-a191-4dce-a6bf-223bbf4569d7" width="360" height="760"> <img src="https://github.com/dongphusang/Terra/assets/45107557/1fd98765-7ad2-4515-89f8-13cf9af92cdb" width="360" height="760">

  - Functionality:
    - Adds, removes workspaces
    - Adds, removes, modifies plant entries within workspaces
    - Views collected data in numeric, text values
    - Views collected data in graphical format
    - Opt-in to receive emails on how your plants health. This email report includes stats from yesterday, year-to-date, and recommendations on what to do
    - Selecting between modes:
      - Auto: system takes care of your plant depending on what modules are enabled (watering, lighting, ventilation) and decision making is based on sensors input.
      - Scheduled: system still performs automated tasks, however, it is now on demand. Users schedule these automated tasks.
      - Disabled: no automation tasks. Users still receive sensors data.
    - A built-in plant wiki for references
  - Others:
    - C# .NET MAUI is used for development
    - sqlite3 is used to store local workspace and plant information
  
