namespace OnTimeScheduling.Communication.Requests;

public class RequestPaginationQuery
{
    public const int DefaultPage = 1;
    public const int DefaultSize = 20;
    public const int MaxSize = 100;

    private int _page = DefaultPage;
    private int _size = DefaultSize;

    public int Page
    {
        get => _page;
        set => _page = value < 1 ? DefaultPage : value;
    }

    public int Size
    {
        get => _size;
        set => _size = value switch
        {
            < 1 => DefaultSize,
            > MaxSize => MaxSize,
            _ => value
        };
    }

    public int Skip => (Page - 1) * Size;

}
