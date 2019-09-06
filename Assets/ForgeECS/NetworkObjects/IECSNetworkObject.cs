using Unity.Entities;

public interface IECSNetworkObject {
    //Fields
    Entity AttachedEntity { get; set; }
    ECSNetworkManager AttachedManager { get; set; }


    //Functions
    void RegisterComponents ();
}
