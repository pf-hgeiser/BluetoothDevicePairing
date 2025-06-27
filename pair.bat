@echo off
echo ## PAIR ##
echo pair.bat: given file: %1
FOR /f %%f IN (%1) DO set mac=%%f
echo MAC adress found=%mac%
c:\Users\hgeiser\source\repos\BluetoothDevicePairing-master\build\obj\BluetoothDevicePairing\Debug\net472\BluetoothDevicePairing.exe  pair-by-mac --mac %mac% --type BluetoothLE

