public class SiteAdditionalDataItem
{
    public string SiteId { get; set; }
    public int NumberOfDrives { get; set; }
    public long StorageUsedInDrives { get; set; }
    public bool SiteHasPreservationHold { get; set; }
    public long StorageUsedPreservationHold { get; set; }

    // Can't get this data from the Graph API
    public int NumberOfLists { get; set; }
    public int NumberOfItemsInLists { get; set; }
}
    