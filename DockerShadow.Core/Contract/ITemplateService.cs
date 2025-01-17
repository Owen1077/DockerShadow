namespace DockerShadow.Core.Contract
{
    public interface ITemplateService
    {
        string GenerateHtmlStringFromViewsAsync<T>(string viewName, T model);
    }
}
