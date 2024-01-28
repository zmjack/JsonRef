namespace JsonRef.Test;

public class Part(DateOnly date, int count)
{
    public DateOnly SaleDate { get; set; } = date;
    public int Count { get; set; } = count;
    public Head HeadLink { get; set; }
}

