namespace IfsDashboardApi.DTOs;

public class SevkiyatDto
{
    public string Site { get; set; } = "";
    public string SevkiyatNo { get; set; } = "";
    public string SiparisNo { get; set; } = "";
    public string Statu { get; set; } = "";
    public string Alici { get; set; } = "";
    public DateTime? KayitTarihi { get; set; }
    public DateTime? PlanlananSevkTarihi { get; set; }
    public DateTime? PlanliTeslimatTarihi { get; set; }
    public DateTime? FiiliSevkTarihi { get; set; }
}
