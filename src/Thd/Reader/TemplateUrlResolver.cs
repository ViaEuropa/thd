using Scriban;

namespace Thd.Reader;

public sealed class TemplateUrlResolver : IResolveUrl
{
    private readonly Template _template;

    public TemplateUrlResolver(string template)
    {
        _template = Template.Parse(template);
    }

    public ResolvedUrl ResolveUrl(ReplayData url)
    {
        string baseUrl = ParseBaseUrl(_template, url);

        var destination = new Uri(baseUrl + url.Path);

        return new ResolvedUrl(baseUrl, destination);
    }

    private static string ParseBaseUrl(Template template, ReplayData url)
    {
        var rendered = template.Render(url.RoutingData);
        return rendered;
    }
}