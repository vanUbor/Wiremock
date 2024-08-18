using WireMock.Admin.Mappings;

namespace WireMock.Server;

public class ChangedMappingsEventArgs(IList<MappingModel> mappingModels) : EventArgs
{
    public required string ServiceId { get; init; }
    public IList<MappingModel> MappingModels { get; } = mappingModels;
}