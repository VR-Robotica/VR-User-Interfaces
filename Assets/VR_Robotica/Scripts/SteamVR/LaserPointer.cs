using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// https://www.raywenderlich.com/149239/htc-vive-tutorial-unity
/// </summary>

namespace VR_Robotica.Vive
{
	public class LaserPointer : MonoBehaviour
	{
		// A reference to the object being tracked. In this case, a controller.
		private SteamVR_TrackedObject trackedObj;

		// This is a reference to the Laser’s prefab.
		public GameObject laserPrefab;

		// laser stores a reference to an instance of the laser.
		private GameObject laser;

		// The transform component is stored for ease of use.
		private Transform laserTransform;

		// This is the position where the laser hits.
		private Vector3 hitPoint;

		// Is the transform of [CameraRig].
		public Transform cameraRigTransform;

		// Stores a reference to the teleport reticle prefab.
		public GameObject teleportReticlePrefab;

		// A reference to an instance of the reticle.
		private GameObject reticle;

		// Stores a reference to the teleport reticle transform for ease of use.
		private Transform teleportReticleTransform;

		// Stores a reference to the player’s head (the camera).
		public Transform headTransform;

		// Is the reticle offset from the floor, so there’s no “Z-fighting” with other surfaces.
		public Vector3 teleportReticleOffset;

		// Is a layer mask to filter the areas on which teleports are allowed.
		public LayerMask teleportMask;

		// Is set to true when a valid teleport location is found.
		private bool shouldTeleport;

		// A Device property to provide easy access to the controller. It uses the tracked object’s index to return the controller’s input.
		private SteamVR_Controller.Device Controller
		{
			get { return SteamVR_Controller.Input((int)trackedObj.index); }
		}

		void Awake()
		{
			trackedObj = GetComponent<SteamVR_TrackedObject>();
		}

		// Use this for initialization
		void Start()
		{
			// Spawn a new laser and save a reference to it in laser.
			laser = Instantiate(laserPrefab);
			// Store the laser’s transform component.
			laserTransform = laser.transform;

			// Spawn a new reticle and save a reference to it in reticle.
			reticle = Instantiate(teleportReticlePrefab);
			// Store the reticle’s transform component.
			teleportReticleTransform = reticle.transform;
		}

		// Update is called once per frame
		void Update()
		{
			// If the touchpad is held down…
			if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
			{
				RaycastHit hit;

				// Shoot a ray from the controller. If it hits something, make it store the point where it hit and show the laser.
				//			if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100))
				if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, teleportMask))
				{
					hitPoint = hit.point;
					ShowLaser(hit);

					// Show the teleport reticle.
					reticle.SetActive(true);
					// Move the reticle to where the raycast hit with the addition of an offset to avoid Z-fighting.
					teleportReticleTransform.position = hitPoint + teleportReticleOffset;
					// Set shouldTeleport to true to indicate the script found a valid position for teleporting.
					shouldTeleport = true;
				}
			}
			else // Hide the laser when the player released the touchpad.
			{
				if(laser != null)
					laser.SetActive(false);

				if(reticle != null)
					reticle.SetActive(false);
			}

			// This teleports the player if the touchpad is released and there’s a valid teleport position.
			if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && shouldTeleport)
			{
				Teleport();
			}
		}

		private void ShowLaser(RaycastHit hit)
		{
			// Show the laser.
			laser.SetActive(true);
			// Position the laser between the controller and the point where the raycast hits. You use Lerp because you can give it two positions and the percent it should travel. If you pass it 0.5f, which is 50%, it returns the precise middle point.
			laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
			// Point the laser at the position where the raycast hit.
			laserTransform.LookAt(hitPoint);
			// Scale the laser so it fits perfectly between the two positions.
			laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
		}

		private void Teleport()
		{
			// Set the shouldTeleport flag to false when teleportation is in progress.
			shouldTeleport = false;

			// Hide the reticle.
			reticle.SetActive(false);

			// Calculate the difference between the positions of the camera rig’s center and the player’s head.
			Vector3 difference = cameraRigTransform.position - headTransform.position;

			// Reset the y-position for the above difference to 0, because the calculation doesn’t consider the vertical position of the player’s head.
			difference.y = 0;

			// Move the camera rig to the position of the hit point and add the calculated difference. Without the difference, the player would teleport to an incorrect location.
			cameraRigTransform.position = hitPoint + difference;
		}
	}
}