namespace IfsDashboardApi.Repositories.Queries;

public static class PdksQueries
{
    public const string GetPdksSureleri = @"
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

   
    
   
}
