using IfsDashboardApi.DTOs;
using IfsDashboardApi.Repositories.Interfaces;
using IfsDashboardApi.Repositories.Queries;
using Oracle.ManagedDataAccess.Client;

namespace IfsDashboardApi.Repositories;

public class IfsRepository(IConfiguration configuration) : IIfsRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("IfsDb")
        ?? throw new InvalidOperationException("IfsDb connection string bulunamadı.");

    public async Task<List<PdksDto>> GetPdksSureleriAsync(DateTime baslangic, DateTime bitis)
    {
        var liste = new List<PdksDto>();

        using var conn = new OracleConnection(_connectionString);
        await conn.OpenAsync();

            const string sql = PdksQueries.GetPdksSureleri;

        using var cmd = new OracleCommand(sql, conn);
        cmd.BindByName = true;
        cmd.Parameters.Add(new OracleParameter("pBaslangic", OracleDbType.Date)).Value = baslangic;
        cmd.Parameters.Add(new OracleParameter("pBitis", OracleDbType.Date)).Value = bitis;

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            liste.Add(new PdksDto
            {
                EmpNo = reader.IsDBNull(0) ? "" : reader.GetString(0),
                EmployeeName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                IsyeriKodu = reader.IsDBNull(2) ? "" : reader.GetString(2),
                Gun = reader.IsDBNull(3) ? "" : reader.GetString(3),
                ToplamCalismaSaat = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4)
            });
        }

        return liste;
    }

    public async Task<List<IscilikDto>> GetIscilikSureleriAsync(DateTime baslangic, DateTime bitis)
    {
        var liste = new List<IscilikDto>();

        using var conn = new OracleConnection(_connectionString);
        await conn.OpenAsync();

        const string sql = IscilikQueries.GetIscilikSureleri;

        using var cmd = new OracleCommand(sql, conn);
        cmd.BindByName = true;

        cmd.Parameters.Add(new OracleParameter("pBaslangic", OracleDbType.Date)).Value = baslangic;
        cmd.Parameters.Add(new OracleParameter("pBitisPlus1", OracleDbType.Date)).Value = bitis.AddDays(1);

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            liste.Add(new IscilikDto
            {
                Tarih = reader.IsDBNull(0) ? "" : reader.GetString(0),
                WorkCenterAdi = reader.IsDBNull(1) ? "" : reader.GetString(1),
                ToplamEmekSuresi = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2)
            });
        }

        return liste;
    }

    public async Task<List<SevkiyatDto>> GetSevkiyatlarAsync(DateTime baslangic, DateTime bitis)
    {
        var liste = new List<SevkiyatDto>();

        using var conn = new OracleConnection(_connectionString);
        await conn.OpenAsync();

        const string sql = SevkiyatQueries.GetSevkiyatlar;

        using var cmd = new OracleCommand(sql, conn);
        cmd.BindByName = true;

        cmd.Parameters.Add(new OracleParameter("pBaslangic", OracleDbType.Date)).Value = baslangic;
        cmd.Parameters.Add(new OracleParameter("pBitisPlus1", OracleDbType.Date)).Value = bitis.AddDays(1);

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            liste.Add(new SevkiyatDto
            {
                Site = reader.IsDBNull(0) ? "" : reader.GetString(0),
                SevkiyatNo = reader.IsDBNull(1) ? "" : reader.GetString(1),
                SiparisNo = reader.IsDBNull(2) ? "" : reader.GetString(2),
                Statu = reader.IsDBNull(3) ? "" : reader.GetString(3),
                Alici = reader.IsDBNull(4) ? "" : reader.GetString(4),
                KayitTarihi = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                PlanlananSevkTarihi = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                PlanliTeslimatTarihi = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7),
                FiiliSevkTarihi = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8),
            });
        }

        return liste;
    }
}
