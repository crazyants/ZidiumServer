﻿using System;

namespace Zidium.UserAccount.Models.Defects
{
    public class SelectResponsibleUserModel
    {
        public Guid EventTypeId { get; set; }

        public Guid? UserId { get; set; }
    }
}