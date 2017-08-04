using System;

namespace MessageClass
{
    [Serializable]
    public class RegisterUserInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Nickname { get; set; }
    }
}
