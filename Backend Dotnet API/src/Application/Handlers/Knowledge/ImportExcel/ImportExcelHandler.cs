using System.Text.Json;
using Application.DTOs.Knowledge;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using ClosedXML.Excel;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.Knowledge.ImportExcel;

public class ImportExcelKnowledgeHandler : BaseHandler
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IModuleService _moduleService;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPayrollRepository _payrollRepository;
    private readonly ISalaryHistoryRepository _salaryHistoryRepository;

    public ImportExcelKnowledgeHandler(
            IAuthenticationService authenticationService,
            IModuleService moduleService,
            IEmployeeRepository employeeRepository,
            IPayrollRepository payrollRepository,
            ISalaryHistoryRepository salaryHistoryRepository)
    {
        _authenticationService = authenticationService;
        _moduleService = moduleService;
        _employeeRepository = employeeRepository;
        _payrollRepository = payrollRepository;
        _salaryHistoryRepository = salaryHistoryRepository;
    }

    public async Task<ErrorOr<KnowledgeResponse>> Handle(ImportExcelKnowledgeRequest request, Module module, CancellationToken cancellationToken)
    {
        if (Validate(request, new ImportExcelKnowledgeRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        if (!_moduleService.HasAccessToModule(user, module))
        {
            return UserErrors.ModuleAccessDenied;
        }

        IList<Employee> employees = [];
        IList<SalaryHistory> salaries = [];
        IList<Payroll> payrolls = [];

        using (Stream stream = request.ExcelFile.OpenReadStream())
        using (var workbook = new XLWorkbook(stream))
        {
            IXLWorksheet employeeWorksheet = workbook.Worksheets.Worksheet(1);
            IEnumerable<IXLRow> employeeRows = employeeWorksheet.RowsUsed().Skip(1);

            foreach (IXLRow row in employeeRows)
            {
                string fullName = row.Cell(3).GetString();
                string admissionDate = row.Cell(4).GetString();
                string terminationDate = row.Cell(5).GetString();
                string statusDescription = row.Cell(6).GetString();
                string birthDate = row.Cell(7).GetString();
                string costCenterName = row.Cell(8).GetString();
                string salary = row.Cell(9).GetString();
                string complementarySalary = row.Cell(10).GetString();
                string salaryEffectiveDate = row.Cell(11).GetString();
                string gender = row.Cell(12).GetString();
                string streetAddress = row.Cell(13).GetString();
                string addressNumber = row.Cell(14).GetString();
                string cityName = row.Cell(15).GetString();
                string race = row.Cell(16).GetString();
                string postalCode = row.Cell(17).GetString();

                string companyCodSeniorNumEmp = row.Cell(20).GetString();
                string employeeCodSeniorNumCad = row.Cell(21).GetString();
                string collaboratorTypeCodeSeniorTipCol = row.Cell(22).GetString();
                string statusCodSeniorSitafa = row.Cell(23).GetString();
                string costCenterCodSeniorCodCcu = row.Cell(24).GetString();

                string idEmployee = $"{employeeCodSeniorNumCad}{collaboratorTypeCodeSeniorTipCol}{companyCodSeniorNumEmp}";

                var employee = new Employee()
                {
                    Id = idEmployee,
                    FullName = fullName,
                    AdmissionDate = DateTime.TryParse(admissionDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime admDate) ? admDate : null,
                    TerminationDate = DateTime.TryParse(terminationDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime termDate) ? termDate : null,
                    StatusDescription = statusDescription,
                    BirthDate = DateTime.TryParse(birthDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime bDate) ? bDate : null,
                    CostCneterName = costCenterName,
                    Salary = decimal.TryParse(salary, out decimal sal) ? sal : null,
                    ComplementarySalary = decimal.TryParse(complementarySalary, out decimal compSal) ? compSal : null,
                    SalaryEffectiveDate = DateTime.TryParse(salaryEffectiveDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime salEffDate) ? salEffDate : null,
                    Gender = gender.Equals("M", StringComparison.OrdinalIgnoreCase)
                        ? Gender.Male
                        : Gender.Female,
                    StreetAddress = streetAddress,
                    AddressNumber = addressNumber,
                    CityName = cityName,
                    Race = race,
                    PostalCode = postalCode,
                    StatusCodSenior = statusCodSeniorSitafa,
                    CostCenterCodSeniorCodCcu = costCenterCodSeniorCodCcu,
                };

                employees.Add(employee);
            }

            IXLWorksheet salaryWorksheet = workbook.Worksheets.Worksheet(2);
            IEnumerable<IXLRow> salaryRows = salaryWorksheet.RowsUsed().Skip(1);

            foreach (IXLRow row in salaryRows)
            {
                string employeeId = row.Cell(2).GetString();
                string changeDate = row.Cell(3).GetString();
                string newSalary = row.Cell(4).GetString();
                string motiveName = row.Cell(5).GetString();

                string employeeCodSeniorNumCad = row.Cell(7).GetString();
                string collaboratorTypeCodeSeniorTipCol = row.Cell(8).GetString();
                string companyCodSeniorCodFil = row.Cell(9).GetString();
                string motiveCodSeniorCodMot = row.Cell(10).GetString();

                var salary = new SalaryHistory()
                {
                    IdEmployee = employeeId,
                    ChangeDate = DateTime.TryParse(changeDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime chDate) ? chDate : null,
                    NewSalary = decimal.TryParse(newSalary, out decimal nSalary) ? nSalary : 0,
                    MotiveName = motiveName,
                    EmployeeCodSeniorNumCad = employeeCodSeniorNumCad,
                    CompanyCodSeniorNumEmp = collaboratorTypeCodeSeniorTipCol,
                    CompanyCodSeniorCodFil = companyCodSeniorCodFil,
                    MotiveCodSeniorCodMot = motiveCodSeniorCodMot
                };

                salaries.Add(salary);
            }

            IXLWorksheet payrollWorksheet = workbook.Worksheets.Worksheet(3);
            IEnumerable<IXLRow> payrollRows = payrollWorksheet.RowsUsed().Skip(1);

            foreach (IXLRow row in payrollRows)
            {
                string employeeId = row.Cell(2).GetString();
                string payrollPeriodCod = row.Cell(3).GetString();
                string eventName = row.Cell(4).GetString();
                string eventAmount = row.Cell(5).GetString();
                string eventTypeName = row.Cell(6).GetString();
                string referenceDate = row.Cell(7).GetString();
                string calculationTypeName = row.Cell(8).GetString();
                string payrollPeriodCodSeniorCodcal = row.Cell(13).GetString();
                string eventCodSeniorCodenv = row.Cell(14).GetString();
                string eventTypeCodSeniorTipeve = row.Cell(15).GetString();
                string calculationTypeCodSeniorTipcal = row.Cell(16).GetString();

                string employeeCodSeniorNumCad = row.Cell(10).GetString();
                string collaboratorTypeCodeSeniorTipCol = row.Cell(11).GetString();
                string companyCodSeniorNumemp = row.Cell(12).GetString();

                var payroll = new Payroll()
                {
                    IdEmployee = employeeId,
                    PayrollPeriodCod = payrollPeriodCod,
                    EventName = eventName,
                    EventAmount = decimal.TryParse(eventAmount, out decimal amount) ? amount : null,
                    EventTypeName = eventTypeName,
                    ReferenceDate = DateTime.TryParse(referenceDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime refDate) ? refDate : null,
                    CalculationTypeName = calculationTypeName,
                    EmployeeCodSeniorNumCad = employeeCodSeniorNumCad,
                    CollaboratorTypeCodeSeniorTipCol = collaboratorTypeCodeSeniorTipCol,
                    CompanyCodSeniorNumEmp = companyCodSeniorNumemp,
                    PayrollPeriodCodSeniorCodCal = payrollPeriodCodSeniorCodcal,
                    EventCodSeniorCodenv = eventCodSeniorCodenv,
                    EventTypeCodSeniorTipEve = eventTypeCodSeniorTipeve,
                    CalculationTypeCodSeniorTipCal = calculationTypeCodSeniorTipcal
                };

                payrolls.Add(payroll);
            }
        }

        await _employeeRepository.AddAsync(employees, cancellationToken);
        await _payrollRepository.AddAsync(payrolls, cancellationToken);
        await _salaryHistoryRepository.AddAsync(salaries, cancellationToken);
        await _employeeRepository.UnitOfWork.Commit();
        await _payrollRepository.UnitOfWork.Commit();
        await _salaryHistoryRepository.UnitOfWork.Commit();

        return new();
    }
}