using MazeGeneratorLib;
using MazeLib;
using MazeProjectLibrary.Common;
using SearchAlgorithmsLib.Interfaces;
using SearchAlgorithmsLib.Searchers;
using Newtonsoft.Json.Linq;

namespace MazeProjectLibrary
{
    public static class MazeHandler
    {
        public static string GenerateMaze(int rows, int cols)
        {
            var maze = new DFSMazeGenerator().Generate(rows, cols);
            return maze.ToJSON();
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

            return searcher.Search(searchableMaze).ToJason().ToString();
        }

    }
}
