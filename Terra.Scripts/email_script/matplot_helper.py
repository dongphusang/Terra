import matplotlib.pyplot as plt
from influx import InfluxHelper

class MatplotHelper:
    
    def __init__(self) -> None:
        _helper = InfluxHelper()

        # get light intensity and associated time 
        _tuple_light = _helper.get_lights_of_day()
        self._light_time = _tuple_light[1]
        self._light = _tuple_light[0]

        # get humid
        _tuple_temps = _helper.get_temps_of_day()
        self._temps_time = _tuple_temps[1]
        self._temps = _tuple_temps[0]

        # get temperature
        _tuple_humids = _helper.get_humids_of_day()
        self._humids_time = _tuple_humids[1]
        self._humids = _tuple_humids[0]
    
    # map light intensity graph and save as png
    def map_light_intensity(self) -> None:
         # plot   
        plt.figure()
        plt.scatter(self._light_time, self._light)
        plt.ylabel('Light Intensity')
        plt.xlabel('Hour of Date')
        plt.title('Atmospheric by Hour Graph (P2)')
        plt.savefig('graph_light.png')

    # map temp and humid graph and save as png
    def map_temp_humid(self) -> None:
        # plot
        plt.figure()
        plt.plot(self._temps_time, self._temps, label='Temperature', color='red')
        plt.plot(self._humids_time, self._humids, label='Humidity', color='blue')
        plt.ylabel('Temp and Humid')
        plt.xlabel('Hour of Date')
        plt.title('Atmospheric by Hour Graph (P1)')
        plt.legend()
        plt.savefig('graph_tempXhumid.png')