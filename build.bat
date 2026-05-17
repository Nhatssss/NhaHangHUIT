@"C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat" >nul
msbuild "D:\code\WPF\Nha_Hang_Huit\Nha_Hang_Huit\Nha_Hang_Huit.sln" /t:Build /p:Configuration=Debug /p:UseHardLinks=false 2>&1
exit /b %ERRORLEVEL%
