namespace OnTimeScheduling.Communication.Requests;

public class RequestUpdateCompanyJson
{
    public string SocialReason { get; set; } = string.Empty;
    public string FantasyName { get; set; } = string.Empty;
    public string CNPJ { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string CompanyEmail { get; set; } = string.Empty;
}
