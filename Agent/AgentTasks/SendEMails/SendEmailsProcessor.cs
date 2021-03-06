﻿using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using MailKit.Security;
using MimeKit;
using NLog;
using Zidium.Api;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;

namespace Zidium.Agent.AgentTasks.SendEMails
{
    public class SendEmailsProcessor
    {
        public string SmtpLogin { get; set; }

        public string SmtpFrom { get; set; }

        public string SmtpPassword { get; set; }

        public string SmtpServer { get; set; }

        public int SmtpPort { get; set; }

        public bool UseMailKit { get; set; }

        public bool UseSsl { get; set; }

        public bool FakeMode { get; set; }

        public MultipleDataBaseProcessor DbProcessor { get; protected set; }

        public int SuccessSendCount;

        public int SendMaxCount = 100;

        public SendEmailsProcessor(
            ILogger logger, CancellationToken cancellationToken,
            string smtpServer,
            int smtpPort,
            string smtpLogin,
            string smtpFrom,
            string smtpPassword,
            bool useMailKit,
            bool useSsl)
        {
            DbProcessor = new MultipleDataBaseProcessor(logger, cancellationToken);
            SmtpServer = smtpServer;
            SmtpPort = smtpPort;
            SmtpLogin = smtpLogin;
            SmtpFrom = smtpFrom;
            SmtpPassword = smtpPassword;
            UseMailKit = useMailKit;
            UseSsl = useSsl;
        }

        public void Process(Guid? accountId = null, Guid? emailId = null)
        {
            DbProcessor.ForEachAccount(data =>
            {
                if (accountId == null)
                    ProcessAccount(data);
                else if (accountId.Value == data.Account.Id)
                    ProcessAccount(data, emailId);
            });
        }

        public void SendEmail(IDisposable smtpClient, string to, string subject, string body, bool isHtml)
        {
            if (FakeMode)
            {
                return;
            }

            if (smtpClient == null)
                smtpClient = GetSmtpClient();

            if (!UseMailKit)
            {
                SendStandardEmail((SmtpClient)smtpClient, to, subject, body, isHtml);
            }
            else
            {
                SendMailKitEmail((MailKit.Net.Smtp.SmtpClient)smtpClient, to, subject, body, isHtml);
            }
        }

        protected void SendStandardEmail(SmtpClient smtpClient, string to, string subject, string body, bool isHtml)
        {
            using (var message = new MailMessage())
            {
                message.From = new MailAddress(SmtpLogin, SmtpFrom);
                message.To.Add(new MailAddress(to));
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = isHtml;
                smtpClient.Send(message);
            }
        }

        protected void SendMailKitEmail(MailKit.Net.Smtp.SmtpClient smtpClient, string to, string subject, string body, bool isHtml)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(SmtpFrom, SmtpLogin));
            message.To.Add(new MailboxAddress(to, to));
            message.Subject = subject;
            var builder = new BodyBuilder();
            if (isHtml)
            {
                builder.HtmlBody = body;
            }
            else
            {
                builder.TextBody = body;
            }
            message.Body = builder.ToMessageBody();
            smtpClient.Send(message);
        }

        protected IDisposable GetSmtpClient()
        {
            if (!UseMailKit)
            {
                var smtpClient = new SmtpClient(SmtpServer, SmtpPort);
                var credentials = new NetworkCredential(SmtpLogin, SmtpPassword);
                smtpClient.EnableSsl = UseSsl;
                smtpClient.Credentials = credentials;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                return smtpClient;
            }
            else
            {
                var smtpClient = new MailKit.Net.Smtp.SmtpClient();
                if (!FakeMode)
                {
                    smtpClient.Connect(SmtpServer, SmtpPort, UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.Auto);

                    // Иногда не удаётся зайти в ящик, на следующий раз всё ок
                    // TODO сделать счётчик неудачных попыток 
                    try
                    {
                        smtpClient.Authenticate(SmtpLogin, SmtpPassword);
                    }
                    catch (AuthenticationException exception)
                    {
                        if (exception.Message.StartsWith("Authentication failed.", StringComparison.OrdinalIgnoreCase))
                        {
                            // Отменяем обработку до следующего запуска
                            throw new OperationCanceledException();
                        }

                        throw;
                    }
                }
                return smtpClient;
            }
        }

        protected void ProcessAccount(ForEachAccountData data, Guid? emailId = null)
        {
            var account = data.Account;

            using (var smtpClient = GetSmtpClient())
            {
                var repository = data.AccountDbContext.GetSendEmailCommandRepository();

                // важно отсортировать письма в порядке их создания
                var emails = repository
                    .GetForSend(SendMaxCount)
                    .OrderBy(x => x.CreateDate)
                    .ToArray();

                if (emailId.HasValue)
                    emails = emails.Where(t => t.Id == emailId.Value).ToArray();

                var count = emails.Length;
                if (count > 0)
                    data.Logger.Debug("Всего писем для отправки: " + count);
                else
                    data.Logger.Trace("Нет писем для отправки");

                var lastSendTime = DateTime.MinValue;

                foreach (var email in emails)
                {
                    data.CancellationToken.ThrowIfCancellationRequested();
                    data.Logger.Debug("Отправляем: {0} Тема: {1}", email.To, email.Subject);
                    try
                    {
                        // todo в почтовом клиенте thunderbird письма иногда не верно сортируются по дате
                        // есть предположение, что это из-за того, что дата создания округляется до секунд
                        // поэтому искуственно сделаем так, чтобы дата отправки писем отличалась как минимум на 1 секунду
                        DateTimeHelper.WaitForTime(lastSendTime.AddMilliseconds(1100));

                        // отправим сообщение
                        SendEmail(smtpClient, email.To, email.Subject, email.Body, email.IsHtml);

                        // отправим событие = чтобы знать, сколько писем в день отправлено
                        /*
                        DbProcesor.ComponentControl
                            .CreateComponentEvent("Отправка почты")
                            .SetJoinInterval(TimeSpan.FromDays(1))
                            .SetImportance(EventImportance.Unknown)
                            .SetJoinKey(DateTime.Now.ToString("yyyy.MM.dd"))
                            .Add();
                        */
                        // TODO Тут надо писать метрику

                        // обновим статус  и статистику
                        repository.MarkAsSendSuccessed(email.Id);
                        Interlocked.Increment(ref SuccessSendCount);

                        data.Logger.Info("Отправлено письмо на адрес '{0}' с темой '{1}'", email.To, email.Subject);
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception exception)
                    {
                        if (!IsRatelimitException(exception))
                        {
                            DbProcessor.SetException(exception);

                            exception.Data.Add("EMailId", email.Id);
                            data.Logger.Error(exception);

                            repository.MarkAsSendFail(email.Id, exception.Message);
                        }
                        else
                        {
                            // Особый случай - превышение лимита нашего ящика
                            // За ошибку не считаем, в следующий раз отправится

                            data.Logger.Error(exception, "Превышен лимит отправки ящика Zidium");

                            // Отправлять дальше нет смысла
                            break;
                        }
                    }
                }
            }
        }

        protected bool IsRatelimitException(Exception exception)
        {
            var result = exception.Message.IndexOf("Ratelimit exceeded for mailbox", StringComparison.OrdinalIgnoreCase) >= 0;
            return result;
        }
    }
}
