namespace IfsDashboardApi.Repositories.Queries;

public static class SevkiyatQueries
{
    public const string GetSevkiyatlar = @"
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
}