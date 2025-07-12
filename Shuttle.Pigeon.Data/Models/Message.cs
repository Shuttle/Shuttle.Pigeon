using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Shuttle.Pigeon.Data.Models;

[PrimaryKey(nameof(Id))]
public class Message
{
    public Guid Id { get; set; }
    [Required]
    [StringLength(30)]
    public string Channel { get; set; } = string.Empty;
    [Required]
    [StringLength(30)]
    public string ChannelMessageSenderName { get; set; } = string.Empty;
    [Required]
    [StringLength(130)]
    public string Sender { get; set; } = string.Empty;
    [StringLength(130)]
    public string? SenderDisplayName { get; set; }
    [Required]
    [StringLength(250)]
    public string Subject { get; set; } = string.Empty;

    [MaxLength(int.MaxValue)]
    public string Content { get; set; } = string.Empty;
    [MaxLength(256)]
    public string ContentType { get; set; } = string.Empty;
    public DateTime DateRegistered { get; set; }
    public DateTime? DateAccepted { get; set; }
    public DateTime? DateSent { get; set; }

    public List<MessageRecipient> Recipients { get; set; } = [];
    public List<MessageAttachment> Attachments { get; set; } = [];
}