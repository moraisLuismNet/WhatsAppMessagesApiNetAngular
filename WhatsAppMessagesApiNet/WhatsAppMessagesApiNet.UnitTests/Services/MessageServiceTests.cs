using AutoMapper;
using FluentAssertions;
using Moq;
using WhatsAppMessagesApiNet.Application.DTOs.Message;
using WhatsAppMessagesApiNet.Application.Interfaces;
using WhatsAppMessagesApiNet.Application.Services;
using WhatsAppMessagesApiNet.Domain.Entities;
using WhatsAppMessagesApiNet.Domain.Enums;
using WhatsAppMessagesApiNet.Domain.Interfaces;
using Xunit;

namespace WhatsAppMessagesApiNet.UnitTests.Services;

public class MessageServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IWhatsAppProvider> _whatsAppProviderMock;
    private readonly MessageService _sut;

    public MessageServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _whatsAppProviderMock = new Mock<IWhatsAppProvider>();
        _whatsAppProviderMock.Setup(p => p.SendTextAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new WhatsAppMessageResult { Success = true, MessageId = "test-sid" });
        _sut = new MessageService(_unitOfWorkMock.Object, _mapperMock.Object, new[] { _whatsAppProviderMock.Object });
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateMessage()
    {
        var userId = Guid.NewGuid().ToString();
        var dto = new CreateMessageDto { To = "+1234567890", Body = "Hello World" };
        var message = new Message { Id = 1, UserId = userId, To = dto.To, Body = dto.Body };
        var messageDto = new MessageDto { Id = 1, To = dto.To, Body = dto.Body };

        _mapperMock.Setup(m => m.Map<MessageDto>(It.IsAny<Message>())).Returns(messageDto);
        _unitOfWorkMock.Setup(u => u.Messages.AddAsync(It.IsAny<Message>())).ReturnsAsync(message);
        _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

        var result = await _sut.CreateAsync(dto, userId);

        result.Should().NotBeNull();
        result.To.Should().Be(dto.To);
        _unitOfWorkMock.Verify(u => u.Messages.AddAsync(It.IsAny<Message>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldUpdateStatus()
    {
        var messageId = 1;
        var message = new Message { Id = messageId, Status = MessageStatus.Pending };
        var dto = new UpdateMessageStatusDto { Status = "Sent" };
        var messageDto = new MessageDto { Id = messageId, Status = "Sent" };

        _unitOfWorkMock.Setup(u => u.Messages.GetByIdAsync(messageId)).ReturnsAsync(message);
        _mapperMock.Setup(m => m.Map<MessageDto>(message)).Returns(messageDto);
        _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

        var result = await _sut.UpdateStatusAsync(messageId, dto);

        result.Should().NotBeNull();
        _unitOfWorkMock.Verify(u => u.Messages.UpdateAsync(message), Times.Once);
        _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
    }
}
