@echo off

rem ������
dotnet build

rem ������� � Handlers
xcopy /y "bin\Debug\net7.0\AVozyakov.TSLab.dll" "%LOCALAPPDATA%\TSLab\TSLab 2.0\Handlers\"
xcopy /y "bin\Debug\net7.0\AVozyakov.TSLab.pdb" "%LOCALAPPDATA%\TSLab\TSLab 2.0\Handlers\"
xcopy /h /i /c /k /e /r /y "GoogleSheetsHelper" "%LOCALAPPDATA%\TSLab\TSLab 2.0\Handlers\AVozyakov_Google\"

rem ������� � out
xcopy /y "bin\Debug\net7.0\AVozyakov.TSLab.dll" "out\build\"
xcopy /y "bin\Debug\net7.0\AVozyakov.TSLab.pdb" "out\build\"
xcopy /h /i /c /k /e /r /y "GoogleSheetsHelper" "out\build\AVozyakov_Google\"

rem �����
cd "out\build\"
tar.exe -a -cf ..\AVozyakov.TSLab.zip *
cd ..\..
