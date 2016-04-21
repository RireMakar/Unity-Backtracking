using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// the maze itself. man, who could have ever guessed? luckily this comment is here to explain the confusing class name.
public class Maze : MonoBehaviour {

    // attributes
	public IntVector2 size; // size in cells
    public float generationStepDelay; // delay for coroutine (crank it up if you want to slow down the generation for visualization purposes)
	private MazeCell[,] cells; // contains all the cells in the maze
    // prefabs
    public MazeCell cellPrefab;
    public MazePassage passagePrefab;
    public MazeWall wallPrefab;
    
    // returns a random cell location
	public IntVector2 RandomCoordinates {
		get {
			return new IntVector2(Random.Range(0, size.x), Random.Range(0, size.z));
		}
	}
    
    // checks to see whether the coordinate is within the maze
	public bool ContainsCoordinates (IntVector2 coordinate) {
		return coordinate.x >= 0 && coordinate.x < size.x && coordinate.z >= 0 && coordinate.z < size.z;
	}

    // gets a specified cell
	public MazeCell GetCell (IntVector2 coordinates) {
		return cells[coordinates.x, coordinates.z];
	}

    // generates a maze
    public IEnumerator Generate()
    {
        WaitForSeconds delay = new WaitForSeconds(generationStepDelay); // sets delay between steps
        cells = new MazeCell[size.x, size.z];   // creates maze array
        List<MazeCell> activeCells = new List<MazeCell>();  // creates list of active cells
        DoFirstGenerationStep(activeCells);
        while (activeCells.Count > 0)
        {
            yield return delay;
            DoNextGenerationStep(activeCells);
        }
    }

    // creates a cell at the specified coordinates and sets attributes
    private MazeCell CreateCell(IntVector2 coordinates)
    {
        MazeCell newCell = Instantiate(cellPrefab) as MazeCell;
        cells[coordinates.x, coordinates.z] = newCell;
        newCell.coordinates = coordinates;
        newCell.name = "Maze Cell " + coordinates.x + ", " + coordinates.z;
        newCell.transform.parent = transform;
        newCell.transform.localPosition =
            new Vector3(coordinates.x - size.x * 0.5f + 0.5f, 0f, coordinates.z - size.z * 0.5f + 0.5f);
        return newCell;
    }

    // first generation step (executes once)
    private void DoFirstGenerationStep(List<MazeCell> activeCells)
    {
        activeCells.Add(CreateCell(RandomCoordinates)); // adds the first cell at a random location
    }

    // all other generation steps
    private void DoNextGenerationStep(List<MazeCell> activeCells)
    {
        int currentIndex = activeCells.Count - 1;
        MazeCell currentCell = activeCells[currentIndex];
        if (currentCell.IsFullyInitialized)
        {
            activeCells.RemoveAt(currentIndex);
            return;
        }
        MazeDirection direction = currentCell.RandomUninitializedDirection;
        IntVector2 coordinates = currentCell.coordinates + direction.ToIntVector2();
        if (ContainsCoordinates(coordinates))   // if target cell is within the maze
        {
            MazeCell neighbor = GetCell(coordinates);
            if (neighbor == null)   // if no neighboring cell is created 
            {
                neighbor = CreateCell(coordinates); // creates cell
                CreatePassage(currentCell, neighbor, direction);    // creates passage
                activeCells.Add(neighbor);  // adds new cell to active cells
            }
            else {  // if creating a wall between two cells
                CreateWall(currentCell, neighbor, direction);
            }
        }
        else {  // if attempting to create wall on the maze border
            CreateWall(currentCell, null, direction);
        }
    }

    // creates a passage between the specified cells
    private void CreatePassage(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        MazePassage passage = Instantiate(passagePrefab) as MazePassage;
        passage.Initialize(cell, otherCell, direction);
        passage = Instantiate(passagePrefab) as MazePassage;
        passage.Initialize(otherCell, cell, direction.GetOpposite());
    }

    // creates a between the specified cells
    private void CreateWall(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        MazeWall wall = Instantiate(wallPrefab) as MazeWall;
        wall.Initialize(cell, otherCell, direction);
        if (otherCell != null)
        {
            wall = Instantiate(wallPrefab) as MazeWall;
            wall.Initialize(otherCell, cell, direction.GetOpposite());
        }
    }
}