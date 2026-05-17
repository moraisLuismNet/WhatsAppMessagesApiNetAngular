using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatsAppMessagesApiNet.Domain.Entities;

namespace WhatsAppMessagesApiNet.Infrastructure.Persistence.EF.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EntityName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.EntityId).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Action).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.ChangedBy).IsRequired().HasMaxLength(200);
        builder.Property(x => x.OldValues);
        builder.Property(x => x.NewValues);
        builder.HasIndex(x => x.EntityName);
        builder.HasIndex(x => x.EntityId);
        builder.HasIndex(x => x.ChangedBy);
        builder.HasIndex(x => x.ChangedAt);
    }
}
