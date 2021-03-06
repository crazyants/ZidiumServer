﻿using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core.AccountsDb.Classes.UnitTests.HttpTests;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Правило http-проверки
    /// </summary>
    public class HttpRequestUnitTestRule
    {
        public HttpRequestUnitTestRule()
        {
            Datas = new HashSet<HttpRequestUnitTestRuleData>();    
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Ссылка на настройку http-проверки
        /// </summary>
        public Guid HttpRequestUnitTestId { get; set; }

        /// <summary>
        /// Http-проверка
        /// </summary>
        public virtual HttpRequestUnitTest HttpRequestUnitTest { get; set; }

        /// <summary>
        /// Порядковый номер
        /// </summary>
        public int SortNumber { get; set; }

        /// <summary>
        /// Дружелюбное имя правила
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Урл запроса
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Метод запроса
        /// </summary>
        public HttpRequestMethod Method { get; set; }

        /// <summary>
        /// Код ответа, который должен быть
        /// </summary>
        public int? ResponseCode { get; set; }

        /// <summary>
        /// Максимальный размер ответа
        /// </summary>
        public int MaxResponseSize { get; set; }

        /// <summary>
        /// Фрагмент Html, который должен быть в ответе, если ошибок нет
        /// </summary>
        public string SuccessHtml { get; set; }

        /// <summary>
        /// Фрагмент Html который должен быть при ошибке
        /// </summary>
        public string ErrorHtml { get; set; }

        /// <summary>
        /// Максимальное время выполнения запроса
        /// </summary>
        public int? TimeoutSeconds { get; set; }

        /// <summary>
        /// Результат последнего выполнения
        /// </summary>
        public HttpRequestErrorCode? LastRunErrorCode { get; set; }

        /// <summary>
        /// Последняя ошибка, если была
        /// </summary>
        public string LastRunErrorMessage { get; set; }

        /// <summary>
        /// Время последнего запуска
        /// </summary>
        public DateTime? LastRunTime { get; set; }

        /// <summary>
        /// Длительность последнего запуска
        /// </summary>
        public int? LastRunDurationMs { get; set; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Данные для запроса
        /// </summary>
        public virtual ICollection<HttpRequestUnitTestRuleData> Datas { get; set; }

        public bool HasWebFormDatas()
        {
            return Datas.Any(x => x.Type == HttpRequestUnitTestRuleDataType.WebFormData);
        }

        public List<HttpRequestUnitTestRuleData> GetWebFormsDatas()
        {
            return Datas.Where(x => x.Type == HttpRequestUnitTestRuleDataType.WebFormData).ToList();
        }

        public List<HttpRequestUnitTestRuleData> GetRequestHeaders()
        {
            return Datas.Where(x => x.Type == HttpRequestUnitTestRuleDataType.RequestHeader).ToList();
        }

        public List<HttpRequestUnitTestRuleData> GetRequestCookies()
        {
            return Datas.Where(x => x.Type == HttpRequestUnitTestRuleDataType.RequestCookie).ToList();
        }
    }
}
