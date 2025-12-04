using EventManagementSystem.Models.Entities;
using EventManagementSystem.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EventManagementSystem.Services;

public class CertificateGeneratorService : ICertificateGeneratorService
{
    public async Task<string> GenerateCertificateAsync(User user, EventSchedule eventSchedule)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var certNumber = $"CERT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        var fileName = $"{certNumber}.pdf";
        
        var certDir = Path.Combine("wwwroot", "certificates");
        Directory.CreateDirectory(certDir);
        var filePath = Path.Combine(certDir, fileName);

        await Task.Run(() =>
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);

                    page.Header().Height(120).Background(Colors.Blue.Darken2)
                        .Padding(20).AlignCenter().AlignMiddle()
                        .Column(col =>
                        {
                            col.Item().Text("CERTIFICATE OF PARTICIPATION")
                                .FontSize(36).Bold().FontColor(Colors.White);
                            col.Item().PaddingTop(5).Text("Event Management System")
                                .FontSize(14).FontColor(Colors.Blue.Lighten3);
                        });

                    page.Content().Padding(40).Column(column =>
                    {
                        column.Spacing(25);

                        column.Item().PaddingTop(30).AlignCenter()
                            .Text("This is to certify that")
                            .FontSize(20).FontColor(Colors.Grey.Darken2);

                        column.Item().AlignCenter().Text(user.Name)
                            .FontSize(42).Bold().FontColor(Colors.Blue.Darken3);

                        column.Item().PaddingTop(10).AlignCenter()
                            .Text("has successfully participated in")
                            .FontSize(20).FontColor(Colors.Grey.Darken2);

                        column.Item().AlignCenter().Text(eventSchedule.Title)
                            .FontSize(32).Bold().FontColor(Colors.Blue.Darken2);

                        column.Item().PaddingTop(20).AlignCenter()
                            .Text($"Held on {eventSchedule.StartDate:dddd, MMMM dd, yyyy}")
                            .FontSize(18).FontColor(Colors.Grey.Darken1);

                        column.Item().AlignCenter()
                            .Text($"at {eventSchedule.Location}")
                            .FontSize(18).FontColor(Colors.Grey.Darken1);

                        if (eventSchedule.Speaker != null)
                        {
                            column.Item().PaddingTop(40).AlignCenter()
                                .Text($"Presented by: {eventSchedule.Speaker.Name}")
                                .FontSize(16).Italic().FontColor(Colors.Grey.Medium);
                        }

                        column.Item().PaddingTop(50).Row(row =>
                        {
                            row.RelativeItem().AlignLeft()
                                .Text($"Certificate No: {certNumber}")
                                .FontSize(11).FontColor(Colors.Grey.Medium);
                            
                            row.RelativeItem().AlignRight()
                                .Text($"Generated: {DateTime.UtcNow:MMM dd, yyyy}")
                                .FontSize(11).FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Footer().Height(60).Background(Colors.Blue.Lighten4)
                        .Padding(15).AlignCenter().AlignMiddle()
                        .Text("This is an official certificate issued by Event Management System")
                        .FontSize(11).FontColor(Colors.Grey.Darken1);
                });
            }).GeneratePdf(filePath);
        });

        return $"certificates/{fileName}";
    }
}
