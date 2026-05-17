using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using WhatsAppMessagesApiNet.Application.DTOs.Auth;
using WhatsAppMessagesApiNet.Application.Interfaces;
using WhatsAppMessagesApiNet.Domain.Entities;
using WhatsAppMessagesApiNet.Domain.Enums;
using WhatsAppMessagesApiNet.Domain.Interfaces;
using WhatsAppMessagesApiNet.Infrastructure.Services;
using Xunit;

namespace WhatsAppMessagesApiNet.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _configurationMock = new Mock<IConfiguration>();

        var jwtSettingsMock = new Mock<IConfigurationSection>();
        jwtSettingsMock.Setup(s => s["SecretKey"]).Returns("ThisIsAVeryLongSecretKeyForTestingPurposes123!");
        jwtSettingsMock.Setup(s => s["Issuer"]).Returns("TestIssuer");
        jwtSettingsMock.Setup(s => s["Audience"]).Returns("TestAudience");
        jwtSettingsMock.Setup(s => s["ExpirationHours"]).Returns("8");
        _configurationMock.Setup(c => c.GetSection("JwtSettings")).Returns(jwtSettingsMock.Object);

        _sut = new AuthService(_unitOfWorkMock.Object, _mapperMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var dto = new LoginDto { Email = "test@test.com", Password = "Password123!" };
        var user = new User
        {
            Name = "Test",
            Email = "test@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Role = UserRole.Admin,
            IsActive = true
        };

        _unitOfWorkMock.Setup(u => u.Users.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

        var result = await _sut.LoginAsync(dto);

        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.Email.Should().Be(dto.Email);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorizedAccessException_WhenPasswordIsInvalid()
    {
        var dto = new LoginDto { Email = "test@test.com", Password = "WrongPassword" };
        var user = new User
        {
            Email = "test@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword"),
            IsActive = true
        };

        _unitOfWorkMock.Setup(u => u.Users.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.LoginAsync(dto));
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsInactive()
    {
        var dto = new LoginDto { Email = "test@test.com", Password = "Password123!" };
        var user = new User
        {
            Email = "test@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            IsActive = false
        };

        _unitOfWorkMock.Setup(u => u.Users.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.LoginAsync(dto));
    }
}
