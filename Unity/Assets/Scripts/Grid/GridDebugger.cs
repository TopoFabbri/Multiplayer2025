using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class GridDebugger : MonoBehaviour
{
    [Header("Grid Visualization")]
    [SerializeField] private Vector2Int drawSize = new(30, 30);

    [SerializeField] private Color gridColor = new(1f, 0.92f, 0.016f, 0.5f); // A nice semi-transparent yellow

    // We can get the grid component automatically.
    private Grid grid;

    private void Awake()
    {
        // Cache the Grid component for performance.
        grid = GetComponent<Grid>();
    }

    /// <summary>
    /// This Unity callback is used to draw gizmos in the Scene view.
    /// It only runs when the GameObject this script is attached to is selected.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // If the grid component isn't assigned yet (e.g., in edit mode), try to get it.
        if (grid == null)
        {
            grid = GetComponent<Grid>();
        }

        // Store the original gizmo color so we can restore it later.
        Color oldColor = Gizmos.color;
        Gizmos.color = gridColor;

        // Loop through a range of cells based on the drawSize.
        // We go from -size/2 to +size/2 to draw the grid around the object's origin.
        for (int x = 0; x < drawSize.x; x++)
        {
            for (int y = 0; y < drawSize.y; y++)
            {
                // The Grid component has a helper method to get the world position
                // of a cell's center. This correctly accounts for cell size, layout, and gaps.
                Vector3Int cellPosition = new(x, 0, y);
                Vector3 cellWorldCenter = grid.GetCellCenterWorld(cellPosition);
                Vector3 cellSize = new Vector3(grid.cellSize.x, 0, grid.cellSize.y) * 2f;

                // Draw a wireframe cube to represent the cell's boundaries.
                // The size of the cube is simply the grid's cell size.
                Gizmos.DrawWireCube(cellWorldCenter, cellSize);
            }
        }

        // It's good practice to restore the original gizmo color.
        Gizmos.color = oldColor;
    }
}