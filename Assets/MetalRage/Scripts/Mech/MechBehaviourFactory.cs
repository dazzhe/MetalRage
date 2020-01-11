using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public abstract class MechActionFactory : ScriptableObject {
    public abstract Entity Create(EntityManager entityManager);

    //public Entity CreateCharBehavior(EntityManager entityManager) {
    //    var entity = entityManager.CreateEntity();

    //    entityManager.AddComponentData(entity, new CharBehaviour());
    //    entityManager.AddComponentData(entity, new AbilityControl());

    //    return entity;
    //}
}
