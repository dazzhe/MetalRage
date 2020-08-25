using Unity.Collections;
using Unity.Physics;
using UnityEngine.Assertions;

public struct SelfFilteringAllHitsCollector<T> : ICollector<T> where T : struct, IQueryResult {
    private int selfRigidBodyIndex;

    public bool EarlyOutOnFirstHit => false;
    public float MaxFraction { get; }
    public int NumHits => this.AllHits.Length;

    public NativeList<T> AllHits;

    public SelfFilteringAllHitsCollector(int selfRigidBodyIndex, float maxFraction, ref NativeList<T> allHits) {
        this.MaxFraction = maxFraction;
        this.AllHits = allHits;
        this.selfRigidBodyIndex = selfRigidBodyIndex;
    }

    #region IQueryResult implementation

    public bool AddHit(T hit) {
        Assert.IsTrue(hit.Fraction < this.MaxFraction);
        if (hit.RigidBodyIndex == this.selfRigidBodyIndex) {
            return false;
        }
        this.AllHits.Add(hit);
        return true;
    }

    #endregion
}

// A collector which stores only the closest hit different from itself.
public struct SelfFilteringClosestHitCollector<T> : ICollector<T> where T : struct, IQueryResult {
    public bool EarlyOutOnFirstHit => false;
    public float MaxFraction { get; private set; }
    public int NumHits { get; private set; }
    public T ClosestHit { get; private set; }

    private readonly int selfRigidBodyIndex;

    public SelfFilteringClosestHitCollector(int selfRigidBodyIndex, float maxFraction) {
        this.MaxFraction = maxFraction;
        this.ClosestHit = default;
        this.NumHits = 0;
        this.selfRigidBodyIndex = selfRigidBodyIndex;
    }

    #region ICollector

    public bool AddHit(T hit) {
        Assert.IsTrue(hit.Fraction <= this.MaxFraction);
        if (hit.RigidBodyIndex == this.selfRigidBodyIndex) {
            return false;
        }
        this.MaxFraction = hit.Fraction;
        this.ClosestHit = hit;
        this.NumHits = 1;
        return true;
    }
    #endregion
}
