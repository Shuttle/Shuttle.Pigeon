using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Shuttle.Pigeon.Data.Models;

[PrimaryKey(nameof(MessageId), nameof(Identifier))]
public class MessageRecipient
{
    [Required]
    [StringLength(130)]
    public string Identifier { get; set; } = null!;

    public Message Message { get; set; } = null!;
    public Guid MessageId { get; set; }
    public int Type { get; set; }
}