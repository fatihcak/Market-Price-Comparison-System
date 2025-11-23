namespace DTOs.DTOs.Responses;

public class MarketResponseDTO
{
    public int Id { get; set; }
    public string MarketName { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Website { get; set; }
    public DateTime CreatedAt { get; set; }
}
