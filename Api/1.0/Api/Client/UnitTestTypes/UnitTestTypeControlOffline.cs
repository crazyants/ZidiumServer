﻿namespace Zidium.Api
{
    public class UnitTestTypeControlOffline : UnitTestTypeControlBase
    {
        public UnitTestTypeControlOffline(Client client, string systemName) : base(client, systemName)
        {
        }

        public override bool IsFake()
        {
            return true;
        }

        public override UnitTestTypeInfo Info
        {
            get { return null; }
        }
    }
}
