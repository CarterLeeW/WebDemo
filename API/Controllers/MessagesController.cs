using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class MessagesController(IMessageRepository messageRepository,
    IUserRepository userRepository, IMapper mapper) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var username = User.GetUsername();
        if (username == createMessageDto.RecipientUsername.ToLower())
        {
            return BadRequest("You cannot message yourself");
        }

        var sender = await userRepository.GetUserByUsernameAsync(username);
        var recipient = await userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
        if (recipient == null || sender == null ||
            sender.UserName == null || recipient.UserName == null)
        {
            return BadRequest("Cannot send message at this time");
        }

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            DateMessageSent = DateTime.UtcNow,
            Content = createMessageDto.Content
        };
        messageRepository.AddMessage(message);
        
        if (await messageRepository.SaveAllAsync())
        {
            return Ok(mapper.Map<MessageDto>(message));
        }
        return BadRequest("Failed to save message");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery]MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();
        var messages = await messageRepository.GetMessagesForUser(messageParams);
        Response.AddPaginationHeader(messages);
        
        return messages;
    }

    [HttpGet("thread/{recipientUsername}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string recipientUsername)
    {
        var currentUsername = User.GetUsername();

        return Ok(await messageRepository.GetMessageThread(currentUsername, recipientUsername));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();
        var message = await messageRepository.GetMessage(id);
        if (message == null)
        {
            return BadRequest("Cannot delete this message");
        }

        // Should the user be allowed to delete this message?
        if (message.SenderUsername != username && message.RecipientUsername != username)
        {
            return Forbid();
        }
        if (message.SenderUsername == username)
        {
            message.SenderDeleted = true;
        }
        if (message.RecipientUsername == username)
        {
            message.RecipientDeleted = true;
        }
        if (message is {SenderDeleted: true, RecipientDeleted: true})
        {
            messageRepository.DeleteMessage(message);
        }

        if (await messageRepository.SaveAllAsync())
        {
            return Ok();
        }
        return BadRequest("Problem deleting the message");
    }
}
