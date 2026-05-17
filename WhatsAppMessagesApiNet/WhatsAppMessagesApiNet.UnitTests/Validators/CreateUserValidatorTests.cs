using FluentAssertions;
using WhatsAppMessagesApiNet.Application.DTOs.User;
using WhatsAppMessagesApiNet.Application.Validators;
using Xunit;

namespace WhatsAppMessagesApiNet.UnitTests.Validators;

public class CreateUserValidatorTests
{
    private readonly CreateUserValidator _validator = new();

    [Fact]
    public void ShouldPass_WhenAllFieldsAreValid()
    {
        var dto = new CreateUserDto
        {
            Name = "Test User",
            Email = "test@test.com",
            Password = "Password123!",
            Role = "Admin"
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ShouldFail_WhenEmailIsInvalid()
    {
        var dto = new CreateUserDto
        {
            Name = "Test",
            Email = "not-an-email",
            Password = "Password123!",
            Role = "Admin"
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void ShouldFail_WhenPasswordIsTooShort()
    {
        var dto = new CreateUserDto
        {
            Name = "Test",
            Email = "test@test.com",
            Password = "12345",
            Role = "Admin"
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void ShouldFail_WhenRoleIsInvalid()
    {
        var dto = new CreateUserDto
        {
            Name = "Test",
            Email = "test@test.com",
            Password = "Password123!",
            Role = "InvalidRole"
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Role");
    }
}
