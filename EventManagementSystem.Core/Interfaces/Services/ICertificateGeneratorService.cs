using EventManagementSystem.Core.Models.Entities;

namespace EventManagementSystem.Core.Interfaces.Services;

public interface ICertificateGeneratorService
{
    Task<string> GenerateCertificateAsync(User user, EventSchedule eventSchedule);
}