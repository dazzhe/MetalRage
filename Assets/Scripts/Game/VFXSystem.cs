using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

[UpdateInGroup(typeof(PresentationSystemGroup))]
[AlwaysSynchronizeSystem]
public class VFXSystem : JobComponentSystem {
    static readonly int positionID = Shader.PropertyToID("position");
    static readonly int targetPositionID = Shader.PropertyToID("targetPosition");
    static readonly int directionID = Shader.PropertyToID("direction");

    class EffectTypeData {
        public VisualEffect visualEffect;
        public float maxDuration = 4.0f;
        public bool active;
        public VFXEventAttribute eventAttribute;
    }

    struct PointEffectRequest {
        public float3 position;
        public float3 direction;
        public VisualEffectAsset asset;
    }

    struct LineEffectRequest {
        public float3 start;
        public float3 end;
        public VisualEffectAsset asset;
    }


    GameObject m_rootGameObject;
    List<PointEffectRequest> m_pointEffectRequests = new List<PointEffectRequest>(32);
    List<LineEffectRequest> m_lineEffectRequests = new List<LineEffectRequest>(32);
    Dictionary<VisualEffectAsset, EffectTypeData> m_EffectTypeData = new Dictionary<VisualEffectAsset, EffectTypeData>(32);
    //    private static List<EffectInstance> s_effectInstances = new List<EffectInstance>(128);


    protected override void OnCreate() {
        base.OnCreate();

        m_rootGameObject = new GameObject("VFXSystem");
        m_rootGameObject.transform.position = Vector3.zero;
        m_rootGameObject.transform.rotation = Quaternion.identity;
        GameObject.DontDestroyOnLoad(m_rootGameObject);
    }

    public void SpawnPointEffect(VisualEffectAsset asset, float3 position, float3 direction) {
        m_pointEffectRequests.Add(new PointEffectRequest {
            asset = asset,
            position = position,
            direction = direction,
        });
    }

    public void SpawnLineEffect(VisualEffectAsset asset, float3 start, float3 end) {
        m_lineEffectRequests.Add(new LineEffectRequest {
            asset = asset,
            start = start,
            end = end,
        });
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        inputDeps.Complete();

        // Handle request
        foreach (var request in m_pointEffectRequests) {
            EffectTypeData effectType;
            if (!m_EffectTypeData.TryGetValue(request.asset, out effectType))
                effectType = RegisterImpactType(request.asset);

            effectType.eventAttribute.SetVector3(positionID, request.position);
            effectType.eventAttribute.SetVector3(directionID, request.direction);
            effectType.visualEffect.Play(effectType.eventAttribute);
            effectType.visualEffect.pause = false;
            //effectType.lastTriggerTick = tick;
            effectType.active = true;
        }
        m_pointEffectRequests.Clear();

        foreach (var request in m_lineEffectRequests) {
            EffectTypeData effectType;
            if (!m_EffectTypeData.TryGetValue(request.asset, out effectType))
                effectType = RegisterImpactType(request.asset);


            effectType.eventAttribute.SetVector3(positionID, request.start);
            effectType.eventAttribute.SetVector3(targetPositionID, request.end);
            effectType.visualEffect.Play(effectType.eventAttribute);
            effectType.visualEffect.pause = false;
            effectType.active = true;
        }
        m_lineEffectRequests.Clear();

        //        foreach (var effectTypeData in m_EffectTypeData.Values)
        //        {
        //            if (effectTypeData.active &&
        //                tick > effectTypeData.lastTriggerTick + effectTypeData.maxDuration)
        //            {
        ////                GameDebug.Log("Reinint effect:" + effectTypeData.visualEffect.name);
        //                effectTypeData.visualEffect.pause = true;
        //                effectTypeData.active = false;
        //                effectTypeData.visualEffect.Stop();
        //                effectTypeData.visualEffect.Reinit();
        //            }
        //        }

        return default;
    }

    EffectTypeData RegisterImpactType(VisualEffectAsset template) {
        GameObject go = new GameObject(template.name);
        go.transform.parent = m_rootGameObject.transform;
        go.transform.position = Vector3.zero;
        go.transform.rotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        var vfx = go.AddComponent<VisualEffect>();
        vfx.visualEffectAsset = template;
        vfx.Reinit();
        vfx.Stop();

        var data = new EffectTypeData {
            visualEffect = vfx,
            eventAttribute = vfx.CreateVFXEventAttribute(),
        };

        m_EffectTypeData.Add(template, data);

        return data;
    }
}
