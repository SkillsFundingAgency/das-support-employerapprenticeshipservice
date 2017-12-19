namespace SFA.DAS.Support.Shared
{
    public class SearchColumnDefinition
    {
        public enum SortOrder
        {
            Asc,
            Desc
        }
        public string Name { get; set; }
        public LinkDefinition Link { get; set; }

        public SortOrder? Sort { get; set; }

        public bool HideColumn { get; set; }

        public string DisplayName { get; set; }
    }
}
