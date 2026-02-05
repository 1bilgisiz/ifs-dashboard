namespace IfsDashboardApi.DTOs;

public class PersonelDto
{
    public string Sirket { get; set; } = "";
    public string IsyeriKodu { get; set; } = "";
    public string No { get; set; } = "";
    public string Tc { get; set; } = "";
    public string AdSoyad { get; set; } = "";
    public string GrupKodu { get; set; } = "";
    public string GrupKoduIk { get; set; } = "";
    public DateTime? IlkIseGirisTarihi { get; set; }
    public DateTime? IstenCikisTarihi { get; set; }
    public int Yas { get; set; }
    public string Telefon { get; set; } = "";
    public string Adres { get; set; } = "";
    public string KanGrubu { get; set; } = "";
    public string Cinsiyet { get; set; } = "";
    public string Departman { get; set; } = "";
    public string Pozisyon { get; set; } = "";
    public string Unvan { get; set; } = "";
    public string Tahsil { get; set; } = "";
}