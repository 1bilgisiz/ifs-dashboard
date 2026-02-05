using IfsDashboardApi.DTOs;
using IfsDashboardApi.Repositories.Interfaces;

namespace IfsDashboardApi.Services;

public class IfsService(IIfsRepository repository) : IIfsService
{
    private readonly IIfsRepository _repository = repository;

    public Task<List<PdksDto>> GetPdksSureleriAsync(DateTime baslangic, DateTime bitis)
    {
        return _repository.GetPdksSureleriAsync(baslangic, bitis);
    }

    public Task<List<IscilikDto>> GetIscilikSureleriAsync(DateTime? baslangic, DateTime? bitis)
    {
        var b = baslangic ?? DateTime.Today.AddDays(-150);
        var t = bitis ?? DateTime.Today;
        return _repository.GetIscilikSureleriAsync(b, t);
    }

    public Task<List<SevkiyatDto>> GetSevkiyatlarAsync(DateTime? baslangic, DateTime? bitis)
    {
        var b = baslangic ?? DateTime.Today.AddDays(-150);
        var t = bitis ?? DateTime.Today;
        return _repository.GetSevkiyatlarAsync(b, t);
    }
    public async Task<List<PersonelDto>> GetAktifPersonelAsync()
    {
        return await _repository.GetAktifPersonelAsync();
    }

}