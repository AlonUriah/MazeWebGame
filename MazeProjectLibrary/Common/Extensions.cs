using System;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using MazeLib;

namespace MazeProjectLibrary.Common
{
    public static class Extensions
    {
        public static bool EqualsLower(this string arg1, string arg2)
        {
            return (arg1.ToLower().Equals(arg2.ToLower()));
        }
        
        public static int CompareTo(this Position position, Position other)
        {
            return 1;
        }
    }
}
