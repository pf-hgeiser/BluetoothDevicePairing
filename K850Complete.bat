@echo off
cls
echo ###################################################################
echo ##### discover + connect and disconnect Logitech K850 keyboard ####
echo ###################################################################
cd c:\Users\hgeiser\source\repos\BluetoothDevicePairing-master
echo # Tastatur einschalten
echo # Wenn noch unbekannt: Bluetoooth Erkennung an der Tastatur einschalten #
echo DISCOVER 
call discover.bat
:pairagain
echo PAIR 
call pair.bat c:\temp\mac.txt
if %errorlevel% NEQ 0 echo fehlerlevel %errorlevel% 
if %errorlevel% NEQ 0 echo UNPAIR übersprungen
if %errorlevel% NEQ 0 goto errorinfo

echo UNPAIR 
call unpair.bat c:\temp\mac.txt
echo ## FINISHED TEST ##
echo ## Keyboard OK ##
exit /b 0

:errorinfo
echo ## finished with error %errorlevel% ##
echo ## FINISHED TEST with ERROR ##
:end
exit /b 1
