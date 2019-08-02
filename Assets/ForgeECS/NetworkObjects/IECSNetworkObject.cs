using Unity.Entities;

public interface IECSNetworkObject {
    Entity AttachedEntity { get; set; }
    ECSNetworkManager AttachedManager { get; set; }
}
