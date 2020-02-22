using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class EmptySystem : JobComponentSystem {
    protected override JobHandle OnUpdate(JobHandle dep) { return dep; }

    public new EntityQuery GetEntityQuery(params EntityQueryDesc[] queriesDesc) {
        return base.GetEntityQuery(queriesDesc);
    }

    public new EntityQuery GetEntityQuery(params ComponentType[] componentTypes) {
        return base.GetEntityQuery(componentTypes);
    }

    public new EntityQuery GetEntityQuery(NativeArray<ComponentType> componentTypes) {
        return base.GetEntityQuery(componentTypes);
    }
}

public abstract class ECSTestBase {
    protected World World { get; set; }
    protected EntityManager EntityManager { get; set; }

    [SetUp]
    public virtual void Setup() {
        this.World = new World("Test World");
        this.EntityManager = this.World.EntityManager;
    }

    [TearDown]
    public virtual void TearDown() {
        this.World.Dispose();
        this.World = null;
    }
}
