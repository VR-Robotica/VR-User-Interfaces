using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.VR_Robotica.VRUI
{
	public class VRUI_HandSwipe : MonoBehaviour
	{
		private Collider[]	_swipeColliders;
		private bool[]		_activeCollider;

		private void Awake()
		{
			getSwipeColliders();
		}

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}

		private void getSwipeColliders()
		{
			int numberOfColliders = this.transform.childCount;
			_swipeColliders = new Collider[numberOfColliders];

			for(int i = 0; i < numberOfColliders; i++)
			{
				_swipeColliders[i] = this.transform.GetChild(i).GetComponent<Collider>();
				//Debug.Log("Got Collider: " + _swipeColliders[i].gameObject.name);
			}
		}
	}
}