using System.Collections.Generic;

namespace Bumblebee
{
    public static class Variables
    {
        public static string Step { get; set; }

        public static string SourceJsonPath { get; set; }

        public static List<StateDiagramIdModel> StateDiagramIds { get; set; } = new List<StateDiagramIdModel>();

        public static string DestinationJsonPath { get; set; }
    }
}