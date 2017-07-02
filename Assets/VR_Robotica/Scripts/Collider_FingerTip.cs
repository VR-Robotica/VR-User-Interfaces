using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.VR_Robotica.VRUI.LeapMotion
{
	public class Collider_FingerTip : MonoBehaviour
	{
		public enum FingerName { Undefined, Index, Middle, Ring, Pinky, Thumb};
		public FingerName NameOfFinger = FingerName.Undefined;
		[Space]
		[Range(1.0f, 20.0f)]
		public float ColliderRadius = 5.0f;
		private void Awake()
		{
			addRigidBody();
			addCollider();
		}

		private void addCollider()
		{
			SphereCollider collider = this.gameObject.GetComponent<SphereCollider>();
			if(collider == null)
			{
				collider = this.gameObject.AddComponent<SphereCollider>();
			}

			collider.isTrigger = false;
			collider.radius = ColliderRadius / 1000;
		}

		private void addRigidBody()
		{
			Rigidbody rigidBody = this.gameObject.GetComponent<Rigidbody>();
			if(rigidBody == null)
			{
				rigidBody = this.gameObject.AddComponent<Rigidbody>();
			}

			rigidBody.mass = 0.0f;
			rigidBody.drag = 0.0f;
			rigidBody.angularDrag = 0.0f;
			rigidBody.useGravity = false;
			rigidBody.isKinematic = true;
			rigidBody.interpolation = RigidbodyInterpolation.None;
			rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
		}
	}
}