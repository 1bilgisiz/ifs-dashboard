namespace IfsDashboardApi.Repositories.Queries;

public static class IKQueries
{
    public const string AktifPersonelListesi = @"
SELECT a.company_id sirket,
       a.cf$_isyeri_kodu isyeri_kodu,
       a.person_id no,
       ifsapp.trbrd_kimlik_bilgileri_api.get_tc_kimlik_no(a.emp_no) tc,
       internal_display_name ad_soyad,
       a.cf$_grup_adi grup_kodu,
       a.value_description2 grup_kodu_ik,
       a.cf$_kideme_esas_tarihi ilk_ise_giris_tarihi,
       a.cf$_isten_cikis_tarihi isten_cikis_tarihi,
       ifsapp.pers_api.get_age(a.person_id) yas,
       ifsapp.pers_comms_api.get_phone(a.emp_no) telefon,
       c.address adres,
       blood_type kan_grubu,
       ifsapp.pers_api.get_gender(a.person_id) cinsiyet,
       ifsapp.company_org_api.get_org_name(a.company_id, a.org_code) departman,
       ifsapp.company_position_api.get_position_title(a.company_id, a.pos_code) pozisyon,
       ifsapp.company_person_api.get_free_field3(company_id, emp_no) unvan,
       ifsapp.pers_education_profile_api.highest_education_level_name(a.person_id) tahsil
  FROM ifsapp.company_person_all_cfv a
  LEFT JOIN ifsapp.pers_address c
    ON a.person_id = c.person_id
 WHERE a.employee_status = '*'
   AND c.address_id <> 'A02'
   AND a.cf$_isten_cikis_tarihi IS NULL";
}