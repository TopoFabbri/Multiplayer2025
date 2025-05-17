:: Define source directories
set "src1=.\lib\MultiplayerLib\bin\Release\netstandard2.1"
set "src2=.\mmc\MatchMaker\bin\Release\net9.0"
set "src3=.\Unity\Builds\Client"

:: Define target directory
set "target=D:\UnityProjects\Multiplayer2025\Release"

:: Remove existing folders if they exist
if exist "%target%\Library" rd /s /q "%target%\Library"
if exist "%target%\MatchMaker" rd /s /q "%target%\MatchMaker"
if exist "%target%\Client" rd /s /q "%target%\Client"

:: Copy directories to target with new names
xcopy /E /I "%src1%" "%target%\Library"
xcopy /E /I "%src2%" "%target%\MatchMaker"
xcopy /E /I "%src3%" "%target%\Client"

:: Remove existing ZIP files if they exist
if exist "%target%\Library.zip" del "%target%\Library.zip"
if exist "%target%\MatchMaker.zip" del "%target%\MatchMaker.zip"
if exist "%target%\Client.zip" del "%target%\Client.zip"

:: Compress folders into separate zip files
powershell -Command "Compress-Archive -Path '%target%\Library' -DestinationPath '%target%\Library.zip'"
powershell -Command "Compress-Archive -Path '%target%\MatchMaker' -DestinationPath '%target%\MatchMaker.zip'"
powershell -Command "Compress-Archive -Path '%target%\Client' -DestinationPath '%target%\Client.zip'"

echo All done! Existing files have been replaced.
endlocal