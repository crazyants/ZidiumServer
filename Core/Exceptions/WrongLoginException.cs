﻿using Zidium.Core.Common;

namespace Zidium.Core
{
    public class WrongLoginException : UserFriendlyException
    {
        public WrongLoginException() : base ("Неверный логин или пароль") {}
    }
}
