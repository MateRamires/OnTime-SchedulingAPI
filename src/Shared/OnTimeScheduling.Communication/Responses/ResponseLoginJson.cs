namespace OnTimeScheduling.Communication.Responses;

public class ResponseLoginJson
{
    public string TokenType { get; set; } = "Bearer";
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public ResponseUserProfileJson User { get; set; } = new();
}
