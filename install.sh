# Download .NET Core 2.1 installer
wget https://dotnetwebsite.azurewebsites.net/download/dotnet-core/scripts/v1/dotnet-install.sh

# Make installer executable
chmod +x dotnet-install.sh

# Install .NET Core 2.1
./dotnet-install.sh --runtime dotnet --version 2.1.0

# Delete .NET Core 2.1 installer
rm dotnet-install.sh

# Clone repository
git clone https://github.com/versx/RoleBot -b netcore

# Change directory into cloned repository
cd RoleBot

# Build RoleBot.dll
~/.dotnet/dotnet build

# Copy example config
cp config.example.json bin/Debug/netcoreapp2.1/config.json

# Change directory into build folder
cd bin/Debug/netcoreapp2.1

# Start RoleBot.dll
#~/.dotnet/dotnet RoleBot.dll
