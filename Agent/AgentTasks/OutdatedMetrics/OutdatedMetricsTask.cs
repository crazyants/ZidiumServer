﻿using System;

namespace Zidium.Agent.AgentTasks.OutdatedMetrics
{
    internal class OutdatedMetricsTask : AgentTaskBase
    {
        public OutdatedMetricsTask()
        {
            ExecutionPeriod = TimeSpan.FromMinutes(1);
        }

        protected override AgentTaskResult Do()
        {
            var processor = new OutdatedMetricsProcessor(Logger, CancellationToken);
            processor.Process();

            var result = string.Format("Обновлено {0} статусов метрик", processor.Count);
            return GetResult(processor.DbProcessor, result);
        }
    }
}
