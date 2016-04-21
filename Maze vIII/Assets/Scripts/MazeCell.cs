using UnityEngine;

// defines a maze cell and all of its attributes
public class MazeCell : MonoBehaviour {

    // attributes
    
	public IntVector2 coordinates; // where the cell is located
    public int virus; // virus type of the cell
    private MazeCellEdge[] edges = new MazeCellEdge[MazeDirections.Count]; // edges
    private MazeCell[] neighbors = new MazeCell[MazeDirections.Count]; // neighboring cells, independent of edges (only used in recursive division)
    private int neighborCount;
    private int initializedEdgeCount; // number of initialized edges

    // gets the edge in the specified direction
    public MazeCellEdge GetEdge(MazeDirection direction)
    {
        return edges[(int)direction];
    }

    // checks to see whether all directions are initialized 
    public bool IsFullyInitialized
    {
        get
        {
            return initializedEdgeCount == MazeDirections.Count;
        }
    }

    // adds a neighbor
    public void AddNeighbor(MazeCell neigh)
    {
        neighbors[neighborCount] = neigh;
        neighborCount += 1;
    }

    // creates an edge in the specified direction
    public void SetEdge(MazeDirection direction, MazeCellEdge edge)
    {
        edges[(int)direction] = edge;
        initializedEdgeCount += 1;
    }

    // chooses a random direction that has not already been given a feature (edge, passage)
    public MazeDirection RandomUninitializedDirection
    {
        get
        {
            int skips = Random.Range(0, MazeDirections.Count - initializedEdgeCount);
            for (int i = 0; i < MazeDirections.Count; i++)
            {
                if (edges[i] == null)
                {
                    if (skips == 0)
                    {
                        return (MazeDirection)i;
                    }
                    skips -= 1;
                }
            }
            throw new System.InvalidOperationException("MazeCell has no uninitialized directions left.");
        }
    }

    
}