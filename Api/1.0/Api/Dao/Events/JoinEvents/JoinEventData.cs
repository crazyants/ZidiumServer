﻿using System;

namespace Zidium.Api
{
    public class JoinEventData
    {
        public Guid? EventId { get; set; }

        public Guid? ComponentId { get; set; }

        public Guid? TypeId { get; set; }

        public long? JoinKey { get; set; }

        public DateTime? StartDate { get; set; }

        public TimeSpan? JoinInterval { get; set; }

        public EventImportance? Importance { get; set; }

        public int? Count { get; set; }

        public string Message { get; set; }

        public string Version { get; set; }
    }
}
