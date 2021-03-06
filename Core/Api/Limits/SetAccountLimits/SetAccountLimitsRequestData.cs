﻿using Zidium.Core.AccountsDb;

namespace Zidium.Core.Api
{
    public class SetAccountLimitsRequestData
    {
        public AccountTotalLimitsDataInfo Limits { get; set; }

        public TariffLimitType Type { get; set; }
    }
}
