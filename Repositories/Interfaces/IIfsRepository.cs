using IfsDashboardApi.DTOs;

namespace IfsDashboardApi.Repositories.Interfaces;

public interface IIfsRepository
{
    Task<List<PdksDto>> GetPdksSureleriAsync(DateTime baslangic, DateTime bitis);
    Task<List<IscilikDto>> GetIscilikSureleriAsync(DateTime baslangic, DateTime bitis);
    Task<List<SevkiyatDto>> GetSevkiyatlarAsync(DateTime baslangic, DateTime bitis);
    Task<List<PersonelDto>> GetAktifPersonelAsync();
}
