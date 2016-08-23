using System;
using Templates;
using Templates.Data;
using Templates.Runtime;
using VitalChoice.Infrastructure.Domain.Mail;

namespace VitalChoice.ExportService.Services
{
    public interface IGiftListCreditCardExportFileGenerator
    {
        string GenerateText(GLOrdersImportEmail model);
    }

    public class GiftListCreditCardExportFileGenerator : IDisposable, IGiftListCreditCardExportFileGenerator
    {
        private readonly TtlTemplate _template;
        protected static string DocumentTemplate { get; } = @"@using(){{VitalChoice.Infrastructure.Domain.Mail}};
@model(){{GLOrdersImportEmail}};
Summary of the following Gift List Import completed @date(Date) {{MM'/'dd'/'yyyy hh:mm tt}}
Agent: @(Agent)
Customer Name: @(CustomerFirstName) @(CustomerLastName)
Customer #: @(IdCustomer)
Total # of imported orders: @(ImportedOrdersCount)
Total $ amount of orders imported: @money(ImportedOrdersAmount)
Credit Card Selected: @(CardNumber)";

        public GiftListCreditCardExportFileGenerator()
        {
            _template = new TtlTemplate(DocumentTemplate, new CompileContext(new TemplateOptions
            {
                AllowCSharp = false,
                ForceRemoveWhitespace = false
            }));
        }

        public string GenerateText(GLOrdersImportEmail model) => _template.Generate(model);

        public void Dispose() => _template.Dispose();
    }
}