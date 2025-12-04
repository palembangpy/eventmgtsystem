using EventManagementSystem.Models.Entities;

namespace EventManagementSystem.Services.Interfaces;

public interface ICertificateGeneratorService
{
    Task<string> GenerateCertificateAsync(User user, EventSchedule eventSchedule);
}