namespace Shuttle.Pigeon;

public static class Permissions
{
    public const string Administrator = "pigeon://*";

    public class Messages
    {
        public const string Manage = "pigeon://messages/manage";
        public const string View = "pigeon://messages/view";
        public const string Register = "pigeon://messages/register";
    }
}