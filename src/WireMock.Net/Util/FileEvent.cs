namespace WireMock.Util
{
    public enum ChangeType
    {
        CHANGED = 0,
        CREATED = 1,
        DELETED = 2,
        RENAMED = 3,
        LOG = 4
    }
    

    public class FileChangedEvent
    {
        /// <summary>
        /// Type of change event
        /// </summary>
        public ChangeType ChangeType { get; set; }

        /// <summary>
        /// The full path
        /// </summary>
        public string FullPath { get; set; } = "";

        /// <summary>
        /// The old full path (used if ChangeType = RENAMED)
        /// </summary>
        public string OldFullPath { get; set; } = "";
    }
    
}
