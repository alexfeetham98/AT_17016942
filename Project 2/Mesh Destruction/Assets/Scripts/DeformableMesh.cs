using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DeformableMesh : MonoBehaviour
{
	//Deform Properties
	[Range(0.0f, 3)]
	public float malleability = 0.05f;
	[Range(0.0f, 0.5f)]
	public float radius = 0.1f;
	//Destroy Properties
	[Range(0, 15)]
	public int maxHits = 10;
	[Range(1, 10)]
	public int thisObjectCuts = 10;
	[Range(0, 5)]
	public int thisObjectExplosiveForce = 1;
	//Debris Properties
	public bool enableDebris = false;
	[Range(1, 10)]
	public int debrisCuts = 5;
	[Range(0, 5)]
	public int debrisExplosiveForce = 1;
	public bool destroyDebrisAfterTime = true;
	[Range(1, 10)]
	public float time = 3.0f;

	//Debris Components
	private GameObject debrisBase;
	private MeshCollider meshCollider;
	private MeshRenderer meshRenderer;
	//public int material = 0;

	private Mesh mesh;
	private Vector3[] verticies;
	private int hits = 0;

	private void Start()
	{
		if (enableDebris)
		{
			debrisBase = GameObject.Find("DebrisBase");		
			meshRenderer = GetComponent<MeshRenderer>();
		}
		mesh = GetComponent<MeshFilter>().mesh;
		meshCollider = GetComponent<MeshCollider>();
		this.gameObject.AddComponent<MeshDestroy>();
		this.gameObject.GetComponent<MeshDestroy>().NumCuts = thisObjectCuts;
		this.gameObject.GetComponent<MeshDestroy>().ExplodeForce = thisObjectExplosiveForce;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Projectile" || collision.gameObject.tag == "Tool")
		{
			if (hits < maxHits)
			{
				//Get point, normal, and impulse
				Vector3 point3 = transform.InverseTransformPoint(collision.GetContact(0).point);
				Vector3 normal = transform.InverseTransformDirection(collision.GetContact(0).normal);
				float impluse = collision.impulse.magnitude;

				verticies = mesh.vertices;
				float scale;
				for (int i = 0; i < verticies.Length; i++)
				{
					//Get deformation scale based on distance
					scale = Mathf.Clamp(radius - (point3 - verticies[i]).magnitude, 0, radius);

					//Deform the verticies based on the impluse, scale and malleability
					verticies[i] += normal * impluse * scale * malleability;
				}

				//Apply changes to collider and mesh
				mesh.vertices = verticies;
				meshCollider.sharedMesh = mesh;

				//Recalculate mesh normal and bounds - Unity functions
				mesh.RecalculateNormals();
				mesh.RecalculateBounds();

				if (collision.gameObject.tag == "Projectile")
				{
					Destroy(collision.gameObject);
				}
			}
			else if (hits > maxHits)
			{
				this.gameObject.GetComponent<MeshDestroy>().destroyAfterTime = true;
				this.gameObject.GetComponent<MeshDestroy>().time = 10.0f;
				this.gameObject.GetComponent<MeshDestroy>().canBreakChildObjs = false;
				this.gameObject.GetComponent<MeshDestroy>().DestroyMesh();
			}

			if (enableDebris)
			{
				//Create new game object
				GameObject debris = Instantiate(debrisBase, collision.transform.position, collision.transform.rotation) as GameObject;

				#region Old Property Setting
				//switch(material)
				//{
				//	case 0:
				//		debris.GetComponent<MeshDestroy>().debrisMat = MeshDestroy.Material.Wood;
				//		debris.GetComponent<MeshDestroy>().CutCascades = 8;
				//		break;
				//	case 1:
				//		debris.GetComponent<MeshDestroy>().debrisMat = MeshDestroy.Material.Brick;
				//		debris.GetComponent<MeshDestroy>().CutCascades = 10;
				//		break;
				//	case 2:
				//		debris.GetComponent<MeshDestroy>().debrisMat = MeshDestroy.Material.Tile;

				//		break;
				//	case 3:
				//		debris.GetComponent<MeshDestroy>().debrisMat = MeshDestroy.Material.Metal;
				//		break;
				//}
				#endregion

				debris.GetComponent<MeshDestroy>().NumCuts = debrisCuts;                        //Set the amount of slices for the debris chunk
				debris.GetComponent<MeshRenderer>().material = meshRenderer.material;           //Set the material to match that of the object that was hit
				debris.GetComponent<MeshDestroy>().ExplodeForce = debrisExplosiveForce;         //Set the scale of how much force to apply to the debris
				debris.GetComponent<MeshDestroy>().destroyAfterTime = destroyDebrisAfterTime;   //Enable automatic destruction of debris
				debris.GetComponent<MeshDestroy>().time = time;                                 //Set the time after which the debris will auto destroy itself
				debris.GetComponent<MeshDestroy>().canBreakChildObjs = false;                   //Don't want to be able to break the debris by shooting it
				debris.transform.localScale = new Vector3(radius * 2, radius * 2, radius);      //Set the scale of the debris object to match that of the hole created by the deform
				debris.GetComponent<MeshDestroy>().DestroyMesh();                               //Slice the debris

				#region Threading Leftovers

				///Unity does not not allow you to edit anything to do with
				///a game object in child threads.
				///The intention would have been to calcute the mesh slicing
				///on a child thread to reduce fps lag however its just not allowed.

				//Thread destroyThread = new Thread(debris.GetComponent<MeshDestroy>().DestroyMesh);
				//lock (debris)
				//{
				//    destroyThread.Start();
				//}
				//while (destroyThread.IsAlive) { Debug.Log("Thread Running"); }
				#endregion

			}
			hits++;
		}
	}
}