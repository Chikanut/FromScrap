using Unity.Entities;
using Zenject;

public partial class GameManagerSystem : SystemBase
{
    
    protected override void OnCreate()
    {
        base.OnCreate();
        
        ProjectContext.Instance.Container.Inject(this);
    }
    
    protected override void OnUpdate()
    {
        
    }
}
