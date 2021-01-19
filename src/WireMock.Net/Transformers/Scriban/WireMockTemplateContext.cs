using Scriban;
using Scriban.Runtime;
using WireMock.Types;

namespace WireMock.Transformers.Scriban
{
    internal class WireMockTemplateContext: TemplateContext
    {
        protected override IObjectAccessor GetMemberAccessorImpl(object target)
        {
            if (target?.GetType().GetGenericTypeDefinition() == typeof(WireMockList<>))
            {
                return new WireMockListAccessor();
            }

            return base.GetMemberAccessorImpl(target);
        }
    }
}