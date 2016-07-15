﻿using System;
using System.Collections.Generic;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Extensions;
using Glimpse.Core.Tab.Assist;
using Glimpse.Mvc.Model;

namespace Glimpse.Mvc.SerializationConverter
{
    public class ListOfExecutionModelConverter : SerializationConverter<List<ExecutionModel>>
    {
        public override object Convert(List<ExecutionModel> models)
        {
            var ordinal = 0;

            var section = new TabSection("Ordinal", "Controller", "Action", "Is Child", "Category", "Category", "Type", "Method", "Time Elapsed");
            foreach (var model in models)
            {
                section.AddRow().Column(ordinal++).Column(model.ControllerName).Column(model.ActionName).Column(model.IsChildAction).Column(model.Category.ToStringOrDefault()).Column(model.Bounds.ToStringOrDefault()).Column(model.ExecutedType).Column(model.ExecutedMethod).Column(model.Duration).SelectedIf(!model.Category.HasValue);
            }

            return section.Build();
        }
    }
}