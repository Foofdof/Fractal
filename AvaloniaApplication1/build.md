
MacOS-arm64
```shell 

dotnet publish AvaloniaApplication1/AvaloniaApplication1.csproj \
-c Release -r osx-arm64 --self-contained true \
-p:PublishSingleFile=true \
-p:PublishReadyToRun=false \
-p:PublishTrimmed=false \
-o dist/osx-arm64
```
to .app

```shell

mkdir -p "$APP.app/Contents/MacOS"
cp dist/osx-arm64/AvaloniaApplication1 "$APP.app/Contents/MacOS/"
cp dist/osx-arm64/*.dylib "$APP.app/Contents/MacOS/"
cat > "$APP.app/Contents/Info.plist" <<'PLIST'
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN"
  "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0"><dict>
  <key>CFBundleName</key><string>AvaloniaApplication1</string>
  <key>CFBundleIdentifier</key><string>com.example.AvaloniaApplication1</string>
  <key>CFBundleVersion</key><string>1.0</string>
  <key>CFBundleShortVersionString</key><string>1.0</string>
  <key>CFBundleExecutable</key><string>AvaloniaApplication1</string>
  <key>CFBundlePackageType</key><string>APPL</string>
</dict></plist>
PLIST
codesign --force --deep --sign - --timestamp=none "$APP.app"
```

Win64
```shell

dotnet publish AvaloniaApplication1/AvaloniaApplication1.csproj \
  -c Release -r win-x64 --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishReadyToRun=false \
  -p:PublishTrimmed=false \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  -p:IncludeAllContentForSelfExtract=true \
  -p:DebugType=none -p:DebugSymbols=false \
  -o dist/win-x64
```