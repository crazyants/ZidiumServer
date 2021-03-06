﻿using System;

namespace Zidium.Agent.AgentTasks.DeleteEvents
{
    public class DeleteComponentEventsTask : AgentTaskBase
    {
        public DeleteComponentEventsTask()
        {
            ExecutionPeriod = TimeSpan.FromHours(1);
            WaitOnErrorTime = TimeSpan.FromHours(1);
        }

        protected override AgentTaskResult Do()
        {
            var processor = new DeleteComponentEventsProcessor(Logger, CancellationToken);
            processor.Process();

            var result = string.Format(
                "Удалено событий: {0}; удалено свойств: {1}",
                processor.DeletedEventsCount,
                processor.DeletedPropertiesCount);

            return GetResult(processor.DbProcessor, result);
        }
    }
}
