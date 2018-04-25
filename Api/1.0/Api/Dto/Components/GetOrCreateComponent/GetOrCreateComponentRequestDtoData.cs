﻿using System;
using System.Collections.Generic;

namespace Zidium.Api.Dto
{
    public class GetOrCreateComponentRequestDtoData
    {
        public Guid? ParentComponentId { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }

        public Guid? TypeId { get; set; }

        public string Version { get; set; }

        public List<ExtentionPropertyDto> Properties { get; set; }
    }
}
