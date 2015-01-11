using UnityEngine;
using System.Collections;

public abstract class BasicPlaneNormalTracer : MonoBehaviour {
	
	// public members
	public bool autoTraceOnUpdate = true;	// call TraceGround() On Update switch
	public string traceObjectName;
	
	// public methods
	public void Trace() { Trace(null); }
	public void Trace( GameObject groundTraceObject ) {
		if ( groundTraceObject == null ) {
			Transform trans = gameObject.transform.FindChild(traceObjectName);
			if ( trans != null ) groundTraceObject = trans.gameObject;
		}
		if ( groundTraceObject == null ) return;
		traceImpl(groundTraceObject);
	}
	
	// protected constructer
	protected BasicPlaneNormalTracer(){}
	protected BasicPlaneNormalTracer( string initialTraceObjectName ){
		traceObjectName = initialTraceObjectName;
	}
	
	// protected methods
	protected abstract void traceImpl( GameObject groundTraceObject );
	
	// private methods
	void Update () {
		if ( autoTraceOnUpdate ) Trace( null );
	}
}