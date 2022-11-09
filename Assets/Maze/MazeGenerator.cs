using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using Random = UnityEngine.Random;


public class MazeGenerator : SerializedMonoBehaviour
{
	public int size;
	public int minimumLength;
	public Color[,] mazeDebug;
	public float waitTime;
	public float hue;
	public GameObject prefab, prefab2, prefab3, prefab4, playerobj, loadscreen;
	public GameObject snow, snow2, snow3, snow4;
	public GameObject desert, taiga;
	public float offset;
	public float localOffset;
	public int timeTBS;
	public bool bypassWait;
	public Vector3 positionOffset;
	Cell[,] maze;


	List<Cell> borderCells = new List<Cell>();

	Cell currentCell;
	int currentLength;

	private void Start()
	{
		Vector2 startPoint;
		maze = new Cell[size, size];
		mazeDebug = new Color[size, size];
		currentLength = 0;
		for (int x = 0; x < size; x++)
		{
			for (int y = 0; y < size; y++)
			{
				bool isAtBorder = false;
				if (x == 0 || y == 0 || x == size - 1 || y == size - 1)
				{
					isAtBorder = true;
				}
				Cell cell = new Cell(isAtBorder, new Vector2(x, y), timeTBS);
				maze[x, y] = cell;
			}
		}
		borderCells = maze.OfType<Cell>().ToList().Where(x => x.isAtBorder.Equals(true)).ToList();
		startPoint = borderCells[Random.Range(0, borderCells.Count)].position;
		CustomEventSystem.current.onUpdateDebug += UpdateDebug;
		GenerateFirstCell(startPoint, false);
		if(Menu.biome ==true)
		{
			desert.SetActive(true);
			taiga.SetActive(false);
		}
		else
		{
			taiga.SetActive(true);
			desert.SetActive(false);
		}
		//GameObject player = Instantiate(playerobj, positionOffset, Quaternion.identity);

	}
	public void GenerateFirstCell(Vector2 startPosition, bool isBranch)
	{
		if (startPosition.x != 500)
		{
			currentCell = maze[(int)startPosition.x, (int)startPosition.y];
			if (!isBranch)
			{
				maze[(int)startPosition.x, (int)startPosition.y].isUsed = true;
				List<Direction> directionToBorder = CalculateDirectionsToBorder(startPosition);
				maze[(int)startPosition.x, (int)startPosition.y].RemoveBorderOnExit(directionToBorder.First());
			}
			GenerateStep(startPosition, isBranch);
			
		}
	}
	public void GenerateStep(Vector2 cellPosition, bool isBranch)
	{
		bool finish;
		int x = (int)cellPosition.x;
		int y = (int)cellPosition.y;
		List<Direction> availableDirections = new List<Direction>();
		if (!isBranch)
		{
			availableDirections = CalculatePossibleDirections(currentCell.position, out finish);
		}
		else
		{
			finish = false;
			availableDirections = CalculatePossibleDirections(currentCell.position);

		}
		CustomEventSystem.current.UpdateTimeTBS();
		if (availableDirections.Count != 0 && !finish)
		{
			Direction moveTo = availableDirections[Random.Range(0, availableDirections.Count)];
			Vector2 nextCell = Move(cellPosition, moveTo);
			print($"nextcell.x = {nextCell.x}, nextcell.y ={nextCell.y}");
			maze[x, y].isUsed = true;
			mazeDebug[x, y] = Color.HSVToRGB(hue, 0.01f * currentLength, 1);
			maze[(int)nextCell.x, (int)nextCell.y].previousCellPos = cellPosition;
			maze[x, y].RemoveBorderOnExit(moveTo);
			if (Menu.biome == true)
			{
				maze[x, y].SetBorders(offset, localOffset, positionOffset, prefab, prefab2, prefab3, prefab4);
			}
			if (Menu.biome == false)
			{
				maze[x, y].SetBorders(offset, localOffset, positionOffset, snow, snow2, snow3, snow4);
			}
			maze[(int)nextCell.x, (int)nextCell.y].RemoveBorderOnEntry(moveTo);
			currentCell = maze[(int)nextCell.x, (int)nextCell.y];
			currentLength++;
			if (!bypassWait)
			{
				StartCoroutine(Wait(currentCell.position, isBranch));
			}
			else
			{
				GenerateStep(currentCell.position, isBranch);
				loadscreen.active = false;
			}
			
		}
		else if (availableDirections.Count == 0 && !finish && !isBranch)
		{
			Vector2 previousCell = maze[x, y].previousCellPos;
			maze[x, y].ChangeToUnsuitable(timeTBS);
			maze[x, y].isUsed = false;
			Direction directionToCell = DirectionToCell(maze[x, y].position, maze[(int)previousCell.x, (int)previousCell.y].position);
			maze[x, y].AddBorderOnentry(directionToCell);
			maze[x, y].RemoveBorders();
			maze[(int)previousCell.x, (int)previousCell.y].AddBorderOnExit(directionToCell);
			mazeDebug[x, y] = Color.red;
			currentCell = maze[(int)previousCell.x, (int)previousCell.y];
			currentLength--;
			if (!bypassWait)
			{
				StartCoroutine(Wait(currentCell.position, isBranch));
			}
			else
			{
				GenerateStep(currentCell.position, isBranch);
			}
		}
		else if (availableDirections.Count == 0 && isBranch)
		{
			maze[x, y].isUsed = true;
			if(Menu.biome == true)
			{
				maze[x, y].SetBorders(offset, localOffset, positionOffset, prefab, prefab2, prefab3, prefab4);
			}
			if (Menu.biome == false)
			{
				maze[x, y].SetBorders(offset, localOffset, positionOffset, snow, snow2, snow3, snow4);
			}

			GenerateBranches();
			print("nextbranch");
		}
		else if (finish)
		{
			maze[x, y].isUsed = true;
			mazeDebug[x, y] = Color.HSVToRGB(hue, 0.01f * currentLength, 1);
			currentLength++;
			List<Direction> directionToBorder = CalculateDirectionsToBorder(new Vector2(x, y));
			//maze[x, y].RemoveBorderOnExit(directionToBorder.First());
			if (Menu.biome == true)
			{
				maze[x, y].SetBorders(offset, localOffset, positionOffset, prefab, prefab2, prefab3, prefab4);
			}
			if (Menu.biome == false)
			{
				maze[x, y].SetBorders(offset, localOffset, positionOffset, snow, snow2, snow3, snow4);
			}
			ClearTBS();
			GenerateBranches();
			
		}
	}
	public List<Direction> CalculatePossibleDirections(Vector2 cellPosition, out bool finish)
	{
		bool skip = false;
		int x = (int)cellPosition.x;
		int y = (int)cellPosition.y;
		List<Direction> returnValue = new List<Direction> { Direction.up, Direction.down, Direction.left, Direction.right };
		List<Direction> directionsToBorder = CalculateDirectionsToBorder(cellPosition);
		if (directionsToBorder.Count != 0 && currentLength < minimumLength)
		{
			foreach (var direction in directionsToBorder)
			{
				returnValue.Remove(direction);
			}
		}
		else if (directionsToBorder.Count != 0 && currentLength >= minimumLength)
		{
			returnValue.Clear();
			returnValue.Add(directionsToBorder.First());
			skip = true;
			
		}
		if (!skip)
		{
			if (returnValue.Contains(Direction.left))
			{
				if (maze[x - 1, y].isUsed)
					returnValue.Remove(Direction.left);
				if (maze[x - 1, y].isNotSuitable)
				{
					returnValue.Remove(Direction.left);
				}
			}
			if (returnValue.Contains(Direction.right))
			{
				if (maze[x + 1, y].isUsed)
					returnValue.Remove(Direction.right);
				if (maze[x + 1, y].isNotSuitable)
				{
					returnValue.Remove(Direction.right);
				}
			}
			if (returnValue.Contains(Direction.up))
			{
				if (maze[x, y - 1].isUsed)
					returnValue.Remove(Direction.up);
				if (maze[x, y - 1].isNotSuitable)
				{
					returnValue.Remove(Direction.up);
				}
			}
			if (returnValue.Contains(Direction.down))
			{
				if (maze[x, y + 1].isUsed)
					returnValue.Remove(Direction.down);
				if (maze[x, y + 1].isNotSuitable)
				{
					returnValue.Remove(Direction.down);
				}
			}
		}
		finish = skip;
		
		return returnValue;
	}
	public List<Direction> CalculatePossibleDirections(Vector2 cellPosition)
	{
		bool skip = false;
		int x = (int)cellPosition.x;
		int y = (int)cellPosition.y;
		List<Direction> returnValue = new List<Direction> { Direction.up, Direction.down, Direction.left, Direction.right };
		List<Direction> directionsToBorder = CalculateDirectionsToBorder(cellPosition);
		if (directionsToBorder.Count != 0)
		{
			foreach (var direction in directionsToBorder)
			{
				returnValue.Remove(direction);
			}
		}
		if (!skip)
		{
			if (returnValue.Contains(Direction.left))
			{
				if (maze[x - 1, y].isUsed)
					returnValue.Remove(Direction.left);
				if (maze[x - 1, y].isNotSuitable)
				{
					returnValue.Remove(Direction.left);
				}
			}
			if (returnValue.Contains(Direction.right))
			{
				if (maze[x + 1, y].isUsed)
					returnValue.Remove(Direction.right);
				if (maze[x + 1, y].isNotSuitable)
				{
					returnValue.Remove(Direction.right);
				}
			}
			if (returnValue.Contains(Direction.up))
			{
				if (maze[x, y - 1].isUsed)
					returnValue.Remove(Direction.up);
				if (maze[x, y - 1].isNotSuitable)
				{
					returnValue.Remove(Direction.up);
				}
			}
			if (returnValue.Contains(Direction.down))
			{
				if (maze[x, y + 1].isUsed)
					returnValue.Remove(Direction.down);
				if (maze[x, y + 1].isNotSuitable)
				{
					returnValue.Remove(Direction.down);
				}
			}
		}
		return returnValue;
	}
	public List<Direction> CalculateDirectionsToBorder(Vector2 cellPosition)
	{
		int x = (int)cellPosition.x;
		int y = (int)cellPosition.y;
		List<Direction> returnValue = new List<Direction>();
		if (x == 0)
		{
			returnValue.Add(Direction.left);
		}
		if (x == size - 1)
		{
			returnValue.Add(Direction.right);
		}
		if (y == 0)
		{
			returnValue.Add(Direction.up);
		}
		if (y == size - 1)
		{
			returnValue.Add(Direction.down);
		}
		return returnValue;
	}
	public Direction DirectionToCell(Vector2 currentCell, Vector2 targetCell)
	{
		Direction returnValue = Direction.none;
		float xc, xt, yc, yt;
		xc = currentCell.x;
		xt = targetCell.x;
		yc = currentCell.y;
		yt = targetCell.y;
		if (xc > xt)
		{
			return Direction.right;
		}
		else if (xc < xt)
		{
			return Direction.left;
		}
		else if (yc > yt)
		{
			return Direction.down;
		}
		else if (yc < yt)
		{
			return Direction.up;
		}
		return returnValue;
	}
	public Vector2 Move(Vector2 currentPos, Direction direction)
	{
		int x = (int)currentPos.x;
		int y = (int)currentPos.y;
		Vector2 returnValue = Vector2.zero;
		if (direction == Direction.up)
		{
			returnValue = new Vector2(x, y - 1);
		}
		else if (direction == Direction.down)
		{
			returnValue = new Vector2(x, y + 1);
		}
		else if (direction == Direction.left)
		{
			returnValue = new Vector2(x - 1, y);
		}
		else if (direction == Direction.right)
		{
			returnValue = new Vector2(x + 1, y);
		}
		return returnValue;

	}
	public IEnumerator Wait(Vector2 position, bool isBranch)
	{
		yield return new WaitForSeconds(waitTime);
		GenerateStep(position, isBranch);
	}
	public void UpdateDebug(Vector2 position)
	{
		mazeDebug[(int)position.x, (int)position.y] = Color.green;
	}

	public void GenerateBorders()
	{
		for (int x = 0; x < size; x++)
		{
			for (int z = 0; z < size; z++)
			{
				foreach (var dir in maze[z, x].borders)
				{
					Vector3 position = Vector3.zero;
					Vector3 rotation = Vector3.zero;
					Vector3 scaleChange;
					switch (dir)
					{
						case Direction.up:
							position = new Vector3(x * offset - localOffset, 0, z * offset);
							rotation = new Vector3(0, 90, 0);
							break;
						case Direction.down:
							position = new Vector3(x * offset + localOffset, 0, z * offset);
							rotation = new Vector3(0, 90, 0);
							break;
						case Direction.left:
							position = new Vector3(x * offset, 0, z * offset - localOffset);
							rotation = new Vector3(0, 0, 0);
							break;
						case Direction.right:
							position = new Vector3(x * offset, 0, z * offset + localOffset);
							rotation = new Vector3(0, 0, 0);
							break;
						case Direction.none:
							break;
						default:
							break;
					}
					int rand = Random.Range(1, 1000);
					if (rand >= 0 && rand <= 100)
					{
						Instantiate(prefab, position + positionOffset, Quaternion.Euler(rotation));
						scaleChange = new Vector3(Random.Range(1f, 5f) / 100, Random.Range(1f, 5f) / 100, Random.Range(1f, 5f) / 100);
						prefab.transform.localScale += scaleChange;
					}
					
				}
			}
		}
	}
	public void GenerateBranches()
	{
		print("est kontakt");
		int notUsedCells = 1;
		List<Cell> mazeList = maze.OfType<Cell>().ToList();
		Vector2 branchPosition = new Vector2(500, 500);
		float min = 0;
		float max = 1;
		hue = Random.Range(min, max);
		notUsedCells = mazeList.Where(x => x.isUsed.Equals(false)).ToList().Count;
		if (notUsedCells > 0)
		{
			List<Cell> usedCells = mazeList.Where(x => x.isUsed).ToList();
			for (int i = 0; i < usedCells.Count; i++)
			{
				List<Direction> possibleDirections = CalculatePossibleDirections(usedCells[i].position);
				if (possibleDirections.Count > 0)
				{
					branchPosition = usedCells[i].position;
					break;
				}
			}
		}
		else
		{
			//GenerateBorders();
		}
		GenerateFirstCell(branchPosition, true);
	}
	public void ClearTBS()
	{
		for (int x = 0; x < size; x++)
		{
			for (int y = 0; y < size; y++)
			{
				if (maze[x, y].isNotSuitable)
				{
					maze[x, y].timeTBS = 0;
				}
			}
		}
		CustomEventSystem.current.UpdateTimeTBS();
	}

}
public class Cell
{
	public bool isAtBorder;
	public bool isUsed;
	public bool isNotSuitable;
	public Vector2 position;
	public Vector2 previousCellPos;
	public int timeTBS;
	List<GameObject> borderList = new List<GameObject>();
	public List<Direction> borders = new List<Direction>();
	public Cell(bool _isAtBorder, Vector2 _position, int timeTBS)
	{
		this.timeTBS = timeTBS;
		borders.Add(Direction.up);
		borders.Add(Direction.down);
		borders.Add(Direction.left);
		borders.Add(Direction.right);
		isUsed = false;
		isAtBorder = _isAtBorder;
		position = _position;
		isNotSuitable = false;
		CustomEventSystem.current.onUpdateTimeTBS += UpdateTimeTBS;
	}
	public void RemoveBorderOnEntry(Direction direction)
	{
		borders.Remove(InvertedDirection(direction));
	}
	public void RemoveBorderOnExit(Direction direction)
	{
		borders.Remove(direction);
	}
	public void AddBorderOnentry(Direction direction)
	{
		borders.Add(InvertedDirection(direction));
	}
	public void AddBorderOnExit(Direction direction)
	{
		borders.Add(direction);
	}
	public void ChangeToUnsuitable(int timeLeft)
	{
		timeTBS = timeLeft;
		isNotSuitable = true;
	}
	public void SetBorders(float offset, float localOffset, Vector3 positionOffset, GameObject prefab, GameObject prefab2, GameObject prefab3, GameObject prefab4)
	{
		int x = (int)position.x;
		int y = (int)position.y;
		RemoveBorders();
		foreach (var dir in borders)
		{
			Vector3 wallPos = Vector3.zero;
			Vector3 wallRot = Vector3.zero;
			Vector3 scaleChange;
			switch (dir)
			{
				case Direction.left:
					wallPos = new Vector3(x * offset - localOffset, 0, y * offset);
					wallRot = new Vector3(0, 90, 0);
					break;
				case Direction.right:
					wallPos = new Vector3(x * offset + localOffset, 0, y * offset);
					wallRot = new Vector3(0, 90, 0);
					break;
				case Direction.up:
					wallPos = new Vector3(x * offset, 0, y * offset - localOffset);
					wallRot = new Vector3(0, 0, 0);
					break;
				case Direction.down:
					wallPos = new Vector3(x * offset, 0, y * offset + localOffset);
					wallRot = new Vector3(0, 0, 0);
					break;
				case Direction.none:
					break;
				default:
					break;
			}
			int rand = Random.Range(1, 1000);
			if (rand >= 0 && rand <= 100)
			{
				GameObject go = UnityEngine.Object.Instantiate(prefab, wallPos + positionOffset, Quaternion.Euler(wallRot));
				scaleChange = new Vector3(Random.Range(1f, 5f) / 100, Random.Range(1f, 5f) / 100, Random.Range(1f, 5f) / 100);
				go.transform.localScale += scaleChange;
				borderList.Add(go);
			}
			if (rand > 100 && rand <= 180)
			{
				GameObject go = UnityEngine.Object.Instantiate(prefab2, wallPos + positionOffset, Quaternion.Euler(wallRot));
				scaleChange = new Vector3(Random.Range(1f, 5f) / 100, Random.Range(1f, 5f) / 100, Random.Range(1f, 5f) / 100);
				go.transform.localScale += scaleChange;
				borderList.Add(go);
			} 
			if (rand > 180 && rand <= 600)
			{
				GameObject go = UnityEngine.Object.Instantiate(prefab3, wallPos + positionOffset, Quaternion.Euler(wallRot));
				scaleChange = new Vector3(Random.Range(1f, 5f) / 100, Random.Range(1f, 5f) / 100, Random.Range(1f, 5f) / 100);
				go.transform.localScale += scaleChange;
				borderList.Add(go);
			}
			if (rand > 600 && rand <= 1000)
			{
				GameObject go = UnityEngine.Object.Instantiate(prefab4, wallPos + positionOffset, Quaternion.Euler(wallRot));
				scaleChange = new Vector3(Random.Range(1f, 5f) / 100, Random.Range(1f, 5f) / 100, Random.Range(1f, 5f) / 100);
				go.transform.localScale += scaleChange;
				borderList.Add(go);
			}

		}
	}
	public void RemoveBorders()
	{
		foreach (var item in borderList)
		{
			UnityEngine.Object.Destroy(item);
		}
		borderList.Clear();
	}
	private void UpdateTimeTBS()
	{
		if (timeTBS == 0 && isNotSuitable)
		{
			isNotSuitable = false;
			CustomEventSystem.current.UpdateDebug(position);
		}
		else
		{
			timeTBS--;
		}
	}
	public Direction InvertedDirection(Direction dir)
	{
		Direction returnValue = Direction.none;
		if (dir == Direction.up)
			returnValue = Direction.down;
		else if (dir == Direction.down)
			returnValue = Direction.up;
		else if (dir == Direction.left)
			returnValue = Direction.right;
		else if (dir == Direction.right)
			returnValue = Direction.left;
		return returnValue;

	}


}

public enum Direction
{
	up, down, left, right, none
}

