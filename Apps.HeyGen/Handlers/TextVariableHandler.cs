using Blackbird.Applications.Sdk.Common;
using Apps.HeyGen.Api;
using Apps.HeyGen.Models.Dtos;
using Apps.HeyGen.Models.Inputs;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.HeyGen.Handlers;

public class TextVariableHandler(InvocationContext invocationContext, [ActionParameter] TemplateInput input)
    : Invocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input?.TemplateId))
            return [];

        var id = Requests.Id(input.TemplateId, "a template ID");
        var template = await Client.Data<Template>(new RestRequest($"templates/{id}"));
        return template.Variables.Where(x => x.Value.Type == "text" && Requests.Matches(context.SearchString, x.Key))
            .Select(x => new DataSourceItem(x.Key, x.Key));
    }
}
