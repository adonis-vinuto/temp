using Domain.Enums;

namespace Domain.Entities;

public sealed class Agent : BaseEntity
{
    public Guid IdUser { get; set; }
    public string Organization { get; set; }
    public Module Module { get; set; }
    public TypeAgent Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public IEnumerable<ChatSession> Chats => _chats;
    private readonly List<ChatSession> _chats = [];
    public IEnumerable<Knowledge> Knowledges => _knowledges;
    private readonly List<Knowledge> _knowledges = [];
    public IEnumerable<File> Files => _files;
    private readonly List<File> _files = [];
    public IEnumerable<SeniorErpConfig> SeniorErpConfigs => _seniorErpConfigs;
    private readonly List<SeniorErpConfig> _seniorErpConfigs = [];
    public IEnumerable<SeniorHcmConfig> SeniorHcmConfigs => _seniorHcmConfigs;
    private readonly List<SeniorHcmConfig> _seniorHcmConfigs = [];
}