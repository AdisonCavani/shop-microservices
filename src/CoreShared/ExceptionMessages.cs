﻿namespace CoreShared;

public static class ExceptionMessages
{
    public const string DatabaseProblem = "There was a problem with database";
    public const string NameIdentifierNull = "NameIdentifier was null";
    public const string EmailNull = "Email was null";
    public const string EmailTaken = "This email is taken";
    public const string EmailNotExist = "Email does not exist";
    public const string EmailNotConfirmed = "Email is not confirmed";
    public const string PasswordInvalid = "Invalid password";
    public const string MissingUserForVerificationToken = "Could not find user connected with this verification token";
    public const string PaymentLost = "Could not find payment";
    public const string ProductLost = "Could not find product";
    public const string UserLost = "Could not find user";
    public const string InvalidToken = "Invalid token";
    public const string EmailAlreadyVerified = "Email already verified";
    public const string PaymentMismatch = "Payment is mismatched (probably lost)";
    public const string ProductSold = "Product sold";
    public const string OrderAlreadyPaid = "Order is already paid";
    public const string PaymentAlreadyCreated = "Pending payment already exists";
    public const string NotificationTriggerExists = "Notification trigger exists";
    public const string InvalidLiquidTemplate = "Invalid liquid template";
    public const string LiquidTemplateNotFound = "Could not find liquid template";
    public const string ProblemWithRenderingLiquidTemplate = "The liquid template was not rendered properly (empty html). Aborting sending notification";
    public const string MissingUserForNotification = "Notification was created, but received DomainEvent without userId";
    public static string NotificationTriggerMissing = "This notification trigger cannot be added. Available trigger names: ";
}