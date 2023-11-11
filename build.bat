dotnet build

rem xcopy /Y "bin\Debug\net7.0\vav.dll" "%APPDATA%\..\Local\TSLab\TSLab 2.0\Handlers"
rem xcopy /Y "bin\Debug\net7.0\vav.pdb" "%APPDATA%\..\Local\TSLab\TSLab 2.0\Handlers"

xcopy /Y "bin\Debug\net7.0\vav.dll" "%LOCALAPPDATA%\TSLab\TSLab 2.0\Handlers"
xcopy /Y "bin\Debug\net7.0\vav.pdb" "%LOCALAPPDATA%\TSLab\TSLab 2.0\Handlers"
