using System.Text.Json;
using Tf1DayOff.Domain.Entities;

namespace Tf1DayOff.Infra.Repositories;

public class InFileStaticDayOffRequestsRepository : TemplateDayOffRequestsRepository
{
    private readonly string _path;

    public InFileStaticDayOffRequestsRepository(FileStorageSettings setting)
    {
        _path = setting.Path;
        EnsureFileExists(_path);
    }
    
    private static void EnsureFileExists(string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        if (!File.Exists(path)) File.Create(path).Dispose();
    }

    protected override async Task<IDictionary<Guid, DayOffRequest>> Storage()
    {
        var txt = await File.ReadAllTextAsync(_path);
        return (string.IsNullOrEmpty(txt) ? null : JsonSerializer.Deserialize<Dictionary<Guid, DayOffRequest>>(txt)) ??
               new Dictionary<Guid, DayOffRequest>();
    }

    protected override async Task Save(IDictionary<Guid, DayOffRequest> request)
    {
        var txt = JsonSerializer.Serialize(request, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_path, txt);
    }
}

public class FileStorageSettings
{
    public FileStorageSettings(string path)
    {
        Path = path;
    }

    public string Path { get; }
}
