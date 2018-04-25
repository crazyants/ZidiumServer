﻿using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Api.Others;
using Zidium.Core.Api;
using Zidium.Core.AccountsDb;

namespace Zidium.Core.Caching
{
    public class EventCacheWriteObject : CacheWriteObjectBase<AccountCacheRequest, EventCacheResponse, IEventCacheReadObject, EventCacheWriteObject>, IEventCacheReadObject
    {
        protected EventCacheWriteObject()
        {
        }

        public new Guid Id { get; protected set; }
        
        /// <summary>
        /// Ссылка на аккаунт
        /// </summary>
        public Guid AccountId { get; protected set; }

        /// <summary>
        /// ИД владельца. Например, для события проверки - это ИД проверки.
        /// </summary>
        public Guid OwnerId { get; protected set; }

        /// <summary>
        /// Ссылка на тип события
        /// </summary>
        public Guid EventTypeId { get; protected set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Важность события
        /// </summary>
        public EventImportance Importance { get; set; }

        /// <summary>
        /// Предыдущая важность. Используется для событий статусов.
        /// </summary>
        public EventImportance PreviousImportance { get; set; }

        /// <summary>
        /// Счетчик - сколько раз случалось событие
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Хэш ключа по которому склеиваются события
        /// </summary>
        public long JoinKeyHash { get; protected set; }

        /// <summary>
        /// Дата создания события в системе мониторинга (время системы)
        /// </summary>
        public DateTime CreateDate { get; protected set; }

        /// <summary>
        /// Дата начала события (по местному времени аккаунта)
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Дата завершения события (по местному времени аккаунта)
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Время ДО которого событие можно считать актуальным
        /// </summary>
        public DateTime ActualDate { get; set; }

        /// <summary>
        /// Признак "событие - пробел" (пробел между колбасками)
        /// </summary>
        public bool IsSpace { get; set; }

        /// <summary>
        /// Время последнего изменения события.
        /// </summary>
        public DateTime LastUpdateDate { get; set; }

        /// <summary>
        /// Время последней обработки события для создания уведомлений
        /// </summary>
        public DateTime? LastNotificationDate { get; set; }

        /// <summary>
        /// Событие обработано человеком
        /// </summary>
        public bool IsUserHandled { get; set; }

        /// <summary>
        /// Версия компонента на момент появления события
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Версия в виде числа для сравнений
        /// </summary>
        public long? VersionLong { get; set; }

        /// <summary>
        /// Категория события
        /// </summary>
        public EventCategory Category { get; protected set; }

        ///// <summary>
        ///// Если True - значит для этого события нужно создать ArchivedStatus
        ///// </summary>
        //public bool IsArchivedStatus { get; set; }

        /// <summary>
        /// Длительность события
        /// </summary>
        public TimeSpan Duration
        {
            get { return GetDuration(DateTime.Now); }
        }

        public TimeSpan GetDuration(DateTime now)
        {
            if (ActualDate > now)
            {
                var duration = now - StartDate;
                if (duration < TimeSpan.Zero)
                {
                    return TimeSpan.Zero;
                }
                return duration;
            }
            return ActualDate - StartDate;
        }

        /// <summary>
        /// Ссылка на последний статус, для которого данное событие является причиной
        /// </summary>
        public Guid? LastStatusEventId { get; set; }

        /// <summary>
        /// Ссылка на первую причину данного статуса
        /// </summary>
        public Guid? FirstReasonEventId { get; set; }

        public TimeSpan GetActualInterval()
        {
            return ActualDate - EndDate;
        }

        private List<EventCacheStatusReference> _newStatuses = new List<EventCacheStatusReference>();

        public List<EventCacheStatusReference> NewStatuses
        {
            get { return _newStatuses; }
        }

        public void UpdateNewStatuses()
        {
            var statuses = _newStatuses.Where(x => x.Saved == false).ToList();
            _newStatuses = statuses;
        }

        private List<EventProperty> _newEventProperties = new List<EventProperty>();

        public List<EventProperty> NewEventProperties
        {
            get { return _newEventProperties; }
        } 

        public override EventCacheWriteObject GetCopy()
        {
            var copy = base.GetCopy();
            // создадим копию списка, чтобы в более старые несохраненные изменения НЕ попали статусы более новых изменений
            copy.UpdateNewStatuses();
            if (ReferenceEquals(_newStatuses, copy._newStatuses))
            {
                throw new Exception("ReferenceEquals(_newStatuses, copy._newStatuses)");
            }
            return copy;
        }

        public override int GetCacheSize()
        {
            return 16 // Id
                   + 4 // Count
                   + 8 // StartDate
                   + StringHelper.GetPropertySize(Version)
                   + 8 // VersionLong
                   + 16 // OwnerId
                   + 1 // MoreImportance
                   + StringHelper.GetPropertySize(Message)
                   + 8 // LastUpdateDate
                   + 16 // LastStatusEventId
                   + 8 // LastNotificationDate
                   + 8 // JoinKeyHash
                   + 1 // IsUserHandled
                   + 1 // IsSpace
                   + 4 // Importance
                   + 16 // EventTypeId
                   + 8 // EndDate
                   + 8 // CreateDate
                   + 4 // Category
                   + 8 // ActualDate
                   + 16 // AccountId
                   + 16; // FirstReasonEventId
        }

        public Event CreateEfEvent()
        {
            var result = new Event()
            {
                Id = Id,
                Count = Count,
                StartDate = StartDate,
                Version = Version,
                VersionLong = VersionLong,
                OwnerId = OwnerId,
                PreviousImportance = PreviousImportance,
                Message = Message,
                LastUpdateDate = LastUpdateDate,
                LastStatusEventId = LastStatusEventId,
                LastNotificationDate = LastNotificationDate,
                JoinKeyHash = JoinKeyHash,
                IsUserHandled = IsUserHandled,
                IsSpace = IsSpace,
                Importance = Importance,
                EventTypeId = EventTypeId,
                EndDate = EndDate,
                CreateDate = CreateDate,
                Category = Category,
                ActualDate = ActualDate,
                FirstReasonEventId = FirstReasonEventId
            };
            foreach (var property in NewEventProperties)
            {
                result.Properties.Add(property);
            }
            return result;
        }

        private static EventCacheWriteObject CreateInternal(Event eventObj, Guid accountId)
        {
            if (eventObj == null)
            {
                return null;
            }
            if (eventObj.CreateDate == DateTime.MinValue)
            {
                throw new Exception("eventObj.CreateDate == DateTime.MinValue");
            }
            if (eventObj.StartDate == DateTime.MinValue)
            {
                throw new Exception("eventObj.StartDate == DateTime.MinValue");
            }
            if (eventObj.EndDate == DateTime.MinValue)
            {
                throw new Exception("eventObj.EndDate == DateTime.MinValue");
            }
            if (eventObj.ActualDate == DateTime.MinValue)
            {
                throw new Exception("eventObj.ActualDate == DateTime.MinValue");
            }
            if (eventObj.LastUpdateDate == DateTime.MinValue)
            {
                throw new Exception("eventObj.LastUpdateDate == DateTime.MinValue");
            }
            var result = new EventCacheWriteObject()
            {
                Id = eventObj.Id,
                Count = eventObj.Count,
                StartDate = eventObj.StartDate,
                Version = eventObj.Version,
                VersionLong = eventObj.VersionLong,
                OwnerId = eventObj.OwnerId,
                PreviousImportance = eventObj.PreviousImportance,
                Message = eventObj.Message,
                LastUpdateDate = eventObj.LastUpdateDate,
                LastStatusEventId = eventObj.LastStatusEventId,
                LastNotificationDate = eventObj.LastNotificationDate,
                JoinKeyHash = eventObj.JoinKeyHash,
                IsUserHandled = eventObj.IsUserHandled,
                IsSpace = eventObj.IsSpace,
                Importance = eventObj.Importance,
                EventTypeId = eventObj.EventTypeId,
                EndDate = eventObj.EndDate,
                CreateDate = eventObj.CreateDate,
                Category = eventObj.Category,
                ActualDate = eventObj.ActualDate,
                AccountId = accountId,
                FirstReasonEventId = eventObj.FirstReasonEventId
            };
            return result;
        }

        public static EventCacheWriteObject CreateForAdd(Event eventObj, Guid accountId)
        {
            if (eventObj == null)
            {
                return null;
            }
            var result = CreateInternal(eventObj, accountId);

            // запомним доп свойства, чтобы их потом вставить
            foreach (var property in eventObj.Properties)
            {
                result.NewEventProperties.Add(property);
            }
            return result;
        }

        public static EventCacheWriteObject CreateForUpdate(Event eventObj, Guid accountId)
        {
            if (eventObj == null)
            {
                return null;
            }
            var result = CreateInternal(eventObj, accountId);

            // при изменении доп. свойства не используем и не меняем

            //foreach (var property in eventObj.Properties)
            //{
            //    result.NewEventProperties.Add(property);
            //}

            return result;
        }
    }
}
