namespace OnTimeScheduling.Communication.Responses;

public class ResponseCompanyJson
{
    public Guid Id { get; set; }
    public string SocialReason { get; set; } = string.Empty;
    public string FantasyName { get; set; } = string.Empty;
    public string CNPJ { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string CompanyEmail { get; set; } = string.Empty;
    public Enums.RecordStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
