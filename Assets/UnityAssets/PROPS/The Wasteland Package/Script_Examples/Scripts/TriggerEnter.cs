using UnityEngine;
using System.Collections;

public class TriggerEnter : MonoBehaviour {
	
	private Rigidbody _rigidBody;
	
	void Awake () {
		_rigidBody = GetComponent<Rigidbody>();
	}
	void OnCollisionEnter() {
		_rigidBody.isKinematic = false;
	}
}