//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using Unity.Entities;
//using UnityEngine;
//using UnityEngine.Profiling;

//[DisableAutoCreation]
//public abstract class BaseComponentSystem : ComponentSystem {

//}

//[DisableAutoCreation]
//public abstract class BaseComponentSystem<T1> : BaseComponentSystem
// 	where T1 : MonoBehaviour, IComponentData {
//	ComponentGroup group;

//	protected override void OnCreateManager() {
//		base.OnCreateManager();
//		var list = new List<ComponentType>(2);
//		list.AddRange(new ComponentType[] { typeof(T1) });
//		list.Add(ComponentType.Subtractive<DespawningFlag>());
//		this.group = GetComponentGroup(list.ToArray());
//	}

//	protected override void OnUpdate() {
//		var entityArray = this.group.GetEntityArray();
//		var dataArray = this.group.GetComponentArray<T1>();
//		for (var i = 0; i < entityArray.Length; i++) {
//			OnUpdate(entityArray[i], dataArray[i]);
//		}
//	}

//	protected abstract void OnUpdate(Entity entity, T1 data);
//}


//[DisableAutoCreation]
//public abstract class BaseComponentSystem<T1, T2> : BaseComponentSystem
//	where T1 : MonoBehaviour
//	where T2 : MonoBehaviour {
//	ComponentGroup group;

//	protected override void OnCreateManager() {
//		base.OnCreateManager();
//		var list = new List<ComponentType>(3);
//		list.AddRange(new ComponentType[] { typeof(T1), typeof(T2) });
//		list.Add(ComponentType.Subtractive<DespawningFlag>());
//		Group = GetComponentGroup(list.ToArray());
//	}

//	protected override void OnUpdate() {
//		Profiler.BeginSample(name);

//		var entityArray = Group.GetEntityArray();
//		var dataArray1 = Group.GetComponentArray<T1>();
//		var dataArray2 = Group.GetComponentArray<T2>();

//		for (var i = 0; i < entityArray.Length; i++) {
//			Update(entityArray[i], dataArray1[i], dataArray2[i]);
//		}

//		Profiler.EndSample();
//	}

//	protected abstract void Update(Entity entity, T1 data1, T2 data2);
//}


//[DisableAutoCreation]
//public abstract class BaseComponentSystem<T1, T2, T3> : BaseComponentSystem
//	where T1 : MonoBehaviour
//	where T2 : MonoBehaviour
//	where T3 : MonoBehaviour {
//	ComponentGroup Group;
//	protected ComponentType[] ExtraComponentRequirements;
//	string name;

//	public BaseComponentSystem(GameWorld world) : base(world) { }

//	protected override void OnCreateManager() {
//		base.OnCreateManager();
//		name = GetType().Name;
//		var list = new List<ComponentType>(6);
//		if (ExtraComponentRequirements != null)
//			list.AddRange(ExtraComponentRequirements);
//		list.AddRange(new ComponentType[] { typeof(T1), typeof(T2), typeof(T3) });
//		list.Add(ComponentType.Subtractive<DespawningEntity>());
//		Group = GetComponentGroup(list.ToArray());
//	}

//	protected override void OnUpdate() {
//		Profiler.BeginSample(name);

//		var entityArray = Group.GetEntityArray();
//		var dataArray1 = Group.GetComponentArray<T1>();
//		var dataArray2 = Group.GetComponentArray<T2>();
//		var dataArray3 = Group.GetComponentArray<T3>();

//		for (var i = 0; i < entityArray.Length; i++) {
//			Update(entityArray[i], dataArray1[i], dataArray2[i], dataArray3[i]);
//		}

//		Profiler.EndSample();
//	}

//	protected abstract void Update(Entity entity, T1 data1, T2 data2, T3 data3);
//}

//[DisableAutoCreation]
//public abstract class BaseComponentDataSystem<T1> : BaseComponentSystem
//	where T1 : struct, IComponentData {
//	ComponentGroup Group;
//	protected ComponentType[] ExtraComponentRequirements;
//	string name;

//	protected override void OnCreateManager() {
//		base.OnCreateManager();
//		name = GetType().Name;
//		var list = new List<ComponentType>(6);
//		if (ExtraComponentRequirements != null)
//			list.AddRange(ExtraComponentRequirements);
//		list.AddRange(new ComponentType[] { typeof(T1) });
//		list.Add(ComponentType.Subtractive<DespawningEntity>());
//		Group = GetComponentGroup(list.ToArray());
//	}

//	protected override void OnUpdate() {
//		Profiler.BeginSample(name);

//		var entityArray = Group.GetEntityArray();
//		var dataArray = Group.GetComponentDataArray<T1>();

//		for (var i = 0; i < entityArray.Length; i++) {
//			Update(entityArray[i], dataArray[i]);
//		}

//		Profiler.EndSample();
//	}

//	protected abstract void Update(Entity entity, T1 data);
//}

//[DisableAutoCreation]
//public abstract class BaseComponentDataSystem<T1, T2> : BaseComponentSystem
//	where T1 : struct, IComponentData
//	where T2 : struct, IComponentData {
//	ComponentGroup Group;
//	protected ComponentType[] ExtraComponentRequirements;
//	private string name;

//	public BaseComponentDataSystem(GameWorld world) : base(world) { }

//	protected override void OnCreateManager() {
//		name = GetType().Name;
//		base.OnCreateManager();
//		var list = new List<ComponentType>(6);
//		if (ExtraComponentRequirements != null)
//			list.AddRange(ExtraComponentRequirements);
//		list.AddRange(new ComponentType[] { typeof(T1), typeof(T2) });
//		list.Add(ComponentType.Subtractive<DespawningEntity>());
//		Group = GetComponentGroup(list.ToArray());
//	}

//	protected override void OnUpdate() {
//		Profiler.BeginSample(name);

//		var entityArray = Group.GetEntityArray();
//		var dataArray1 = Group.GetComponentDataArray<T1>();
//		var dataArray2 = Group.GetComponentDataArray<T2>();

//		for (var i = 0; i < entityArray.Length; i++) {
//			Update(entityArray[i], dataArray1[i], dataArray2[i]);
//		}

//		Profiler.EndSample();
//	}

//	protected abstract void Update(Entity entity, T1 data1, T2 data2);
//}

//[DisableAutoCreation]
//public abstract class BaseComponentDataSystem<T1, T2, T3> : BaseComponentSystem
//	where T1 : struct, IComponentData
//	where T2 : struct, IComponentData
//	where T3 : struct, IComponentData {
//	ComponentGroup Group;
//	protected ComponentType[] ExtraComponentRequirements;
//	string name;

//	public BaseComponentDataSystem(GameWorld world) : base(world) { }

//	protected override void OnCreateManager() {
//		base.OnCreateManager();
//		name = GetType().Name;
//		var list = new List<ComponentType>(6);
//		if (ExtraComponentRequirements != null)
//			list.AddRange(ExtraComponentRequirements);
//		list.AddRange(new ComponentType[] { typeof(T1), typeof(T2), typeof(T3) });
//		list.Add(ComponentType.Subtractive<DespawningEntity>());
//		Group = GetComponentGroup(list.ToArray());
//	}

//	protected override void OnUpdate() {
//		Profiler.BeginSample(name);

//		var entityArray = Group.GetEntityArray();
//		var dataArray1 = Group.GetComponentDataArray<T1>();
//		var dataArray2 = Group.GetComponentDataArray<T2>();
//		var dataArray3 = Group.GetComponentDataArray<T3>();

//		for (var i = 0; i < entityArray.Length; i++) {
//			Update(entityArray[i], dataArray1[i], dataArray2[i], dataArray3[i]);
//		}

//		Profiler.EndSample();
//	}

//	protected abstract void Update(Entity entity, T1 data1, T2 data2, T3 data3);
//}
