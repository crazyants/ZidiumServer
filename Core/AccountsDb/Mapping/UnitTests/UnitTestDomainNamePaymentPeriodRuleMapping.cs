﻿using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class UnitTestDomainNamePaymentPeriodRuleMapping : EntityTypeConfiguration<UnitTestDomainNamePaymentPeriodRule>
    {
        public UnitTestDomainNamePaymentPeriodRuleMapping()
        {
            ToTable("UnitTestDomainNamePaymentPeriodRules");
            HasKey(t => t.UnitTestId);
            Property(t => t.Domain).HasColumnName("Domain");
            Property(t => t.LastRunErrorCode).HasColumnName("LastRunErrorCode");
            Property(t => t.AlarmDaysCount).HasColumnName("AlarmDaysCount");
            Property(t => t.WarningDaysCount).HasColumnName("WarningDaysCount");
            HasRequired(t => t.UnitTest).WithOptional(t => t.DomainNamePaymentPeriodRule).WillCascadeOnDelete(false);
        }
    }
}
