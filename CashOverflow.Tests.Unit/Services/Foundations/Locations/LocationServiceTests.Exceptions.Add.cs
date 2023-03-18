﻿// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by CashOverflow Team
// --------------------------------------------------------

using System;
using System.Threading.Tasks;
using CashOverflow.Models.Locations;
using CashOverflow.Models.Locations.Exceptions;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace CashOverflow.Tests.Unit.Services.Foundations.Locations
{
	public partial class LocationServiceTests
	{
		[Fact]
		public async Task ShouldThrowCriticalDependencyExceptionOnAddIfDependencyErrorOccursAndLogItAsync()
		{
			// given
			Location someLocation = CreateRandomLocation();
			SqlException sqlException = CreateSqlException();
			var failedLocationStorageException = new FailedLocationStorageException(sqlException);
			var expecteLocationdependencyException = new LocationDependencyException(failedLocationStorageException);

			this.dateTimeBrokerMock.Setup(broker =>
			broker.GetCurrentDateTimeOffset()).Throws(sqlException);

			// when
			ValueTask<Location> addLocationTask = this.locationService.AddLocationAsync(someLocation);

			LocationDependencyException actualLocationdependencyException =
				await Assert.ThrowsAsync<LocationDependencyException>(addLocationTask.AsTask);

            // then
            actualLocationdependencyException.Should().BeEquivalentTo(expecteLocationdependencyException);

			this.dateTimeBrokerMock.Verify(broker => broker.GetCurrentDateTimeOffset(), Times.Once);

			this.loggingBrokerMock.Verify(broker =>
			broker.LogCritical(It.Is(SameExceptionAs(expecteLocationdependencyException))), Times.Once);

			this.dateTimeBrokerMock.VerifyNoOtherCalls();
			this.loggingBrokerMock.VerifyNoOtherCalls();
			this.storageBrokerMock.VerifyNoOtherCalls();
        }

		[Fact]
		public async Task ShouldThrowDependencyValidationExceptionOnAddIfDublicateKeyErrorOccursAndLogItAsync()
		{
			// given
			string someMessage = GetRandomString();
			Location someLocation = CreateRandomLocation();
			var duplicateKeyException = new DuplicateKeyException(someMessage);
			var  alreadyExistsLocationException = new AlreadyExistsLocationException(duplicateKeyException);

			var expectedLocationDependencyValidationException =
				new LocationDependencyValidationException(alreadyExistsLocationException);

			this.dateTimeBrokerMock.Setup(broker =>
			broker.GetCurrentDateTimeOffset()).Throws(duplicateKeyException);

			// when
			ValueTask<Location> addLocationTask = this.locationService.AddLocationAsync(someLocation);

			LocationDependencyValidationException actualLocationDependencyValidationException =
				await Assert.ThrowsAsync<LocationDependencyValidationException>(addLocationTask.AsTask);

			// then
			actualLocationDependencyValidationException.Should().
				BeEquivalentTo(expectedLocationDependencyValidationException);

			this.dateTimeBrokerMock.Verify(broker => broker.GetCurrentDateTimeOffset(), Times.Once);

			this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(SameExceptionAs(
				expectedLocationDependencyValidationException))), Times.Once);

			this.dateTimeBrokerMock.VerifyNoOtherCalls();
			this.loggingBrokerMock.VerifyNoOtherCalls();
			this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}

