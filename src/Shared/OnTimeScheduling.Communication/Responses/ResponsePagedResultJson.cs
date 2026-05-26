namespace OnTimeScheduling.Communication.Responses;

public class ResponsePagedResultJson<T>
{
    public required int Page { get; init; }
    public required int Size { get; init; }
    public required int TotalItems { get; init; }
    public required int TotalPages { get; init; }
    public required IReadOnlyList<T> Items { get; init; }

}
