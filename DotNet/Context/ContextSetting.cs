using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public enum ContextSetting : int
    {
        // Context setting keys
        CompanyName = 0,
        SignInGreeting,
        CompanyUrl,


        ProductBadgeText, 
        ProductBadgeUrl,
        SendActivationEmail,
        SendActivationEmailInstructions,
        ContextFromEmail,
        ContextBccEmail,
        ContextCcEmail,
        AccountActivationEmailSubject,
        AccountActivationEmailBody,
        SiteName,
        PasswordRegex,
        MinPasswordComplexityDescription,
        SignIn,


        DataRecoveryPINRegex,

        // Issue #61, context key material
        ContextPublicKey,
        ContextProtectedPrivateKey,
        ContextKeyIterations,
        ContextSalt,
        ContextProtectionIV,
        ContextAsymmetricKeyStrength,
        ContextSymmetricKeyStrength,

        PasswordResetEmailBody,
        PasswordResetEmailSubject,


        AccountInspectionEnabled,
        
        CompanyLogoUrl,

        ContainerName,

        LegalText,
        BingMapsAPIKey,
        PrivacyPolicyUrl,
        TermsOfServiceUrl,
        TwitterUrl,
        FacebookUrl,
        LinkedInUrl,
        DefaultErrorText,

        RequireTosAndPrivacyAgreementToSignIn,

        ContextFromEmailDisplayName,

        ContextSMTPServer,
        ContextSMTPServerPort,
        ContextSMTPHostUsesSSL,
        ContextSMTPHostRequiresAuthentication,
        ContextSMTPHostUserName,
        ContextSMTPHostPassword,


        // Context admin entity IDs csv
        AdminEntityIDCSV,

        // 2factor code settings
        TwoFactorCodeLength,
        TwoFactorAuthenticationEnabled,

        // Recurly settings
        RecurlyApiKey,
        RecurlySubdomain,

        // Context support information
        ContextSupportEmailsCSV,
        ContextSupportMobileNumbersCSV,

        ContextAPISupportEnabled,


        // SFT policies
        SFTPolicy_MandatoryContainerModifiersCSV,
        SFTPolicy_AutomaticSecurePackageExpirationDays,


        // SFT auto settings
        SFT_AutoEnableLogging,
        SFT_AutoEnableSecureMessaging,

        // Brute force protection threshold
        BruteForceProtectionThreshold,
        BruteForceProtectionEnabled,


        // Max number of entities in a context
        MaxNumContextEntities,

        // Notes about the Context
        ContextNotes,

        // Use master context SMTP settings
        UseMasterContextSMTPSettings,           // Use master context SMTP settings


        // Notification formats
        SendNotificationAsHTML,                                     // Bool flag that indicates whether to use HTML or text email
        NotificationRedactSensitiveInformationAfterNumChars,        // Number of characters to redact after


        EmailLegalText,

        // Powered by seal URL
        PoweredBySealURL,

        // Redact configuration
        ContainerNameRedactThresholdInt,                            // Number of characters before container names are redacted
        FileNameRedactThresholdInt,                                 // Number of characters before file names are redacted

        // All future setting keys, keep order consistent!
    }
}
