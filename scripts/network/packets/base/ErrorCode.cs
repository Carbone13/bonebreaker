namespace Bonebreaker.Net
{
    public enum ErrorCode
    {
        TimedOut = 0,
        Success = 1,
        EmailAlreadyInUse = 2,
        UsernameAlreadyInUse = 3,
        CouldNotFindUsername = 4,
        CouldNotFindEmail = 5,
        WrongPassword = 6
    }
}