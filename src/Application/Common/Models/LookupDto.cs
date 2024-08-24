using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Common.Models;

public class LookupDto
{
    public int Id { get; init; }

    public string? Title { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            //CreateMap<TodoList, LookupDto>();
            //CreateMap<TodoItem, LookupDto>();
        }
    }
}
