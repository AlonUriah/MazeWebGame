using System.Collections.Generic;
using MazeLib;
using SearchAlgorithmsLib;
using SearchAlgorithmsLib.Interfaces;
using Newtonsoft.Json.Linq;

namespace MazeProjectLibrary.Common
{
    /// <summary>
    /// Adapter for Maze.
    /// This MazeAdapter is Searchable and Drawable.
    /// </summary>
    public class MazeAdapter : ISearchable<Position>
    {
        private const char WALL_SIGN = '1';
        private string _mazeStr;
        private readonly MazeCells _cells = new MazeCells();

        public int Cols { set; get; }
        public int Rows { set; get; }
        public Position InitialPosition { set; get; }
        public Position GoalPosition { set; get; }
        public string MazeString {
            set
            {
                _mazeStr = value;
                int index = 0;

                for(int i = 0; i<Rows; i++)
                {
                    for(int j=0; j<Cols; j++)
                    {
                        index = (i * Cols) + j;
                        _cells[i, j] = (value[index] == WALL_SIGN) ? CellType.Wall : CellType.Free;
                    }
                }
            }
            get
            {
                return _mazeStr;
            }
        }

        /// <summary>
        /// Ctor.
        /// Initiate _maze class member.
        /// </summary>
        /// <param name="maze">A maze to adapt to an ISearchable object</param>
        public MazeAdapter(JObject mazeJson)
        {
            Cols = mazeJson["Rows"].Value<int>();
            Rows = mazeJson["Cols"].Value<int>();
            var initPos = (JObject)mazeJson["Start"];
            var goalPos = (JObject)mazeJson["End"];

            InitialPosition = new Position(initPos["Row"].Value<int>(), initPos["Col"].Value<int>());
            GoalPosition = new Position(goalPos["Row"].Value<int>(), goalPos["Col"].Value<int>());
            MazeString = mazeJson["Maze"].Value<string>();
        }

        /// <summary>
        /// Try to get a free neihbor in a relative location
        /// from currentPos.
        /// </summary>
        /// <param name="relativeLocation">Direction to look at, relative to currentLocation</param>
        /// <param name="currentPos">Current Position in Maze</param>
        /// <param name="neighbor">Out Position if cell type is Free</param>
        /// <returns>Returns true if a neighnor in relativeLocation is free</returns>
        private bool TryGetFreeNeighbor(Direction relativeLocation, Position currentPos, out Position neighbor)
        {
            int neighborCol = currentPos.Col, neighborRow = currentPos.Row;

            switch (relativeLocation)
            {
                case Direction.Left:
                    neighborCol--;
                    break;
                case Direction.Right:
                    neighborCol++;
                    break;
                case Direction.Down:
                    neighborRow--;
                    break;
                case Direction.Up:
                default:
                    neighborRow++;
                    break;
            }

            neighbor = new Position(neighborRow, neighborCol);

            if (neighborCol < 0 || neighborRow < 0 || 
                neighborCol == Cols || neighborRow == Rows ||
                _cells[neighborRow,neighborCol]==CellType.Wall)
            {
                return false;
            }

            return true;
        }

        #region ISearchable interface implementation
        /// <summary>
        /// Returns all available moves from current state.
        /// </summary>
        /// <param name="state">Current state to look from</param>
        /// <returns>A list of states containing all free neighbors</returns>
        public List<State<Position>> GetAllPossibleStates(State<Position> state)
        {
            var neighbors = new List<State<Position>>();
            var relativeDirections = new List<Direction>{Direction.Up,Direction.Right,Direction.Down,Direction.Left};
            
            Position currentPosition = state.Value;            
            var neighbor = new Position();

            foreach (var direction in relativeDirections)
            {
                if(TryGetFreeNeighbor(direction,currentPosition,out neighbor))
                {
                    neighbors.Add(new State<Position>(neighbor));
                }
            }

            return neighbors;
        }

        /// <summary>
        /// Returns _maze Goal State
        /// </summary>
        /// <returns>Goal state of _maze</returns>
        public State<Position> GetGoalState()
        {
            var goalPosition = new Position(GoalPosition.Row, GoalPosition.Col);
            return new State<Position>(goalPosition);
        }

        /// <summary>
        /// Returns _maze Initial State
        /// </summary>
        /// <returns>Initial state of _maze</returns>
        public State<Position> GetInitialState()
        {
            var initialPosition = new Position(InitialPosition.Row, InitialPosition.Col);
            return new State<Position>(initialPosition);
        }
        #endregion
    }

    internal class MazeCells
    {
        private Position _positionPtr = new Position();
        private Dictionary<Position, CellType> _positionsToCellTypes = new Dictionary<Position, CellType>();

        public CellType this[int row,int col]
        {
            set
            {
                _positionPtr.Row = row;
                _positionPtr.Col = col; 
                _positionsToCellTypes[_positionPtr] = value;
            }
            get
            {
                _positionPtr.Row = row;
                _positionPtr.Col = col;
                return _positionsToCellTypes[_positionPtr];
            }
        }
    }
}
