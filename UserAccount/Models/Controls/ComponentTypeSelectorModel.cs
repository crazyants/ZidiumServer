﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Zidium.Core.AccountsDb;
using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount.Models
{
    public class ComponentTypeSelectorModel : SelectorModel
    {
        public ComponentTypeSelectorMode Mode { get; set; }

        public Guid? ComponentTypeId { get; set; }

        public ComponentTypeSelectorModel(
            string name, 
            Guid? componentTypeId, 
            bool allowEmpty, 
            bool autoRefreshPage, 
            bool hideWhenFilter = false, 
            Expression<Func<object, object>> expression = null, 
            HtmlHelper<object> htmlHelper = null)
        {
            Name = name;
            ComponentTypeId = componentTypeId;
            AllowEmpty = allowEmpty;
            AutoRefreshPage = autoRefreshPage;
            HideWhenFilter = hideWhenFilter;
            Expression = expression;
            HtmlHelper = htmlHelper;
        }

        public ComponentTypeSelectorModel(
            HtmlHelper htmlHelper, 
            string name, 
            ComponentTypeSelectorOptions options)
        {
            Name = name;
            HtmlHelperBase = htmlHelper;
            if (options != null)
            {
                ComponentTypeId = options.SelectedValue;
                AllowEmpty = options.AllowEmpty;
                AutoRefreshPage = options.AutoRefreshPage;
                HideWhenFilter = options.HideWhenFilter;
                Mode = options.Mode;
            }
        }

        public List<SelectListItem> GetTypeItems()
        {
            var items = new List<SelectListItem>();
            if (Mode == ComponentTypeSelectorMode.ShowAllTypes)
            {
                items = GuiHelper.GetAccountComponentTypes(ComponentTypeId, AllowEmpty);
            }
            else if (Mode == ComponentTypeSelectorMode.ShowOnlyUsedTypes)
            {
                items = GuiHelper.GetUsedAccountComponentTypes(ComponentTypeId, AllowEmpty);
            }
            if (AllowEmpty == false && (ComponentTypeId == null || ComponentTypeId==Guid.Empty))
            {
                var others = items.FirstOrDefault(x => x.Value == SystemComponentTypes.Others.Id.ToString());
                if (others != null)
                {
                    others.Selected = true;
                }
            }
            return items;
        }
    }
}