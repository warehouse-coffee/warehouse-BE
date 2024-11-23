

using warehouse_BE.Application.Storages.Commads.UpdateStorage;

namespace warehouse_BE.Application.Storages.Queries.GetStorageDetailUpdate;

public class StorageDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Location { get; set; }
    public string? Status { get; set; }
    public List<AreaDto>? Areas { get; set; }
}
