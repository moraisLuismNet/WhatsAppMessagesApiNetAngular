using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WhatsAppMessagesApiNet.Api.Controllers;
using WhatsAppMessagesApiNet.Application.DTOs.Auth;
using WhatsAppMessagesApiNet.Application.Interfaces;
using Xunit;

namespace WhatsAppMessagesApiNet.UnitTests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _sut;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _sut = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        var dto = new LoginDto { Email = "test@test.com", Password = "Password123!" };
        var response = new AuthResponseDto { Token = "test-token", Email = "test@test.com", Name = "Test", Role = "Admin" };
        _authServiceMock.Setup(a => a.LoginAsync(dto)).ReturnsAsync(response);

        var result = await _sut.Login(dto);

        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(response);
    }

    [Fact]
    public async Task Register_ShouldReturnOk()
    {
        var dto = new RegisterDto { Name = "New User", Email = "new@test.com", Password = "Password123!" };
        var response = new AuthResponseDto { Token = "test-token", Email = "new@test.com", Name = "New User", Role = "Operator" };
        _authServiceMock.Setup(a => a.RegisterAsync(dto)).ReturnsAsync(response);

        var result = await _sut.Register(dto);

        result.Should().BeOfType<OkObjectResult>();
    }
}
