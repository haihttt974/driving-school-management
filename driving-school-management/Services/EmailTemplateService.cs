namespace driving_school_management.Services
{
    public interface IEmailTemplateService
    {
        Task<string> GetTemplateAsync(string templateName);
        string ReplacePlaceholders(string templateContent, Dictionary<string, string> values);
    }
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IWebHostEnvironment _environment;

        public EmailTemplateService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> GetTemplateAsync(string templateName)
        {
            var templatePath = Path.Combine(_environment.ContentRootPath, "Templates", "Email", templateName);

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Không tìm thấy template email: {templateName}");
            }

            return await File.ReadAllTextAsync(templatePath);
        }

        public string ReplacePlaceholders(string templateContent, Dictionary<string, string> values)
        {
            foreach (var item in values)
            {
                templateContent = templateContent.Replace(item.Key, item.Value);
            }

            return templateContent;
        }
    }
}