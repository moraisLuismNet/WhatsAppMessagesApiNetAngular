using AutoMapper;
using FluentAssertions;
using Moq;
using WhatsAppMessagesApiNet.Application.DTOs.User;
using WhatsAppMessagesApiNet.Application.Interfaces;
using WhatsAppMessagesApiNet.Application.Services;
using WhatsAppMessagesApiNet.Domain.Entities;
using WhatsAppMessagesApiNet.Domain.Enums;
using WhatsAppMessagesApiNet.Domain.Interfaces;
using Xunit;

namespace WhatsAppMessagesApiNet.UnitTests.Services;

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _sut = new UserService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUserDto_WhenUserExists()
    {
        var email = "test@test.com";
        var user = new User { Name = "Test", Email = email };
        var userDto = new UserDto { Name = "Test", Email = email };

        _unitOfWorkMock.Setup(u => u.Users.GetByEmailAsync(email)).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        var result = await _sut.GetByEmailAsync(email);

        result.Should().NotBeNull();
        result!.Email.Should().Be(email);
        result.Name.Should().Be("Test");
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        _unitOfWorkMock.Setup(u => u.Users.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var result = await _sut.GetByEmailAsync("nonexistent@test.com");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        var users = new List<User>
        {
            new() { Email = "user1@test.com", Name = "User1" },
            new() { Email = "user2@test.com", Name = "User2" }
        };
        var userDtos = users.Select(u => new UserDto { Email = u.Email, Name = u.Name }).ToList();

        _unitOfWorkMock.Setup(u => u.Users.GetAllAsync()).ReturnsAsync(users);
        _mapperMock.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateUser()
    {
        var dto = new CreateUserDto { Name = "New User", Email = "new@test.com", Password = "Password123!" };
        var user = new User { Name = dto.Name, Email = dto.Email };
        var userDto = new UserDto { Name = dto.Name, Email = dto.Email };

        _mapperMock.Setup(m => m.Map<User>(dto)).Returns(user);
        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);
        _unitOfWorkMock.Setup(u => u.Users.AddAsync(user)).ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

        var result = await _sut.CreateAsync(dto);

        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        result.Email.Should().Be(dto.Email);
        _unitOfWorkMock.Verify(u => u.Users.AddAsync(It.IsAny<User>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
    {
        _unitOfWorkMock.Setup(u => u.Users.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteAsync("nonexistent@test.com"));
    }
}
