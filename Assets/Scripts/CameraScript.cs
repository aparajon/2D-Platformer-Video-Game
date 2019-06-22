using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraScript : MonoBehaviour {
	// Sets follow to player on play.
	private CinemachineVirtualCamera virCam;
	private CinemachineConfiner confCam;
	// Use this for initialization
	void Start () {
		confCam = GetComponent<CinemachineConfiner>();
		virCam = GetComponent<CinemachineVirtualCamera>();
		virCam.m_Follow = GameObject.FindGameObjectWithTag("Player").transform;
		confCam.m_BoundingShape2D = GameObject.FindGameObjectWithTag("BoundingObject").GetComponent<PolygonCollider2D>();
	}
}
