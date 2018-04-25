﻿using System;

namespace Zidium.Core.Api
{
    public class AddCheckBaseRequestData
    {
        public Guid ComponentId { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }

        public int PeriodSeconds { get; set; }

        public UnitTestResult? ErrorColor { get; set; }
    }
}
