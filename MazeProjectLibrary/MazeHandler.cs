using MazeGeneratorLib;
using MazeLib;
using MazeProjectLibrary.Common;
using SearchAlgorithmsLib;
using SearchAlgorithmsLib.Interfaces;
using SearchAlgorithmsLib.Searchers;
using Newtonsoft.Json.Linq;
using System.Text;

namespace MazeProjectLibrary
{
    public static class MazeHandler
    {
        public static JObject GenerateMaze(int rows, int cols)
        {
            var maze = new DFSMazeGenerator().Generate(rows, cols);
            var mazeJson = JObject.Parse(maze.ToJSON());
            mazeJson["Maze"] = maze.ToString().Replace("\r\n", string.Empty);
            var currentPos = new JObject();
            currentPos["Row"] = ((JObject)mazeJson["Start"])["Row"];
            currentPos["Col"] = ((JObject)mazeJson["Start"])["Col"];

            mazeJson["CurrentPos"] = currentPos;

            return mazeJson;
        }

        public static string SolveMaze(string maze, string algorithm)
        {
            var mazeJason = JObject.Parse(maze);
            var searchableMaze = new MazeAdapter(mazeJason);
            ISearcher<Position> searcher;

            if (algorithm.EqualsLower("bfs"))
            {
                searcher = new BFS<Position>(new CostComperator<Position>());
            }else
            {
                searcher = new DFS<Position>();
            }

            var solution = searcher.Search(searchableMaze);
            return BuildRelativeSolution(solution);
        }

        private static string BuildRelativeSolution(Solution<Position> solution)
        {
            var relativeSolutionBuilder = new StringBuilder();
            State<Position> currentState = solution.States.Pop();
            int directionSymbol; 

            while (solution.States.Count > 0)
            {
                var nextState = solution.States.Pop();
                switch (currentState.Value.Subtract(nextState.Value))
                {
                    case Direction.Down:
                        directionSymbol = 3;
                        break;
                    case Direction.Up:
                        directionSymbol = 2;
                        break;
                    case Direction.Left:
                        directionSymbol = 0;
                        break;
                    default:
                    case Direction.Right:
                        directionSymbol = 1;
                        break;
                }
                relativeSolutionBuilder.Append(directionSymbol);
                currentState = nextState;
            }

            return relativeSolutionBuilder.ToString();
        }
    }
}
