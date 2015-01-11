using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
public class CharacterControllerPlaneNormalTracer : BasicPlaneNormalTracer {
	
	// public members
	public float animationAngle = 270f;
	public float traceAngleLimit = float.PositiveInfinity;
	public Vector3 currentPlaneNormal {
		get { return m_currentPlaneNormal; }
	}
	
	// public constructer
	public CharacterControllerPlaneNormalTracer() {}
	
	// protected methods
	protected override void traceImpl (GameObject groundTraceObject) {
		
		Quaternion q = Quaternion.FromToRotation(Vector3.up, transform.InverseTransformDirection(m_currentPlaneNormal));
		
		if ( float.IsNaN(animationAngle) == false && float.IsInfinity(animationAngle) == false )
			q = Quaternion.RotateTowards( groundTraceObject.transform.localRotation, q, animationAngle * Time.deltaTime );
		
		if ( (float.IsNaN(traceAngleLimit) == false) && (float.IsInfinity(traceAngleLimit) == false) )
			q = Quaternion.RotateTowards( Quaternion.Euler(0f,0f,0f), q, traceAngleLimit );
		
		groundTraceObject.transform.localRotation = q;
	}
	
	// private members
	private Vector3 m_currentPlaneNormal = Vector3.up;
	
	// private methods
	void OnControllerColliderHit (ControllerColliderHit hit) {
		if ( Mathf.Cos( hit.controller.slopeLimit * Mathf.Deg2Rad ) <= hit.normal.y ) m_currentPlaneNormal = hit.normal;
	}
}