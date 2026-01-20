using Oracle.ManagedDataAccess.Client;

namespace IfsDashboardApi.Services
{
    public class IfsService
    {
        private readonly string _connectionString;

        public IfsService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("IfsDb")
                ?? throw new InvalidOperationException("IfsDb connection string bulunamadı.");
        }

        // ✅ PDKS
        public async Task<List<PdksDto>> GetPdksSureleriAsync(DateTime baslangic, DateTime bitis)
        {
            var liste = new List<PdksDto>();

            using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();

            string sql = @"
                SELECT p.emp_no,
                       c.employee_name,
                       NVL(c.cf$_isyeri_kodu, 'Bilinmiyor') AS isyeri_kodu,
                       TO_CHAR(p.tarih, 'DD.MM.YYYY') AS gun,
                       ROUND(SUM(p.diff_days * 24) - COUNT(DISTINCT p.tarih) * 1, 2) AS toplam_calisma_saat
                FROM (
                    SELECT emp_no,
                           tarih,
                           islem_adi,
                           CASE
                               WHEN islem_adi = 'Cikis' THEN
                                   (saat - LAG(saat) OVER(PARTITION BY emp_no, tarih ORDER BY saat))
                           END AS diff_days
                    FROM ifsapp.trbrd_pdks_hareket
                    WHERE tarih BETWEEN :pBaslangic AND :pBitis
                ) p
                JOIN ifsapp.company_person_all_cfv c
                  ON p.emp_no = c.emp_no
                 AND c.free_field2 IN ('MY01')
                WHERE p.diff_days IS NOT NULL
                GROUP BY p.emp_no, c.employee_name, NVL(c.cf$_isyeri_kodu, 'Bilinmiyor'), TO_CHAR(p.tarih,'DD.MM.YYYY')
            ";

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

        // ✅ İŞÇİLİK
        public async Task<List<IscilikDto>> GetIscilikSureleriAsync(DateTime? baslangic, DateTime? bitis)
        {
            var liste = new List<IscilikDto>();

            using var conn = new OracleConnection(_connectionString);
            await conn.OpenAsync();

            // tarih gönderilmezse default: son 150 gün
            var b = baslangic ?? DateTime.Today.AddDays(-150);
            var t = bitis ?? DateTime.Today;

            string sql = @"
                SELECT aa.order_no,
                       CASE
                           WHEN SUBSTR(aa.work_center_no, CEIL(LENGTH(aa.work_center_no) / 2), 1) = '1' THEN 'MEKA 1'
                           WHEN SUBSTR(aa.work_center_no, CEIL(LENGTH(aa.work_center_no) / 2), 1) = '2' THEN 'MEKA 2'
                           WHEN SUBSTR(aa.work_center_no, CEIL(LENGTH(aa.work_center_no) / 2), 1) = '3' THEN 'MEKA 3'
                           WHEN SUBSTR(aa.work_center_no, CEIL(LENGTH(aa.work_center_no) / 2), 1) = '4' THEN 'MEKA 4'
                           WHEN SUBSTR(aa.work_center_no, CEIL(LENGTH(aa.work_center_no) / 2), 1) = 'E' THEN 'MEKA ESK'
                           ELSE 'Bilinmiyor'
                       END AS work_center_adi,
                       SUM(aa.labour_time) AS toplam_emek_suresi
                  FROM ifsapp.trcost_dist_base_qry aa
                 WHERE aa.date_applied >= :pBaslangic
                   AND aa.date_applied <  :pBitisPlus1
                 GROUP BY aa.order_no,
                          CASE
                              WHEN SUBSTR(aa.work_center_no, CEIL(LENGTH(aa.work_center_no) / 2), 1) = '1' THEN 'MEKA 1'
                              WHEN SUBSTR(aa.work_center_no, CEIL(LENGTH(aa.work_center_no) / 2), 1) = '2' THEN 'MEKA 2'
                              WHEN SUBSTR(aa.work_center_no, CEIL(LENGTH(aa.work_center_no) / 2), 1) = '3' THEN 'MEKA 3'
                              WHEN SUBSTR(aa.work_center_no, CEIL(LENGTH(aa.work_center_no) / 2), 1) = '4' THEN 'MEKA 4'
                              WHEN SUBSTR(aa.work_center_no, CEIL(LENGTH(aa.work_center_no) / 2), 1) = 'E' THEN 'MEKA ESK'
                              ELSE 'Bilinmiyor'
                          END
            ";

            using var cmd = new OracleCommand(sql, conn);
            cmd.BindByName = true;

            cmd.Parameters.Add(new OracleParameter("pBaslangic", OracleDbType.Date)).Value = b;
            cmd.Parameters.Add(new OracleParameter("pBitisPlus1", OracleDbType.Date)).Value = t.AddDays(1);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                liste.Add(new IscilikDto
                {
                    OrderNo = reader.IsDBNull(0) ? "" : reader.GetString(0),
                    WorkCenterAdi = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    ToplamEmekSuresi = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2)
                });
            }

            return liste;
        }
    }

    // ✅ DTO’lar (aynı dosyada dursun sorun yok)
    public class PdksDto
    {
        public string EmpNo { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string IsyeriKodu { get; set; } = "";
        public string Gun { get; set; } = "";
        public decimal ToplamCalismaSaat { get; set; }
    }

    public class IscilikDto
    {
        public string OrderNo { get; set; } = "";
        public string WorkCenterAdi { get; set; } = "";
        public decimal ToplamEmekSuresi { get; set; }
    }
}
