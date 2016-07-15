using System.Collections.Generic;

namespace Glimpse.Core.Tab.Assist
{
    public class TabObject : ITabBuild
    {
        private readonly IList<TabObjectRow> rows = new List<TabObjectRow>();

        public TabObjectRow AddRow()
        {
            var row = new TabObjectRow();
            rows.Add(row);
            return row;
        }

        public object Build()
        {
            var dictionary = new Dictionary<object, object>();
            foreach (var row in rows)
            {
                var valueRow = row.BaseValue as ITabBuild;
                dictionary[row.BaseKey] = valueRow != null ? valueRow.Build() : row.BaseValue;
            }

            return dictionary;
        } 
    }
}