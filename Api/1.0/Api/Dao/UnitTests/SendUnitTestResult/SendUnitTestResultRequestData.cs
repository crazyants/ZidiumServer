﻿using System;

namespace Zidium.Api
{
    /// <summary>
    /// Сообщение с результатом юнит-теста
    /// </summary>
    public class SendUnitTestResultRequestData
    {
        public SendUnitTestResultRequestData()
        {
            Properties = new ExtentionPropertyCollection();
        }

        public Guid? UnitTestId { get; set; }
        
        public TimeSpan? ActualInterval{ get; set; }

        public UnitTestResult? Result { get; set; }

        /// <summary>
        /// Код причины (чтобы не склеивать в одно событие разные проблемы)
        /// </summary>
        public int? ReasonCode { get; set; }

        /// <summary>
        /// Комментарий к результату юнит-теста (необязательно)
        /// </summary>
        public string Message { get; set; }

        public ExtentionPropertyCollection Properties { get; set; }
    }
}
