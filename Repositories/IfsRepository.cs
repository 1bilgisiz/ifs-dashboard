using IfsDashboardApi.DTOs;
using IfsDashboardApi.Repositories.Interfaces;
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

        const string sql = @"
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

    public async Task<List<IscilikDto>> GetIscilikSureleriAsync(DateTime baslangic, DateTime bitis)
    {
        var liste = new List<IscilikDto>();

        using var conn = new OracleConnection(_connectionString);
        await conn.OpenAsync();

        const string sql = @"
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

        cmd.Parameters.Add(new OracleParameter("pBaslangic", OracleDbType.Date)).Value = baslangic;
        cmd.Parameters.Add(new OracleParameter("pBitisPlus1", OracleDbType.Date)).Value = bitis.AddDays(1);

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

    public async Task<List<SevkiyatDto>> GetSevkiyatlarAsync(DateTime baslangic, DateTime bitis)
    {
        var liste = new List<SevkiyatDto>();

        using var conn = new OracleConnection(_connectionString);
        await conn.OpenAsync();

        const string sql = @"
        SELECT 
            sh.CONTRACT AS Site,
            s.SHIPMENT_ID AS Sevkiyat_No,
            s.SOURCE_REF1 AS Siparis_No,
            sh.STATE AS Statu,
            IFSAPP.Shipment_Source_Utility_API.Get_Receiver_Name(sh.RECEIVER_ID, sh.RECEIVER_TYPE_DB) AS Alici,
            sh.CREATED_DATE AS Kayit_Tarihi,
            sh.PLANNED_SHIP_DATE AS Planlanan_Sevk_Tarihi,
            sh.PLANNED_DELIVERY_DATE AS Planli_Teslimat_Tarihi,
            sh.ACTUAL_SHIP_DATE AS Fiili_Sevk_Tarihi
        FROM IFSAPP.SHIPMENT_LINE s
        LEFT JOIN IFSAPP.CUSTOMER_ORDER_LINE co 
               ON s.SOURCE_REF1 = co.ORDER_NO
              AND s.SOURCE_REF2 = co.LINE_NO
              AND s.SOURCE_REF3 = co.REL_NO
              AND co.CUSTOMER_NO NOT LIKE 'SSH-01'
        LEFT JOIN IFSAPP.SHIPMENT sh 
               ON s.SHIPMENT_ID = sh.SHIPMENT_ID
        LEFT JOIN IFSAPP.SHOP_ORD so 
               ON co.ORDER_NO = so.ORDER_NO
              AND co.LINE_NO = so.RELEASE_NO
              AND co.REL_NO = so.SEQUENCE_NO
              AND s.SOURCE_REF1 = so.ORDER_NO
        LEFT JOIN IFSAPP.CUSTOMER_ORDER c
               ON co.ORDER_NO = c.ORDER_NO
        WHERE s.SOURCE_REF1 IS NOT NULL
          AND sh.CREATED_DATE >= :pBaslangic
          AND sh.CREATED_DATE <  :pBitisPlus1
        ORDER BY sh.CREATED_DATE DESC
    ";

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
