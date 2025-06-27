@echo off
cls
echo given file %1
FOR /f %%f IN (%1) DO echo %%f
FOR /f %%f IN (%1) DO set mac=%%f
echo MAC=%mac%
pause