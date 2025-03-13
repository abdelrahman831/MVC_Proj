using AutoMapper;
using Castle.Core.Logging;
using Demo.BLL.DTOS.Employees;
using Demo.DAL.Entities.Employees;
using Demo.DAL.Presistance.Repositories.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Demo.DAL.Presistance.UnitOfWork;
using Demo.BLL.Services.Attacments;
using Microsoft.AspNetCore.Hosting;

// Aggiungi questa using


namespace Demo.BLL.Services.Employees
{
    public class EmployeeService : IEmployeeService
    {

        public IEmployeeRepository _employeeRepository;
        private readonly IAttacchmentService _attachmentService;
        public IUnitOfWork _unitOfWork;
        public IMapper _mapper;
        public ILogger<EmployeeService> _logger;

        public IWebHostEnvironment _env;
        public EmployeeService(ILogger<EmployeeService> logger,IMapper mapper,IUnitOfWork unitOfWork,IAttacchmentService attacchmentService, IWebHostEnvironment env) //Ask Clr to Create instance
        {
    
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _attachmentService = attacchmentService;
            _env = env;
        }

        #region Create
        public int CreateEmployee(EmployeeToCreateDto employeeCreateDto)
        {
            try
            {
                _logger.LogInformation("Ricevuto DTO: {@Dto}", employeeCreateDto);

                // Se c'è un file, lo salviamo prima
                if (employeeCreateDto.Image != null)
                {
                    _logger.LogInformation("Trovato file: {FileName}, Dimensione: {Size} bytes",
                        employeeCreateDto.Image.FileName, employeeCreateDto.Image.Length);

                    var fileName = _attachmentService.Upload(employeeCreateDto.Image, "Images");

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        _logger.LogInformation("File salvato con nome: {FileName}", fileName);
                        employeeCreateDto.ImagePath = fileName; // Assegniamo il nome del file
                    }
                    else
                    {
                        _logger.LogError("Errore durante il salvataggio dell'immagine.");
                        return 0; // Fallisce il salvataggio
                    }
                }
                else
                {
                    _logger.LogWarning("Nessun file ricevuto per l'upload.");
                }

                // Ora possiamo mappare l'oggetto
                var employee = _mapper.Map<Employee>(employeeCreateDto);

                _unitOfWork.EmployeeRepository.AddT(employee);
                int result = _unitOfWork.Complete();

                _logger.LogInformation("Dipendente salvato: {Employee}", JsonConvert.SerializeObject(employee));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione del dipendente.");
                throw;
            }
        }


        #endregion

        #region Delete
        public bool DeleteEmployee(int id)
        {
            var employee = _unitOfWork.EmployeeRepository.GetById(id);
            if (employee is not null)
                _unitOfWork.EmployeeRepository.DeleteT(employee);
            return _unitOfWork.Complete() >0;

        }
        #endregion

        #region Index
        public IEnumerable<EmployeeToReturnDto> GetAllEmployees()
        {
            return _unitOfWork.EmployeeRepository.GetAllQuarable().Include(E => E.Department)
                 .Where(E => !E.IsDeleted)
                 .Select(employee => _mapper.Map<EmployeeToReturnDto>(employee));

        } 
        #endregion

        #region Details
        public EmployeeDetailsDto? GetEmployeesById(int id)
        {
            var employee = _unitOfWork.EmployeeRepository.GetById(id);  
            if (employee is not null)
            {
                return _mapper.Map<EmployeeDetailsDto>(employee);
      

            }
            return null!;
        }
        #endregion

        #region Update
        public int UpdateEmployee(EmployeeToUpdateDto employeeUpdateDto)
        {
            try
            {
                // Recupera il dipendente esistente dal repository
                var employee = _unitOfWork.EmployeeRepository.GetById(employeeUpdateDto.Id);
                if (employee == null)
                {
                    _logger.LogWarning("Dipendente con ID {Id} non trovato.", employeeUpdateDto.Id);
                    return 0; // Nessun aggiornamento effettuato
                }

                // Se è stata caricata una nuova immagine, aggiorna l'immagine
                if (employeeUpdateDto.Image != null)
                {
                    _logger.LogInformation("Nuova immagine caricata: {FileName}", employeeUpdateDto.Image.FileName);

                    // Elimina l'immagine precedente, se esiste
                    if (!string.IsNullOrEmpty(employee.ImagePath))
                    {
                        var oldFilePath = Path.Combine(_env.WebRootPath, "Files", "Images", employee.ImagePath);
                        if (File.Exists(oldFilePath))
                        {
                            File.Delete(oldFilePath);
                            _logger.LogInformation("Immagine precedente eliminata: {OldFilePath}", oldFilePath);
                        }
                    }

                    // Salva la nuova immagine e aggiorna il campo ImagePath
                    var newFileName = _attachmentService.Upload(employeeUpdateDto.Image, "Images");
                    if (!string.IsNullOrEmpty(newFileName))
                    {
                        employee.ImagePath = newFileName; // Aggiorna il nome dell'immagine nel database
                        _logger.LogInformation("Immagine aggiornata con successo: {NewFileName}", newFileName);
                    }
                }

                // Mappa gli altri campi del DTO nell'entità esistente (ma non l'immagine, che è già stata aggiornata)
                _mapper.Map(employeeUpdateDto, employee); // Ignora tutti i membri non esplicitamente mappati

                // Aggiorna il dipendente nel repository
                _unitOfWork.EmployeeRepository.UpdateT(employee);
                return _unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento del dipendente.");
                throw;
            }
        }

        #endregion
    }
}
