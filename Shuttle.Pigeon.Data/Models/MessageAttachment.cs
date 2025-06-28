using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Shuttle.Pigeon.Data.Models;

[PrimaryKey(nameof(MessageId), nameof(Name))]
public class MessageAttachment
{
    public Guid MessageId { get; set; }
    [Required]
    [StringLength(250)]
    public string Name { get; set; } = null!;
    [Required]
    [StringLength(100)]
    public string ContentType { get; set; } = null!;
    public byte[] Content { get; set; } = null!;

    public Message Message { get; set; } = null!;
}