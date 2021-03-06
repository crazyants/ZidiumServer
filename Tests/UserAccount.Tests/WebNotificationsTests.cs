﻿using System;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core.Api;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.TestTools;
using Zidium.UserAccount.Controllers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Tests
{
    public class WebNotificationsTests
    {
        [Fact]
        public void NotificationsListTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var component = account.CreateRandomComponentControl();

            var eventType = TestHelper.GetTestEventType(account.Id);
            var dispatcher = DispatcherHelper.GetDispatcherService();
            var eventRequest = new SendEventRequest()
            {
                Token = account.GetCoreToken(),
                Data = new SendEventData()
                {
                    ComponentId = component.Info.Id,
                    TypeSystemName = eventType.SystemName,
                    Category = EventCategory.ComponentEvent,
                    Version = "1.2.3.4",
                    JoinInterval = TimeSpan.FromSeconds(0).TotalSeconds
                }
            };
            var sendEventResponse = dispatcher.SendEvent(eventRequest);
            Assert.True(sendEventResponse.Success);
            var eventId = sendEventResponse.Data.EventId;

            var dispatcherClient = TestHelper.GetDispatcherClient();
            var response = dispatcherClient.CreateSubscription(
                account.Id, 
                new CreateSubscriptionRequestData()
            {
                UserId = user.Id,
                Object = SubscriptionObject.ComponentType,
                ComponentTypeId = component.Type.Info.Id,
                Channel = SubscriptionChannel.Email
            });
            var subscription = response.Data;

            Notification notification;
            using (var accountDbContext = account.CreateAccountDbContext())
            {
                var repository = accountDbContext.GetNotificationRepository();
                notification = new Notification()
                {
                    Id = Guid.NewGuid(),
                    CreationDate = DateTime.Now,
                    EventId = eventId,
                    SendDate = DateTime.Now.AddDays(1),
                    Status = NotificationStatus.Sended,
                    SubscriptionId = subscription.Id,
                    Type = Core.AccountsDb.NotificationType.Email,
                    UserId = user.Id,
                    Address = "test@mail.ru"
                };
                notification = repository.Add(notification);
                accountDbContext.SaveChanges();
            }

            using (var controller = new NotificationsController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Index(component.Info.Id);
                var model = (NotificationsListModel)result.Model;
                var item = model.Notifications.SingleOrDefault(t => t.Id == notification.Id);
                Assert.NotNull(item);
                Assert.Equal(notification.EventId, item.Event.Id);
                Assert.Equal(notification.Status, item.Status);
                Assert.Equal(notification.Type, item.Channel);
                Assert.Equal(notification.UserId, item.User.Id);
            }
        }

        [Fact]
        public void NotificationShowTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var component = account.CreateRandomComponentControl();

            var eventType = TestHelper.GetTestEventType(account.Id);
            var dispatcher = DispatcherHelper.GetDispatcherService();
            var eventRequest = new SendEventRequest()
            {
                Token = account.GetCoreToken(),
                Data = new SendEventData()
                {
                    ComponentId = component.Info.Id,
                    TypeSystemName = eventType.SystemName,
                    Category = EventCategory.ComponentEvent,
                    Version = "1.2.3.4",
                    JoinInterval = TimeSpan.FromSeconds(0).TotalSeconds
                }
            };
            var sendEventResponse = dispatcher.SendEvent(eventRequest);
            Assert.True(sendEventResponse.Success);
            var eventId = sendEventResponse.Data.EventId;

            var dispatcherClient = TestHelper.GetDispatcherClient();
            var response = dispatcherClient.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
            {
                UserId = user.Id,
                Object = SubscriptionObject.ComponentType,
                Channel = SubscriptionChannel.Email,
                ComponentTypeId = component.Type.Info.Id
            }); 
            var subscription = response.Data;

            Notification notification;
            using (var accountDbContext = account.CreateAccountDbContext())
            {
                var repository = accountDbContext.GetNotificationRepository();
                notification = new Notification()
                {
                    Id = Guid.NewGuid(),
                    CreationDate = DateTime.Now,
                    EventId = eventId,
                    SendDate = DateTime.Now.AddDays(1),
                    Status = NotificationStatus.Sended,
                    SubscriptionId = subscription.Id,
                    Type = NotificationType.Email,
                    UserId = user.Id,
                    Address = "test@mail.ru"
                };
                notification = repository.Add(notification);
                accountDbContext.SaveChanges();
            }

            using (var controller = new NotificationsController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Show(notification.Id, component.Info.Id);
                var model = (NotificationDetailsModel)result.Model;
                Assert.Equal(notification.Id, model.Id);
                Assert.Equal(notification.EventId, model.Event.Id);
                Assert.Equal(notification.SendError, model.SendError);
                Assert.Equal(notification.Status, model.Status);
                Assert.Equal(notification.SubscriptionId, model.Subscription.Id);
                Assert.Equal(notification.Type, model.Channel);
                Assert.Equal(notification.UserId, model.User.Id);
                Assert.Equal(notification.Address, model.Address);
            }
        }

    }
}
