namespace CoreShared;

public static class ExceptionMessages
{
    public const string DatabaseProblem = "There was a problem with database";
    public const string NameIdentifierNull = "NameIdentifier was null";
    public const string EmailTaken = "This email is taken";
    public const string EmailNotExist = "Email does not exist";
    public const string EmailNotConfirmed = "Email is not confirmed";
    public const string PasswordInvalid = "Invalid password";
    public const string MissingUserForVerificationToken = "Could not find user connected with this verification token";
}