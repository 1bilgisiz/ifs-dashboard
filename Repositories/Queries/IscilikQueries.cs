namespace IfsDashboardApi.Repositories.Queries;

public static class IscilikQueries
{
    public const string GetIscilikSureleri = @"
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
}