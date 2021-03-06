﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class NotificationMapping : EntityTypeConfiguration<Notification>
    {
        public NotificationMapping()
        {
            ToTable("Notifications");
            HasKey(t => t.Id);
            Property(t => t.SendError).HasMaxLength(4000);
            Property(t => t.Address).IsRequired().HasMaxLength(1000);

            HasRequired(t => t.Event).WithMany(t => t.Notifications).HasForeignKey(t => t.EventId);
            HasRequired(t => t.User).WithMany().HasForeignKey(t => t.UserId);
            HasOptional(t => t.Subscription).WithMany().HasForeignKey(t => t.SubscriptionId);

            Property(t => t.Status).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute()));
        }
    }
}
