# RoleBot  
Automatically manage access to RealDeviceMap and PMSF maps based on members with Discord role(s) in Discord guild(s).  

## Installation  

**From the installation script:** (Just need to fill out config and run)  
```
wget https://raw.githubusercontent.com/versx/RoleBot/netcore/install.sh && chmod +x install.sh && ./install.sh && rm install.sh
```

**Manually:**
- RoleBot
1. `wget https://dotnetwebsite.azurewebsites.net/download/dotnet-core/scripts/v1/dotnet-install.sh && chmod +x dotnet-install.sh && ./dotnet-install.sh && rm dotnet-install.sh`  
2. `git clone https://github.com/versx/RoleBot -b netcore`  
3. `cd RoleBot`  
4. `~/.dotnet/dotnet build`  
5. `cp config.example.json bin/Debug/netcoreapp2.1/config.json`  
6. `cd bin/Debug/netcoreapp2.1`  
7. `nano config.json` / `vi config.json` (Fill out config)  
8. `~/.dotnet/dotnet RoleBot.dll`  

## Updating  
1. `git pull`  
2. `~/.dotnet/dotnet build`  
3. `cd bin/Debug/netcoreapp2.1`  
4. `~/.dotnet/dotnet RoleBot.dll`  

**If using PMSF**: Replace `discord-callback.php` with the `discord-callback.php` file in this repository and change the RDM `group_name` column on line `35` to the group name in the `config.json` file.  
This modified discord callback checks RDM `user` table if the user has their Discord account linked to their RDM map account and has the desired group name.  