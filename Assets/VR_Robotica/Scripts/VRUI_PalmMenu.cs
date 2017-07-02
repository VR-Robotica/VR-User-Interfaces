using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// LEAP UI Elements are based on Leap's Unity SDK packages:
/// - CoreAssets 4.2.0 
/// - Interaction Engine 1.0.1
/// - Hand Module 2.1.2
/// </summary>

namespace com.VR_Robotica.VRUI.LeapMotion
{
	public enum FingerStates { Undefined, Open, Closed, Pinch};

	public class VRUI_PalmMenu : MonoBehaviour
	{
		public GameObject MenuPrefab;
		[Space]
		public  bool			AreAllFingerTips_Closed;
		public  bool			AreAllFingerTips_Opened;

		private FingerStates _fingerStates = FingerStates.Undefined;
		public  FingerStates FingerStates
		{
			get
			{
				return _fingerStates;
			}
			set
			{
				if (_fingerStates == value)
				{
					return;
				}
				_fingerStates = value;
				OnStateChange(_fingerStates);
			}
		}
		private FingerStates prevState;

		private Animator		_menuAnimator;
		private bool[]			_areFingersClosed = new bool[5];
		private Transform		_palmCenter;
		private SphereCollider	_palmCollider;
		private bool			_isMenuOpen;
		private bool			_isButtonPressed;

		public delegate void Delegate_OnStateChange(FingerStates newState);
		public event Delegate_OnStateChange OnStateChange;



		// Use this for initialization
		private void Start()
		{
			createPalmTrigger();
			createMenu();
			getReferences();

			this.OnStateChange += handler_onStateChange;
		}

		// Update is called once per frame
		private void Update()
		{
			update_isPalmFacingUp();
			update_fingerTipCollisions();
		}

		private void OnTriggerEnter(Collider collider)
		{
			// check if collision object has fingerTip component...
			Collider_FingerTip fingerTip = collider.gameObject.GetComponent<Collider_FingerTip>();
			// if so, check fingerTip name...
			if (fingerTip != null)
			{
				switch (fingerTip.NameOfFinger)
				{
					case Collider_FingerTip.FingerName.Index:
						_areFingersClosed[0] = true;
						break;

					case Collider_FingerTip.FingerName.Middle:
						_areFingersClosed[1] = true;
						break;

					case Collider_FingerTip.FingerName.Ring:
						_areFingersClosed[2] = true;
						break;

					case Collider_FingerTip.FingerName.Pinky:
						_areFingersClosed[3] = true;
						break;

					case Collider_FingerTip.FingerName.Thumb:
						_areFingersClosed[4] = true;
						break;
				}
			}

			if (_fingerStates == FingerStates.Closed && _isButtonPressed == false)
			{
				// start the timer for finger release
				StartCoroutine(delayButton());
			}
		}

		private void OnTriggerStay(Collider collider)
		{

		}

		private void OnTriggerExit(Collider collider)
		{
			// check if collision object has fingerTip component...
			Collider_FingerTip fingerTip = collider.gameObject.GetComponent<Collider_FingerTip>();
			// if so, check fingerTip name...
			if (fingerTip != null)
			{
				switch (fingerTip.NameOfFinger)
				{
					case Collider_FingerTip.FingerName.Index:
						_areFingersClosed[0] = false;
						break;
					case Collider_FingerTip.FingerName.Middle:
						_areFingersClosed[1] = false;
						break;
					case Collider_FingerTip.FingerName.Ring:
						_areFingersClosed[2] = false;
						break;
					case Collider_FingerTip.FingerName.Pinky:
						_areFingersClosed[3] = false;
						break;
					case Collider_FingerTip.FingerName.Thumb:
						_areFingersClosed[4] = false;
						break;
				}
			}

			if(_fingerStates == FingerStates.Open && _isButtonPressed)
			{
				// toggle menu
			//	Debug.Log("Toggling Menu");
			}
		}

		private void handler_onStateChange(FingerStates newState)
		{
			if (newState != prevState)
			{
				Debug.Log("State Changed: " + newState);
				if (newState == FingerStates.Open && _isButtonPressed)
				{
					if (_isMenuOpen)
					{
						closeMenu();
					}
					else
					{
						openMenu();
					}
				}

				if (newState == FingerStates.Closed && _isButtonPressed == false)
				{
					StartCoroutine(delayButton());
				}
			}
			prevState = newState;
		}

		private void update_isPalmFacingUp()
		{
			bool isPalmFacingUp;

			if (Vector3.Dot(_palmCenter.transform.up, Vector3.up) > 0.65f)
			{
				isPalmFacingUp = true;
			}
			else
			{
				isPalmFacingUp		= false;

				// if palm is not active, reset booleans
				_fingerStates = FingerStates.Undefined;
				AreAllFingerTips_Closed	= false;
				AreAllFingerTips_Opened = false;
			}

			enablePalmCollider(isPalmFacingUp);
		}

		private void update_fingerTipCollisions()
		{
			int count_closed = 0;
			int count_open = 0;

			for(int i = 0; i < _areFingersClosed.Length; i++)
			{
				// if a finger is NOT CLOSED (aka OPENED)
				if(_areFingersClosed[i] == false) // false = OPENED
				{
					// Then NOT ALL fingers are closed
					AreAllFingerTips_Closed = false;
					// count the finger
					count_open++;
					// if the number of fingers NOT closed is equal to all the fingers...
					if(count_open == _areFingersClosed.Length)
					{
						//Debug.Log("All Fingers Are OPENED");
						AreAllFingerTips_Opened = true;
						FingerStates = FingerStates.Open;
					}
				}

				if(_areFingersClosed[i] == true) // true = CLOSED
				{
					// if any finger is closed, then NOT ALL fingers are open...
					AreAllFingerTips_Opened = false;
					// start counting each closed finger
					count_closed++;

					if(count_closed == _areFingersClosed.Length)
					{
						//Debug.Log("All Fingers Are CLOSED");
						AreAllFingerTips_Closed = true;
						FingerStates = FingerStates.Closed;
					}
				}
			}
		}

		private void openMenu()
		{
			Debug.Log("Opening Menu");
			_isMenuOpen = true;
			//createTestSphere();
			MenuPrefab.transform.parent = null;
			MenuPrefab.transform.position = new Vector3(_palmCenter.position.x,
															_palmCenter.position.y + 0.1f,
															_palmCenter.position.z
															);
			_menuAnimator.SetTrigger("triggerMenu");
		}

		private void closeMenu()
		{
			Debug.Log("Closing Menu");
			_isMenuOpen = false;
			MenuPrefab.transform.parent = _palmCenter.transform;
			MenuPrefab.transform.position = _palmCenter.transform.position;
			_menuAnimator.SetTrigger("triggerMenu");
		}

		private void createTestSphere()
		{
			GameObject newObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			newObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			newObject.transform.position = _palmCollider.transform.position;
		}

		private void enablePalmCollider(bool enable)
		{
			_palmCollider.enabled = enable;
		}

		private void createPalmTrigger()
		{
			_palmCenter = this.transform;
			// Get Collider Component... 
			_palmCollider = this.gameObject.GetComponent<SphereCollider>();
			//if it doesn't exist, create one...
			if (_palmCollider == null)
			{
				_palmCollider = this.gameObject.AddComponent<SphereCollider>();
				_palmCollider.isTrigger			 = true;
				_palmCollider.radius = 0.03f;
				_palmCollider.center = new Vector3(-0.1f, 0.07f, -0.01f);
			}
		}

		private void createMenu()
		{
			if (MenuPrefab == null)
			{
				MenuPrefab = Instantiate(Resources.Load("Prefabs/Menus/PalmMenu")) as GameObject;

				if (MenuPrefab == null)
				{
					Debug.LogWarning("Failed to load PalmMenu Prefab!");
				}
				else
				{
					Debug.Log("PalmMenu Prefab LOADED!");
					
					MenuPrefab.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
				}
			}
		}

		private void getReferences()
		{
			if(MenuPrefab != null)
			{
				

				_menuAnimator = MenuPrefab.gameObject.GetComponent<Animator>();
				if(_menuAnimator == null)
				{
					Debug.LogWarning("Menu does not have an Animation Component!");
				}
			}
		}

		private IEnumerator delayButton()
		{
			// A simple boolean timer that allows a short window for the fingers tips
			// to exit the trigger area

		//	Debug.Log("Button Delay");

			_isButtonPressed = true;
			yield return new WaitForSeconds(2.0f);
			_isButtonPressed = false;
		}
	}
}