namespace FilterCore.Interfaces
{
    public interface IFilter
    {
        int CurrentPage { get; set; }

        int PageSize { get; set; }
    }
}