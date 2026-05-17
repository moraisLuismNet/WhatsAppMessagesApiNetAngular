using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using WhatsAppMessagesApiNet.Application.DTOs.Auth;
using WhatsAppMessagesApiNet.Application.DTOs.Message;
using Xunit;

namespace WhatsAppMessagesApiNet.IntegrationTests.Controllers;

public class MessagesControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MessagesControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> GetTokenAsync()
    {
        var registerDto = new RegisterDto
        {
            Name = "Msg Test User",
            Email = $"msgtest{Guid.NewGuid():N}@test.com",
            Password = "Password123!"
        };
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        var content = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        return content!.Token;
    }

    [Fact]
    public async Task CreateMessage_WithValidToken_ShouldReturnCreated()
    {
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new CreateMessageDto { To = "+1234567890", Body = "Integration test message" };
        var response = await _client.PostAsJsonAsync("/api/messages", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateMessage_WithoutToken_ShouldReturnUnauthorized()
    {
        var dto = new CreateMessageDto { To = "+1234567890", Body = "Test" };

        var response = await _client.PostAsJsonAsync("/api/messages", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
