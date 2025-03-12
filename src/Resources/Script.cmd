"%WINDIR%\System32\TASKKILL.exe" /F /PID {0}
:_
COPY /Y "{1}" "{2}"
if not %ERRORLEVEL%==0 goto _
"{2}"
DEL "%~f0"