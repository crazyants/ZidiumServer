﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Zidium.Api.XmlConfig
{
    public class ConfigHelper
    {
        private static string GetString(XmlElement element, string property)
        {
            if (element.HasAttributes == false)
            {
                return null;
            }

            var attr = element
                .Attributes
                .Cast<XmlAttribute>()
                .FirstOrDefault(x => string.Equals(x.Name, property, StringComparison.OrdinalIgnoreCase));

            if (attr == null)
            {
                return null;
            }

            var result = attr.Value;
            if (string.IsNullOrEmpty(result))
            {
                return null;
            }
            return result;

        }

        private static LogLevel? GetLogLevel(XmlElement element, string property)
        {
            string value = GetString(element, property);
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            try
            {
                return (LogLevel) Enum.Parse(typeof (LogLevel), value, true);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static EventImportance? GetEventImportance(XmlElement element, string property)
        {
            string value = GetString(element, property);
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            try
            {
                return (EventImportance)Enum.Parse(typeof(EventImportance), value, true);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool GetBool(XmlElement element, string property)
        {
            string value = GetString(element, property);
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            return string.Equals("true", value, StringComparison.OrdinalIgnoreCase);
        }

        private static TimeSpan? GetTimeSpanFromSeconds(XmlElement element, string property)
        {
            int? seconds = GetInt32(element, property);
            if (seconds == null)
            {
                return null;
            }
            return TimeSpan.FromSeconds(seconds.Value);
        }

        private static Encoding GetEncoding(XmlElement element, string property)
        {
            string value = GetString(element, property);
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            try
            {
                return Encoding.GetEncoding(value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static int? GetInt32(XmlElement element, string property)
        {
            string value = GetString(element, property);
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            value = value.Replace(" ", "");
            int result = 0;
            if (IntHelper.TryParse(value, out result))
            {
                return result;
            }
            return null;
        }

        private static Uri GetUri(XmlElement element, string property)
        {
            string value = GetString(element, property);
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            try
            {
                return new Uri(value);
            }
            catch (FormatException)
            {
                return null;
            }
        }

        private static Guid? GetGuid(XmlElement element, string property)
        {
            string value = GetString(element, property);
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            try
            {
                return new Guid(value);
            }
            catch (FormatException)
            {
                return null;
            }
        }

        private static AccessElement GetAccessElement(XmlElement element)
        {
            if (element == null)
            {
                return new AccessElement();
            }
            var access = new AccessElement();
            access.AccountName = GetString(element, "accountName") ?? access.AccountName;
            access.SecretKey = GetString(element, "secretKey") ?? access.SecretKey;
            access.WaitOnError = GetTimeSpanFromSeconds(element, "waitOnErrorSeconds") ?? access.WaitOnError;
            access.Disable = GetBool(element, "disable");
            access.Url = GetString(element, "url");
            return access;
        }

        private static LogsElement GetLogsElement(XmlElement element)
        {
            if (element == null)
            {
                return new LogsElement();
            }

            var logs = new LogsElement();

            // webLog
            var webElem = GetFirstChildElement(element, "webLog");
            logs.WebLog = GetWebLogElement(webElem);

            var internalLogElem = GetFirstChildElement(element, "internalLog");
            logs.InternalLog = GetInternalLogElement(internalLogElem);

            // auto create events
            var autoCreateEventsElem = GetFirstChildElement(element, "autoCreateEvents");
            logs.AutoCreateEvents = GetAutoCreateEventsElement(autoCreateEventsElem);

            return logs;
        }

        private static AutoCreateEventsElement GetAutoCreateEventsElement(XmlElement element)
        {
            if (element == null)
                return new AutoCreateEventsElement();

            var result = new AutoCreateEventsElement();
            result.Disable = GetBool(element, "Disable");

            return result;
        }

        private static DefaultEventValuesElement GetDefaultEventValuesElement(XmlElement element)
        {
            if (element == null)
            {
                return new DefaultEventValuesElement();
            }

            var defaultValues = new DefaultEventValuesElement();

            // componentEvent
            var componentEventElem = GetFirstChildElement(element, "componentEvent");
            defaultValues.ComponentEvent = GetEventCategoryDefaultValuesElement(componentEventElem, defaultValues.ComponentEvent);

            // applicationError
            var applicationErrorElem = GetFirstChildElement(element, "applicationError");
            defaultValues.ApplicationError = GetEventCategoryDefaultValuesElement(applicationErrorElem, defaultValues.ApplicationError);

            return defaultValues;
        }

        private static EventsElement GetEventsElement(XmlElement element)
        {
            if (element == null)
            {
                return new EventsElement();
            }

            var eventsElem = new EventsElement();

            // eventManager
            var eventManagerElem = GetFirstChildElement(element, "eventManager");
            eventsElem.EventManager = GetEventManagerElement(eventManagerElem);

            // defaultValues
            var defaultValuesElem = GetFirstChildElement(element, "defaultValues");
            eventsElem.DefaultValues = GetDefaultEventValuesElement(defaultValuesElem);

            return eventsElem;
        }

        private static WebLogElement GetWebLogElement(XmlElement element)
        {
            if (element == null)
            {
                return new WebLogElement();
            }
            var webLog = new WebLogElement();
            webLog.Disable = GetBool(element, "disable");
            webLog.Threads = GetInt32(element, "threads") ?? webLog.Threads;
            webLog.BatchBytes = GetInt32(element, "batchBytes") ?? webLog.BatchBytes;
            webLog.SendPeriod = GetTimeSpanFromSeconds(element, "sendPeriodSeconds") ?? webLog.SendPeriod;
            webLog.ReloadConfigsPeriod = GetTimeSpanFromSeconds(element, "reloadConfigsPeriodSeconds") ?? webLog.ReloadConfigsPeriod;
            webLog.QueueBytes = GetInt32(element, "queueBytes") ?? webLog.QueueBytes;
            return webLog;
        }

        private static EventManagerElement GetEventManagerElement(XmlElement element)
        {
            if (element == null)
            {
                return new EventManagerElement();
            }
            var eventManager = new EventManagerElement();
            eventManager.Disabled = GetBool(element, "disable");
            eventManager.SendPeriod = GetTimeSpanFromSeconds(element, "sendPeriodSeconds") ?? eventManager.SendPeriod;
            eventManager.QueueBytes = GetInt32(element, "queueBytes") ?? eventManager.QueueBytes;
            eventManager.Threads = GetInt32(element, "threads") ?? eventManager.Threads;
            eventManager.MaxSend = GetInt32(element, "maxSend") ?? eventManager.MaxSend;
            eventManager.MaxJoin = GetInt32(element, "maxJoin") ?? eventManager.MaxJoin;
            return eventManager;
        }

        private static EventCategoryDefaultValuesElement GetEventCategoryDefaultValuesElement(
            XmlElement element,
            EventCategoryDefaultValuesElement node)
        {
            if (element == null)
            {
                return node;
            }
            node.JoinInterval = GetTimeSpanFromSeconds(element, "joinIntervalSeconds") ?? node.JoinInterval;
            return node;
        }

        private static InternalLogElement GetInternalLogElement(XmlElement element)
        {
            if (element == null)
            {
                return new InternalLogElement();
            }
            var log = new InternalLogElement();
            log.Disable = GetBool(element, "disable");
            log.Encoding = GetEncoding(element, "encoding") ?? log.Encoding;
            log.FilePath = GetString(element, "filePath") ?? log.FilePath;
            log.DeleteOldFileOnStartup = GetBool(element, "deleteOldFileOnStartup");
            log.MinLevel = GetLogLevel(element, "minLevel") ?? log.MinLevel;
            return log;
        }

        private static DefaultComponentElement GetDefaultComponentElement(XmlElement element)
        {
            if (element == null)
            {
                return new DefaultComponentElement();
            }
            var defaultComponent = new DefaultComponentElement();
            defaultComponent.Id = GetGuid(element, "id");
            return defaultComponent;
        }

        private static XmlElement GetFirstChildElement(XmlElement parent, string name)
        {
            var elemList = GetChildElements(parent, name);
            if (elemList.Count > 0)
            {
                return elemList[0];
            }
            return null;
        }

        private static List<XmlElement> GetChildElements(XmlElement parent, string name)
        {
            if (parent.HasChildNodes == false)
            {
                return new List<XmlElement>();
            }
            var result = new List<XmlElement>();
            foreach (XmlNode childNode in parent.ChildNodes)
            {
                var element = childNode as XmlElement;
                if (element != null && string.Equals(element.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(element);
                }
            }
            return result;
        }

        public static Config Load(string path)
        {
            var doc = new XmlDocument();
            doc.Load(path);
            var root = doc.DocumentElement;

            var config = new Config();

            // access
            var accessElem = GetFirstChildElement(root, "access");
            config.Access = GetAccessElement(accessElem);

            // defaultComponent
            var defaultComponent = GetFirstChildElement(root, "defaultComponent");
            config.DefaultComponent = GetDefaultComponentElement(defaultComponent);

            // logs
            var logsElem = GetFirstChildElement(root, "logs");
            config.Logs = GetLogsElement(logsElem);

            // events
            var eventsElem = GetFirstChildElement(root, "events");
            config.Events = GetEventsElement(eventsElem);

            return config;
        }

        /// <summary>
        /// Загружает конфиг из XML файла, если не получится, то вернет конфиг по умолчанию.
        /// </summary>
        /// <returns></returns>
        public static Config LoadFromXmlOrGetDefault()
        {
            try
            {
                string dir = Tools.GetApplicationDir();
                var files = new[] { "Zidium.xml", "Zidium.config" };
                foreach (var file in files)
                {
                    string path = Path.Combine(dir, file);
                    if (File.Exists(path))
                    {
                        var config = Load(path);
                        return config;
                    }
                }
            }
            catch
            {
                
            }
            return new Config();
        }
    }
}
