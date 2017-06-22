using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.VR_Robotica.VRUI.LeapMotion
{
	public class LeapUI_PalmMenu : MonoBehaviour
	{
		private Transform _palmCenter;
		private Collider _palmCollider;


		// Use this for initialization
		void Start()
		{
			
			createPalmTrigger();
		}

		// Update is called once per frame
		void Update()
		{

		}

		private void OnTriggerEnter(Collider other)
		{

		}

		private void OnTriggerStay(Collider other)
		{

		}

		private void OnTriggerExit(Collider other)
		{

		}

		private void createPalmTrigger()
		{
			_palmCollider = this.gameObject.GetComponent<SphereCollider>();
			if(_palmCollider == null)
			{
				_palmCollider = this.gameObject.AddComponent<SphereCollider>();
			}
		}
	}
}