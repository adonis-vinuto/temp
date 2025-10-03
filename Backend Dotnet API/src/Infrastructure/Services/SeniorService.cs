using Application.DTOs.Knowledge.ImportSenior;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Infrastructure.Services;

public class SeniorService : ISeniorService
{
    public const String SoapEnvelopeNs = "http://schemas.xmlsoap.org/soap/envelope/";
    public const String SeniorServicesNs = "http://services.senior.com.br";
    public const String MediaType = "text/xml";
    public static readonly Encoding Encoding = Encoding.UTF8;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ISalaryHistoryRepository _salaryHistoryRepository;
    private readonly IPayrollRepository _payrollRepository;

    public SeniorService(IEmployeeRepository employeeRepository, ISalaryHistoryRepository salaryHistoryRepository, IPayrollRepository payrollRepository)
    {
        _employeeRepository = employeeRepository;
        _salaryHistoryRepository = salaryHistoryRepository;
        _payrollRepository = payrollRepository;
    }

    private static XDocument ConstructSoapRequest(Senior senior)
    {
        XNamespace soapenv = SoapEnvelopeNs;
        XNamespace ser = SeniorServicesNs;

        return new XDocument(
            new XElement(soapenv + "Envelope",
                new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                new XAttribute(XNamespace.Xmlns + "ser", ser),
                new XElement(soapenv + "Header"),
                new XElement(soapenv + "Body",
                    new XElement(ser + senior.Service,
                        new XElement("user", senior.LoginSenior),
                        new XElement("password", senior.PasswordSenior),
                        new XElement("encryption", senior.Encryption),
                        new XElement("parameters",
                            new XElement(senior.Service == "biPessoas" ? "dataCadastro" : "dataReferencia", senior.DataReferencia),
                            !string.IsNullOrWhiteSpace(senior.Parameters)
                                ? XElement.Parse(senior.Parameters)
                                : null
                        )
                    )
                )
            )
        );
    }

    public async Task<XDocument> SendRequest(Senior senior)
    {
        try
        {
            XDocument soapBody = ConstructSoapRequest(senior);
            String soapString = soapBody.ToString();

            using var httpClient = new System.Net.Http.HttpClient();
            httpClient.DefaultRequestHeaders.Add("SOAPAction", senior.Uri.ToString());

            using var content = new StringContent(soapString, Encoding, MediaType);
            HttpResponseMessage response = await httpClient.PostAsync(senior.Uri, content);

            response.EnsureSuccessStatusCode();

            string? responseString = await response.Content.ReadAsStringAsync();
            var soap = XDocument.Parse(responseString);

            return soap;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Failed to send SOAP request: {ex.Message}", ex);
        }
        catch (XmlException ex)
        {
            throw new Exception($"Failed to parse SOAP response: {ex.Message}", ex);
        }
    }

    public async Task SalvaDadosSalaryHistory(XDocument document, Guid idKnowledge)
    {
        var retornos = document
            .Descendants("retorno")
            .Select(r =>
            {
                if (!DateTime.TryParseExact((string)r.Element("rdatalt") ?? "", "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime changeDate))
                {
                    changeDate = DateTime.MinValue;
                }

                if (!decimal.TryParse((string)r.Element("rvalsal") ?? "0", NumberStyles.Any, CultureInfo.InvariantCulture, out decimal newSalary))
                {
                    newSalary = 0m;
                }

                string idEmployee = $"{(string)r.Element("rnumemp")}-{(string)r.Element("rtipcol")}-{(string)r.Element("rnumcad")}";

                return new SalaryHistory
                {
                    IdEmployee = idEmployee,
                    ChangeDate = changeDate,
                    NewSalary = newSalary,
                    EmployeeCodSeniorNumCad = (string)r.Element("rnumcad") ?? string.Empty,
                    CompanyCodSeniorNumEmp = (string)r.Element("rnumemp") ?? string.Empty,
                    MotiveCodSeniorCodMot = (string)r.Element("rcodmot") ?? string.Empty,
                    MotiveName = (string)r.Element("rnommot") ?? string.Empty,
                    CompanyCodSeniorCodFil = (string)r.Element("codfil") ?? string.Empty
                };
            })
            .ToList();

        foreach (SalaryHistory r in retornos)
        {
            SalaryHistory salaryExist = await _salaryHistoryRepository.SearchSalaryHistoryByIdAndIdKnowledge(r.Id, idKnowledge, CancellationToken.None);
            Employee employee = await _employeeRepository.SearchByIdAsync(idKnowledge, r.IdEmployee, CancellationToken.None);
            if (salaryExist is null && employee is not null)
            {
                await _salaryHistoryRepository.CreateSalaryHistory(r, CancellationToken.None);
                await _salaryHistoryRepository.UnitOfWork.Commit();
            }
        }
    }

    public async Task SalvaDadosEmployee(XDocument document, Guid idKnowledge)
    {
        var retornos = document
            .Descendants("retorno")
            .Select(r =>
            {
                if (!DateTime.TryParseExact((string)r.Element("datadm") ?? "", "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime admissionDate))
                {
                    admissionDate = DateTime.MinValue;
                }

                if (!DateTime.TryParseExact((string)r.Element("datafa") ?? "", "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime terminationDate))
                {
                    terminationDate = DateTime.MinValue;
                }

                if (!DateTime.TryParseExact((string)r.Element("datnas") ?? "", "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate))
                {
                    birthDate = DateTime.MinValue;
                }

                if (!DateTime.TryParseExact((string)r.Element("datsal") ?? "", "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime salaryEffectiveDate))
                {
                    salaryEffectiveDate = DateTime.MinValue;
                }

                if (!decimal.TryParse((string)r.Element("valsal") ?? "0", NumberStyles.Any, CultureInfo.InvariantCulture, out decimal salary))
                {
                    salary = 0m;
                }

                if (!decimal.TryParse((string)r.Element("cplsal") ?? "0", NumberStyles.Any, CultureInfo.InvariantCulture, out decimal complementarySalary))
                {
                    complementarySalary = 0m;
                }

                string id = $"{(string)r.Element("numemp")}-{(string)r.Element("tipcol")}-{(string)r.Element("numcad")}";

                return new Employee
                {
                    Id = id,
                    IdKnowledge = idKnowledge,
                    CompanyName = (string)r.Element("apeemp"),
                    FullName = (string)r.Element("nomfun"),
                    AdmissionDate = admissionDate,
                    TerminationDate = terminationDate,
                    StatusDescription = (string)r.Element("dessit"),
                    BirthDate = birthDate,
                    Salary = salary,
                    ComplementarySalary = complementarySalary,
                    SalaryEffectiveDate = salaryEffectiveDate,
                    StreetAddress = (string)r.Element("endrua"),
                    AddressNumber = (string)r.Element("endnum"),
                    CityName = (string)r.Element("nomcid"),
                    Race = (string)r.Element("raccor"),
                    Gender = (string)r.Element("tipsex") == "M" ? Gender.Male : Gender.Female,
                    PostalCode = (string)r.Element("endcep"),
                    CostCenterCodSeniorCodCcu = (string)r.Element("endcep"),
                    StatusCodSenior = (string)r.Element("sitafa"),
                    CollaboratorTypeCodeSeniorTipeCol = (string)r.Element("tipcol"),
                    CompanyCodSeniorNumEmp = (string)r.Element("numemp"),
                    EmployeeCodSeniorNumCad = (string)r.Element("numcad"),
                    CostCneterName = (string)r.Element("codccu")
                };
            })
            .ToList();

        foreach (Employee r in retornos)
        {
            Employee employeeExist = await _employeeRepository.SearchByIdAsync(r.IdKnowledge, r.Id, CancellationToken.None);
            if (employeeExist is null)
            {
                await _employeeRepository.AddAsync(r, CancellationToken.None);
                await _employeeRepository.UnitOfWork.Commit();
            }
        }
    }

    public async Task SalvaDadosPayroll(XDocument document, Guid idKnowledge)
    {
        var retornos = document
            .Descendants("retorno")
            .Select(r =>
            {
                if (!DateTime.TryParseExact((string)r.Element("perref") ?? "", "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime referenceDate))
                {
                    referenceDate = DateTime.MinValue;
                }

                if (!decimal.TryParse((string)r.Element("cplsal") ?? "0", NumberStyles.Any, CultureInfo.InvariantCulture, out decimal eventAmount))
                {
                    eventAmount = 0m;
                }

                string idEmployee = $"{(string)r.Element("numemp")}-{(string)r.Element("tipcol")}-{(string)r.Element("numcad")}";

                return new Payroll
                {
                    IdEmployee = idEmployee,
                    PayrollPeriodCod = (string)r.Element("codcal") ?? string.Empty,
                    EventName = (string)r.Element("deseve") ?? string.Empty,
                    EventAmount = eventAmount,
                    EventTypeName = (string)r.Element("tipeve") ?? string.Empty,
                    ReferenceDate = referenceDate,
                    CalculationTypeName = (string)r.Element("descal") ?? string.Empty,
                    CollaboratorTypeCodeSeniorTipCol = (string)r.Element("tipcol") ?? string.Empty,
                    PayrollPeriodCodSeniorCodCal = (string)r.Element("codcal") ?? string.Empty,
                    EventTypeCodSeniorTipEve = (string)r.Element("tipeve") ?? string.Empty,
                    CalculationTypeCodSeniorTipCal = (string)r.Element("tipcal") ?? string.Empty,
                    CompanyCodSeniorNumEmp = (string)r.Element("numemp") ?? string.Empty,
                    EmployeeCodSeniorNumCad = (string)r.Element("numcad") ?? string.Empty,
                    EventCodSeniorCodenv = (string)r.Element("codeve") ?? string.Empty
                };
            })
            .ToList();

        foreach (Payroll r in retornos)
        {
            Employee employeeExist = await _employeeRepository.SearchByIdAsync(idKnowledge, r.IdEmployee, CancellationToken.None);
           
            if (employeeExist is not null)
            {
                await _payrollRepository.AddAsync(r, CancellationToken.None);
                await _payrollRepository.UnitOfWork.Commit();
            }
        }
    }
}
