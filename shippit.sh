rm -rf ./pub-linux
rm -rf ./pub-windows
rm -rf ./pub-mac
dotnet publish ./src/BagelQuest/BagelQuest.vbproj -o ./pub-linux -c Release --sc -p:PublishSingleFile=true -r linux-x64
dotnet publish ./src/BagelQuest/BagelQuest.vbproj -o ./pub-windows -c Release --sc -p:PublishSingleFile=true -r win-x64
dotnet publish ./src/BagelQuest/BagelQuest.vbproj -o ./pub-mac -c Release --sc -p:PublishSingleFile=true -r osx-x64
butler push pub-windows thegrumpygamedev/bagel-quest:windows
butler push pub-linux thegrumpygamedev/bagel-quest:linux
butler push pub-mac thegrumpygamedev/bagel-quest:mac
git add -A
git commit -m "shipped it!"