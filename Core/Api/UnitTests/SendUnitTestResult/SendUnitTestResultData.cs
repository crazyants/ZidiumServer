﻿using System;
using System.Collections.Generic;
using Zidium.Core.Common.Helpers;

namespace Zidium.Core.Api
{
    /// <summary>
    /// Сообщение с результатом юнит-теста
    /// </summary>
    public class SendUnitTestResultRequestData
    {
        public Guid? UnitTestId { get; set; }

        public double? ActualIntervalSeconds { get; set; }

        public UnitTestResult? Result { get; set; }

        /// <summary>
        /// Код причины (чтобы разные причины не склеивались в одно событие)
        /// </summary>
        public int? ReasonCode { get; set; }

        /// <summary>
        /// Комментарий к результату юнит-теста (необязательно)
        /// </summary>
        public string Message { get; set; }

        public List<ExtentionPropertyDto> Properties { get; set; }

        public long GetSize()
        {
            // Каждый результат проверки создаёт событие категории UnitTestResult
            // Поэтому считаем как для события
            long size = DataSizeHelper.DbEventRecordOverhead;
            if (Message != null)
            {
                size += Message.Length * 2;
            }
            if (Properties != null)
            {
                foreach (var property in Properties)
                {
                    if (property != null)
                    {
                        size += DataSizeHelper.DbEventParameterRecordOverhead + property.GetSize();
                    }
                }
            }
            size += DataSizeHelper.DbEventStatusRecordOverhead;
            return size;
        }
    }
}
