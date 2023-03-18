﻿// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by CashOverflow Team
// --------------------------------------------------------

using System.Threading.Tasks;
using CashOverflow.Models.Languages;
using CashOverflow.Models.Languages.Exceptions;
using Microsoft.Data.SqlClient;
using Xeptions;

namespace CashOverflow.Services.Foundations.Languages {
    public partial class LanguageService {
        private delegate ValueTask<Language> ReturningLanguageFunction();

        private async ValueTask<Language> TryCatch(ReturningLanguageFunction returningLanguageFunction) {
            try {
                return await returningLanguageFunction();
            }
            catch (NullLanguageException nulLanguageException) {
                throw CreateAndLogValidationException(nulLanguageException);
            }
            catch (InvalidLanguageException invalidLanguageException) {
                throw CreateAndLogValidationException(invalidLanguageException);
            }
            catch (NotFoundLanguageException notFoundLanguageException) {
                throw CreateAndLogValidationException(notFoundLanguageException);
            }
            catch (SqlException sqlException) {
                var failedLanguageStorageException = new FailedLanguageStorageException(sqlException);
                throw CreateAndLogCriticalDependencyException(failedLanguageStorageException);
            }
        }
        private LanguageValidationException CreateAndLogValidationException(Xeption exception) {
            var languageValidationException = new LanguageValidationException(exception);
            this.loggingBroker.LogError(languageValidationException);

            throw languageValidationException;
        }

        private LanguageDependencyException CreateAndLogCriticalDependencyException(Xeption exception) {
            var languageDependencyException = new LanguageDependencyException(exception);
            this.loggingBroker.LogCritical(languageDependencyException);

            return languageDependencyException;
        }
    }
}
