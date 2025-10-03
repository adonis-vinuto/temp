using Application.DTOs.Knowledge.ImportSenior;
using System.Xml.Linq;

namespace Application.Interfaces.Services;
public interface ISeniorService
{
    public Task<XDocument> SendRequest(Senior senior);
    public Task SalvaDadosSalaryHistory(XDocument document, Guid idKnowledge);
    public Task SalvaDadosEmployee(XDocument document, Guid idKnowledge);
    public Task SalvaDadosPayroll(XDocument document, Guid idKnowledge);
}

