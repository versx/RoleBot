namespace RoleBot.Data.Model
{
    using ServiceStack.DataAnnotations;

    [Alias("user")]
    class User
    {
        [Alias("username")]
        public string Username { get; set; }

        [Alias("email")]
        public string Email { get; set; }

        [Alias("discord_id")]
        public ulong? DiscordId { get; set; }

        [Alias("email_verified")]
        public bool EmailVerified { get; set; }

        [Alias("group_name")]
        public string GroupName { get; set; }
    }
}