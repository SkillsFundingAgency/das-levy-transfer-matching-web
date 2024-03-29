﻿using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;
using SFA.DAS.LevyTransferMatching.Web.Extensions;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Extensions;

[TestFixture]
public class GetApplicationsResponseExtensionsTests
{
    private Fixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
    }

    [Test]
    public void GetRemainingDaysForDelayedApproval_Returns_null_if_more_than_week_remaining()
    {
        // Arrange
        var createdOn = DateTime.UtcNow.AddDays(-20);
        const ApplicationStatus status = ApplicationStatus.Pending;
        const AutomaticApprovalOption automaticApprovalOption = AutomaticApprovalOption.DelayedAutoApproval;

        var app = _fixture.Build<GetApplicationsResponse.Application>()
            .With(a => a.Status, status)
            .With(a => a.CreatedOn, createdOn)
            .With(a => a.IsJobRoleMatch, true)
            .With(a => a.IsLocationMatch, true)
            .With(a => a.IsSectorMatch, true)
            .With(a => a.IsLevelMatch, true)
            .Create();

        // Act
        var result = app.GetRemainingDaysForDelayedApproval(automaticApprovalOption);

        // Assert
        Assert.That(result, Is.Null, "Result should be null");            
    }

    [Test]
    public void GetRemainingDaysForDelayedApproval_Returns_Valid_Response()
    {
        // Arrange
        var createdOn = DateTime.UtcNow.AddDays(-40);
        const ApplicationStatus status = ApplicationStatus.Pending;
        const AutomaticApprovalOption automaticApprovalOption = AutomaticApprovalOption.DelayedAutoApproval;

        var app = _fixture.Build<GetApplicationsResponse.Application>()
            .With(a => a.Status, status)
            .With(a => a.CreatedOn, createdOn)
            .With(a => a.IsJobRoleMatch, true)
            .With(a => a.IsLocationMatch, true)
            .With(a => a.IsSectorMatch, true)
            .With(a => a.IsLevelMatch, true)
            .Create();

        // Act
        var result = app.GetRemainingDaysForDelayedApproval(automaticApprovalOption);

        // Assert
        Assert.That(result, Is.Not.Null, "Result should not be null");
        Assert.That(result, Is.GreaterThan(0), "Result should be a positive number");
        Assert.That(result, Is.LessThan(7), "Result should be less than 7");
    }

    [Test]
    public void GetRemainingDaysForDelayedApproval_Returns_Null_If_Not_Delayed()
    {
        // Arrange
        var createdOn = DateTime.UtcNow.AddDays(-40);
        const ApplicationStatus status = ApplicationStatus.Pending;
        const AutomaticApprovalOption automaticApprovalOption = AutomaticApprovalOption.ImmediateAutoApproval;

        var app = _fixture.Build<GetApplicationsResponse.Application>()
            .With(a => a.Status, status)
            .With(a => a.CreatedOn, createdOn)
            .With(a => a.IsJobRoleMatch, true)
            .With(a => a.IsLocationMatch, true)
            .With(a => a.IsSectorMatch, true)
            .With(a => a.IsLevelMatch, true)
            .Create();

        // Act
        var result = app.GetRemainingDaysForDelayedApproval(automaticApprovalOption);

        // Assert
        Assert.That(result, Is.Null, "Result should be null");        
    }


    [Test]
    public void GetRemainingDaysForAutoRejection_Returns_Null_If_Not_Pending()
    {
        // Arrange
        var createdOn = DateTime.UtcNow.AddDays(-40);
        const ApplicationStatus status = ApplicationStatus.Approved;

        var app = _fixture.Build<GetApplicationsResponse.Application>()
            .With(a => a.Status, status)
            .With(a => a.CreatedOn, createdOn)
            .With(a => a.IsJobRoleMatch, true)
            .With(a => a.IsLocationMatch, true)
            .With(a => a.IsSectorMatch, true)
            .With(a => a.IsLevelMatch, true)
            .Create();

        // Act
        var result = app.GetRemainingDaysForAutoRejection();

        // Assert
        Assert.That(result, Is.Null, "Result should be null");
    }

    [Test]
    public void GetRemainingDaysForAutoRejection_Returns_Null_If_Over_3months_old()
    {
        // Arrange
        var createdOn = DateTime.UtcNow.AddDays(-100);
        const ApplicationStatus status = ApplicationStatus.Pending;

        var app = _fixture.Build<GetApplicationsResponse.Application>()
            .With(a => a.Status, status)
            .With(a => a.CreatedOn, createdOn)
            .With(a => a.IsJobRoleMatch, true)
            .With(a => a.IsLocationMatch, true)
            .With(a => a.IsSectorMatch, true)
            .With(a => a.IsLevelMatch, true)
            .Create();

        // Act
        var result = app.GetRemainingDaysForAutoRejection();

        // Assert
        Assert.That(result, Is.Null, "Result should be null");
    }

    [Test]
    public void GetRemainingDaysForAutoRejection_Returns_Value_if_due_to_autoreject()
    {
        // Arrange
        var createdOn = DateTime.UtcNow.AddDays(-89);
        const ApplicationStatus status = ApplicationStatus.Pending;

        var app = _fixture.Build<GetApplicationsResponse.Application>()
            .With(a => a.Status, status)
            .With(a => a.CreatedOn, createdOn)
            .With(a => a.IsJobRoleMatch, true)
            .With(a => a.IsLocationMatch, true)
            .With(a => a.IsSectorMatch, true)
            .With(a => a.IsLevelMatch, true)
            .Create();

        // Act
        var result = app.GetRemainingDaysForAutoRejection();

        // Assert
        Assert.That(result, Is.Not.Null, "Result should not be null");
        Assert.That(result, Is.GreaterThan(0), "Result should be a positive number");
        Assert.That(result, Is.LessThan(7), "Result should be less than 7");
    }

    [Test]
    public void GetDateDependentStatus_Returns_AutoApproval_String()
    {
        // Arrange
        var createdOn = DateTime.UtcNow.AddDays(-40);
        const ApplicationStatus status = ApplicationStatus.Pending;
        const AutomaticApprovalOption automaticApprovalOption = AutomaticApprovalOption.DelayedAutoApproval;

        var app = _fixture.Build<GetApplicationsResponse.Application>()
            .With(a => a.Status, status)
            .With(a => a.CreatedOn, createdOn)
            .With(a => a.IsJobRoleMatch, true)
            .With(a => a.IsLocationMatch, true)
            .With(a => a.IsSectorMatch, true)
            .With(a => a.IsLevelMatch, true)
            .Create();

        // Act
        var result = app.GetDateDependentStatus(automaticApprovalOption);

        // Assert
        Assert.That(result, Does.Contain("AUTO APPROVAL ON"));
    }


    [Test]
    public void GetDateDependentStatus_Returns_AutoRejection_String()
    {
        // Arrange
        var createdOn = DateTime.UtcNow.AddDays(-89);
        const ApplicationStatus status = ApplicationStatus.Pending;
        const AutomaticApprovalOption automaticApprovalOption = AutomaticApprovalOption.DelayedAutoApproval;

        var app = _fixture.Build<GetApplicationsResponse.Application>()
            .With(a => a.Status, status)
            .With(a => a.CreatedOn, createdOn)
            .With(a => a.IsJobRoleMatch, true)
            .With(a => a.IsLocationMatch, true)
            .With(a => a.IsSectorMatch, true)
            .With(a => a.IsLevelMatch, true)
            .Create();

        // Act
        var result = app.GetDateDependentStatus(automaticApprovalOption);

        // Assert
        Assert.That(result, Does.Contain("APPLICATION EXPIRES ON"));
    }

    [Test]
    public void GetDateDependentStatus_Returns_Status_String()
    {
        // Arrange
        var createdOn = DateTime.UtcNow.AddDays(-5);
        const ApplicationStatus status = ApplicationStatus.Pending;
        const AutomaticApprovalOption automaticApprovalOption = AutomaticApprovalOption.NotApplicable;

        var app = _fixture.Build<GetApplicationsResponse.Application>()
            .With(a => a.Status, status)
            .With(a => a.CreatedOn, createdOn)
            .With(a => a.IsJobRoleMatch, true)
            .With(a => a.IsLocationMatch, true)
            .With(a => a.IsSectorMatch, true)
            .With(a => a.IsLevelMatch, true)
            .Create();

        // Act
        var result = app.GetDateDependentStatus(automaticApprovalOption);

        // Assert
        Assert.That(result, Does.Contain("AWAITING YOUR APPROVAL"));
    }
}