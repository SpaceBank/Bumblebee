namespace Bumblebee
{
    public static class Variables
    {
        public static string Step { get; set; }

        public static string SourceJsonPath { get; set; }

        public static string SourceStateDiagramId { get; set; }

        public static string DestinationJsonPath { get; set; }

        public static string DestinationStateDiagramId { get; set; }

        public static string NewParameterSource { get; set; }
        
        public static string NewParameterName { get; set; }

        public static bool NewParameterSendToApis { get; set; }

        public static string NewParameterSendToApiDestination { get; set; }
    }
}