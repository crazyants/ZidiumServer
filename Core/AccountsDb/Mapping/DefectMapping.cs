using System.Data.Entity.ModelConfiguration;
using Zidium.Core.AccountsDb.Classes;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class DefectMapping : EntityTypeConfiguration<Defect>
    {
        public DefectMapping()
        {
            HasKey(t => t.Id);
            ToTable("Defects");
            Property(t => t.Notes).HasMaxLength(1000);
            Property(t => t.Title).HasMaxLength(500);
            Property(t => t.Number);
            Property(t => t.LastChangeId).HasColumnName("LastChangeId");
            Property(t => t.ResponsibleUserId).HasColumnName("ResponsibleUserId");
            HasOptional(t=>t.EventType).WithMany().HasForeignKey(t=>t.EventTypeId).WillCascadeOnDelete(false);
            HasOptional(t => t.ResponsibleUser).WithMany().HasForeignKey(t => t.ResponsibleUserId);
            HasOptional(t => t.LastChange).WithMany().HasForeignKey(t => t.LastChangeId).WillCascadeOnDelete(false);
        }
    }
}
