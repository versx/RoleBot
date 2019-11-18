namespace RoleBot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Timers;
    using System.Threading.Tasks;

    using DSharpPlus;
    using DSharpPlus.Entities;
    using DSharpPlus.EventArgs;

    using ServiceStack.OrmLite;

    using RoleBot.Configuration;
    using RoleBot.Data;
    using RoleBot.Data.Model;  

    class Bot
    {
        const string RootGroupName = "root";
        const string DefaultGroupName = "default";

        #region Variables

        private readonly DiscordClient _client;
        private readonly Config _config;
        private readonly Timer _timer;
        private bool _checking;

        #endregion

        #region Constructor

        public Bot(Config config)
        {
            _config = config;
            _client = new DiscordClient(new DiscordConfiguration
            {
                AutomaticGuildSync = true,
                AutoReconnect = true,
                EnableCompression = true,
                LogLevel = LogLevel.Debug,
                Token = _config.Token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = false
            });
            _client.Ready += OnReady;
            _timer = new Timer
            {
                Interval = _config.RoleCheckInterval * (60 * 1000)
            };
            _timer.Elapsed += OnTimerElapsed;
        }

        #endregion

        #region Events

        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_checking)
                return;

            _checking = true;
            var users = GetUsers();
            Console.WriteLine($"Checking list of {users.Count.ToString("N0")} map users from database...");
            for (var i = 0; i < users.Count; i++)
            {
                var user = users[i];
                //TODO: Check user.discordId against guild members list, if it exists check if user has role if so set to donor in user table.
                if (!user.DiscordId.HasValue)
                {
                    //Console.WriteLine($"User {user.Username} doesn't have their Discord account linked...");
                    continue;
                }

                var hasRole = await IsValidUser(_config.Guilds, user.DiscordId ?? 0);
                if (string.Compare(user.GroupName, RootGroupName, true) == 0)
                    continue;

                //If user already has desired role and group is set for map, skip.
                var hasGroupName = string.Compare(user.GroupName, _config.DesiredGroupName, true) == 0;
                if (hasRole && hasGroupName)
                    continue;

                if (hasRole)
                {
                    Console.WriteLine($"Updating access for {user.Username} ({user.DiscordId})...");
                    user.GroupName = _config.DesiredGroupName;
                    UpdateUser(user);
                }
                else
                {
                    //Delete user if they created an account but don't have the desired role
                    Console.WriteLine($"Removing access from user {user.Username}...");
                    user.GroupName = DefaultGroupName;
                    UpdateUser(user);
                    //DeleteUser(user);
                }
            }

            _checking = false;
        }

        private async Task OnReady(ReadyEventArgs e)
        {
            if (!_timer.Enabled)
            {
                _timer.Start();
            }

            Console.WriteLine($"Connected to Discord as {e.Client.CurrentUser.Username}#{e.Client.CurrentUser.Discriminator}\r\n{e.Client.CurrentUser.Id}");
            await Task.CompletedTask;
        }

        #endregion

        #region Public Methods

        public async Task Start()
        {
            await _client.ConnectAsync();
        }

        #endregion

        #region Private Methods

        private async Task<bool> IsValidUser(Dictionary<ulong, List<ulong>> guilds, ulong userId)
        {
            if (userId == 0)
                return false;

            var keys = guilds.Keys.ToList();
            for (var i = 0; i < keys.Count; i++)
            {
                var guildId = keys[i];
                if (!_client.Guilds.ContainsKey(guildId))
                {
                    //TODO: Print error
                    continue;
                }

                var guild = _client.Guilds[guildId];
                if (guild == null)
                {
                    //TODO: Print error
                    continue;
                }

                try
                {
                    var member = await GetMember(guild, userId);
                    if (member == null)
                    {
                        //TODO: Print error
                        continue;
                    }

                    var memberRoleIds = member?.Roles?.Select(x => x.Id);
                    if (memberRoleIds == null)
                    {
                        continue;
                    }

                    var roles = guilds[guildId];
                    for (var j = 0; j < roles.Count; j++)
                    {
                        var role = roles[j];
                        if (memberRoleIds.Contains(role))
                        {
                            return true;
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"ERROR: Failed to get roles for member with id {userId}");
                    continue;
                }
            }

            return false;
        }

        private async Task<DiscordMember> GetMember(DiscordGuild guild, ulong userId)
        {
            try
            {
                var member = await guild.GetMemberAsync(userId);
                if (member == null)
                {
                    //TODO: Print error
                    return null;
                }

                return member;
            }
            catch //(DSharpPlus.Exceptions.NotFoundException ex)
            {
                return null;
            }
        }

        private bool UpdateUser(User user)
        {
            if (string.IsNullOrEmpty(_config.ConnectionString))
                return false;

            try
            {
                using (var db = DataAccessLayer.CreateFactory(_config.ConnectionString).Open())
                {
                    db.Save(user);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        private bool DeleteUser(User user)
        {
            if (string.IsNullOrEmpty(_config.ConnectionString))
                return false;

            try
            {
                using (var db = DataAccessLayer.CreateFactory(_config.ConnectionString).Open())
                {
                    var result = db.Delete(user);
                    if (result > 0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }

        private List<User> GetUsers()
        {
            if (string.IsNullOrEmpty(_config.ConnectionString))
                return null;

            try
            {
                using (var db = DataAccessLayer.CreateFactory(_config.ConnectionString).Open())
                {
                    var users = db.LoadSelect<User>();
                    return users;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex}");
            }

            return null;
        }

        #endregion
    }
}