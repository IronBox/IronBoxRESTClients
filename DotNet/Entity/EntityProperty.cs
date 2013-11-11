using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockBox
{
    public enum EntityProperty : long
    {
        // Important:  Keep the ordering of these properties the same
        FirstName = 0,
        LastName,
        Address1,
        Address2,
        City,
        State,
        Country,
        Zip,
        
        // Key material data recovery
        DataRecoveryMode,
        RSA_1024_Public_Backup,
        RSA_2048_Public_Backup,
        RSA_3072_Public_Backup,
        RSA_15360_Public_Backup,

        RSA_1024_Private_Backup,
        RSA_2048_Private_Backup,
        RSA_3072_Private_Backup,
        RSA_15360_Private_Backup,

        KeyDerivationIterations_Backup,
        PasswordSalt_Backup,
        ProtectionIV_Backup,
        ProtectionKeyStrength_Backup,

        // Issue #60, save protection key strength
        KeyMaterialProtectionStrength,


        ManagedProtectedDataRecoveryPIN,            // deprecated


        // Entity password reset data
        PasswordResetToken,
        PasswordResetTimeStampUtc,

        // Password recovery
        ProtectedPasswordCopy,
        ProtectedPasswordCopyKeyIterations,
        ProtectedPasswordCopyIV,
        ProtectedPasswordCopySalt,
        ProtectedSecurityAnswerContextCopy,
        SecurityAnswerIndex,

        // Issue #76, refresh token
        RefreshToken,
        CompanyName,
        CompanyURL,
        CompanyLogoURL,
        Latitude,
        Longitude,
        PreferredTimeZone,
        MobileNumber,
        AlternateContainerName,
        LegalText,
        AdminNotes,                 // Admin notes about this entity
        ProfilePictureUrl,          // The entity's profile picture URL,
        NumResultsToShow,           // Preferred number of results to show

        FacebookPageUrl,
        TwitterFeedUrl,
        LinkedInPageUrl,


        // Context roles for ICO online
        CreateDataExchangeContainersInTheseContextsCSV,
        WriteSecureMessagesInTheseContextsCSV,
        LogContainerActivityInTheseContextsCSV,
        CreateAPIKeysInTheseContextsCSV,

        // 2 Factor authentication
        CurrentTwoFactorSMSCode,
        Require2FactorAuthForLogin,
        CurrentTwoFactorSMSCodeExpirationUtc,


        // Recurly hosted login token
        RecurlyHostedLoginToken,

        // SFT Wizard temporary context:containerid CSV
        WizardTempContainerIDsCSV,


        // Data leak guard user role
        DataLeakGuardUserInTheseContextsCSV,


        // Touch codes seen by entity
        ViewedEntityContainerTouchCodesCSV,

        // Issue #131, Entity property: PayAsYouGo role in context tracker CSV
        RetailPayAsYouGoInContextIDsCSV,
        RecurlyPlanCode,

        // Identity provider
        IdentityProviderPBKDF2Iterations,
        IdentityProviderAes256IV,
        IdentityProviderProtectedIBXPassword,
        IdentityProviderID,
        IdentityProviderLoginEnabled,
        IdentityProviderSalt,
    }
}
