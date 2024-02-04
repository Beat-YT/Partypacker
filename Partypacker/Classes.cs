namespace Partypacker
{
    public enum UserRole
    {
        User = 100,
        VerifiedUser = 200,
        TrackVerifier = 250,
        Moderator = 300,
        Administrator = 400
    }

    public class UserDetailObject
    {
        public string ID;
        public string Username;
        public string GlobalName;
        public string Avatar;
        public bool IsAdmin;
        public UserRole Role;
    }
}