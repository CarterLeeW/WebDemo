using System;
using API.Entities;

namespace API.DTOs;

public class MessageDto
{
    public int Id { get; set; }
    // sender
    public int SenderId { get; set; }
    public required string SenderUsername { get; set; }
    public required string SenderPhotoUrl { get; set; }
    // recipient
    public int RecipientId { get; set; }
    public required string RecipientUsername { get; set; }
    public required string RecipientPhotoUrl { get; set; }
    // content
    public required string Content { get; set; }
    public DateTime? DateRead { get; set; }
    public DateTime DateMessageSent { get; set; }
}
