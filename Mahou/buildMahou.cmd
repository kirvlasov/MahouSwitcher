@echo off
setlocal

set "ROOT=%~dp0"
set "RID=%~1"
if "%RID%"=="" set "RID=win-x64"

if /i not "%RID%"=="win-x64" if /i not "%RID%"=="win-x86" (
  echo Unsupported runtime "%RID%".
  echo Use win-x64 or win-x86.
  exit /b 2
)

set "HAS_DOTNET_SDK="
for /f "usebackq delims=" %%s in (`dotnet --list-sdks 2^>nul`) do set "HAS_DOTNET_SDK=1"
if not defined HAS_DOTNET_SDK (
  echo .NET SDK was not found. Install the .NET 10 SDK to build Mahou.
  exit /b 1
)

dotnet publish "%ROOT%Mahou.csproj" ^
  --configuration Release ^
  --runtime %RID% ^
  --self-contained true ^
  -p:PublishSingleFile=true ^
  -p:PublishTrimmed=false ^
  -p:EnableCompressionInSingleFile=true ^
  -p:IncludeNativeLibrariesForSelfExtract=true ^
  -p:DebugType=none ^
  -p:DebugSymbols=false ^
  -p:Platform=%RID:win-=% ^
  -o "%ROOT%publish\%RID%"

exit /b %errorlevel%
