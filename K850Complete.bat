@echo off
cls
echo ###################################################################
echo ##### discover + connect and disconnect Logitech K850 keyboard ####
echo ###################################################################
cd c:\Users\hgeiser\source\repos\BluetoothDevicePairing-master
echo # keyboard OFF #
pause
call discover.bat
echo # keyboard ON #
pause
call discover.bat
echo # Bluetoooth detect ON #
pause
call pair.bat c:\temp\mac.txt
call unpair.bat c:\temp\mac.txt
