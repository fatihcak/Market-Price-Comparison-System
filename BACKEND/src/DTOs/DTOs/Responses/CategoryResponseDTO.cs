namespace DTOs.DTOs.Responses;

public class CategoryResponseDTO
{
    public int Id { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
