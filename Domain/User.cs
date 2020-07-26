using System;

namespace EdAppTest.Domain
{
    public class User
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiresAt { get; set; }
    }
}
