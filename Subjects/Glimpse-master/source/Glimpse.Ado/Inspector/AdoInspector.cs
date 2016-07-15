using Glimpse.Ado.Inspector.Core;
using Glimpse.Core.Extensibility;

namespace Glimpse.Ado.Inspector
{
    public class AdoInspector : IInspector
    {
        public void Setup(IInspectorContext context)
        {
            AdoExecutionBlock.Instance.Execute();
        } 
    }
}