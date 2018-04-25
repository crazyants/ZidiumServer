﻿using System;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models
{
    public class DashboardEventModel
    {
        public EventImportance Importance { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsProcessed { get; set; }
        public Guid EventTypeId { get; set; }
        public DateTime ActualDate { get; set; }
    }
}