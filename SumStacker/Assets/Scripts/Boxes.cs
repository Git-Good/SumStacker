﻿using UnityEngine;
using System.Collections;

public class Boxes : MonoBehaviour {

	float fall = 0;
	public static int gridWeight = 5;
	public static int gridHeight = 8;
	public static Transform[,] grid = new Transform[gridWeight, gridHeight];
	public int currenttotal = 0;
	public static int goaltotal = 10;

	private bool canMove = true;

	void Start () {
		if (!isValidPosition()) {
			Application.LoadLevel(0);
			Destroy(gameObject);
		}
		canMove = true;
	}

	void Update() {
		
		if (Input.GetKeyDown(KeyCode.RightArrow)|| Input.GetKeyDown(KeyCode.D)) {
			transform.position += new Vector3(1, 0, 0);

			if (isValidPosition())
				GridUpdate();
			else
				transform.position += new Vector3(-1, 0, 0);
		}

		else if (Input.GetKeyDown(KeyCode.LeftArrow)|| Input.GetKeyDown(KeyCode.A)) {
			transform.position += new Vector3(-1, 0, 0);
			if (isValidPosition())
					GridUpdate();
			else
				transform.position += new Vector3(1, 0, 0);
		}
		/*
		else if (Input.GetKeyDown(KeyCode.UpArrow)) {
			transform.Rotate(0, 0, -90);
			if (isValidPosition())
				GridUpdate();
			else
				transform.Rotate(0, 0, 90);
		}*/

		else if (Input.GetKeyDown(KeyCode.DownArrow) ||
		         Time.time - fall >= 1 || Input.GetKeyDown(KeyCode.S)) {
			transform.position += new Vector3(0, -1, 0);
			if (isValidPosition()) {
				GridUpdate();
			} else {
				transform.position += new Vector3(0, 1, 0);
				DeleteRow();
				FindObjectOfType<SpawnBox>().SpawnNewBox();
				enabled = false;
			}
			
			fall = Time.time;
		}
	}

	bool isValidPosition() { 
		if (canMove) {
			foreach (Transform child in transform) {
				Vector2 v = round (child.position);
				if (!isInsideGrid (v))
					return false;
				if (grid [(int)v.x, (int)v.y] != null &&
					grid [(int)v.x, (int)v.y].parent != transform)
					return false;
			}
			return true;
		}
		return false;
	}

	void GridUpdate() {
		for (int y = 0; y < gridHeight; ++y)
			for (int x = 0; x < gridWeight; ++x)
				if (grid[x, y] != null)
					if (grid[x, y].parent == transform)
						grid[x, y] = null;
		foreach (Transform child in transform) {
			Vector2 v = round(child.position);
			grid[(int)v.x, (int)v.y] = child;
		}        
	}
	public static Vector2 round(Vector2 v) {
		return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
	}
	
	public static bool isInsideGrid(Vector2 pos) {
		return ((int)pos.x >= 0 && (int)pos.x < gridWeight && (int)pos.y >= 0);
	}
	
	public static void Delete(int y) {
		for (int x = 0; x < gridWeight; ++x) {
			Destroy(grid[x, y].gameObject);
			grid[x, y] = null;
		}
	}

	public static bool isFull(int y) {
		for (int x = 0; x < gridWeight; ++x)
			if (grid[x, y] == null)
				return false;
		return true;
	}

	public bool equalsTotal(int y) {
		for (int x = 0; x < gridWeight; ++x)
			currenttotal++; //adds the total of the box values at grid[x, y]
		if (currenttotal == goaltotal) {
			currenttotal = 0;
			return true;
		} else {
			currenttotal = 0;
			return false;
		}
	}

	public void DeleteRow() {
		for (int y = 0; y < gridHeight; ++y) {
			if (isFull(y) && equalsTotal(y)) {
				Delete(y);
				RowDownAll(y+1);
				--y;
			}
		}
	}
	
	public static void RowDown(int y) {
		for (int x = 0; x < gridWeight; ++x) {
			if (grid[x, y] != null) {
				grid[x, y-1] = grid[x, y];
				grid[x, y] = null;
				grid[x, y-1].position += new Vector3(0, -1, 0);
			}
		}
	}
	
	public static void RowDownAll(int y) {
		for (int i = y; i < gridHeight; ++i)
			RowDown(i);
	}

	void OnCollisionEnter2D(Collision2D coll) {
		canMove = false;
		
	}
	void OnTriggerEnter2D(Collider2D other) {
		canMove = false;
	}
}
