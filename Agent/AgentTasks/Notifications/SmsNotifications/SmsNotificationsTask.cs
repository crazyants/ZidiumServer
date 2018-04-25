﻿using System;

namespace Zidium.Agent.AgentTasks.Notifications
{
    /// <summary>
    /// Задача создает для каждого sms-уведомления задачу по отправке sms
    /// </summary>
    public class SmsNotificationsTask : AgentTaskBase
    {
        public SmsNotificationsTask()
        {
            ExecutionPeriod = TimeSpan.FromSeconds(10);
        }

        protected override AgentTaskResult Do()
        {
            var processor = new SmsNotificationsProcessor(Logger, CancellationToken);
            processor.Process();

            var result = string.Format("Отправлено {0} уведомлений", processor.CreatedNotificationsCount);
            return GetResult(processor.DbProcessor, result);
        }
    }
}
