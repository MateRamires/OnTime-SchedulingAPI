namespace OnTimeScheduling.Communication.Requests;

public class RequestRegisterCompanyJson
{
    // Admin Data
    public string Name { get; set; } = string.Empty; 
    public string Email { get; set; } = string.Empty; 
    public string Password { get; set; } = string.Empty; 

    // Company Data
    public string SocialReason { get; set; } = string.Empty;
    public string FantasyName { get; set; } = string.Empty;
    public string CNPJ { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
