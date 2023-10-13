using System.Collections.Generic;
using groveale;

public class SiteAdditionalDataItem
{
    public string SiteId { get; set; }
    public int NumberOfDrives { get; set; }
    public long StorageUsedInDrives { get; set; }
    public long StoragePreviousVersionsInDrives { get; set; }
    public bool SiteHasPreservationHold { get; set; }
    public long StorageUsedPreservationHold { get; set; }
    public long RecycleBinSize { get; set; }
    public bool IsOrphaned { get; set; }
    public bool IsHomeSite { get; set; }
    public bool IsAccessLocked { get; set; }

    public bool IsDeleted { get; set; }
    public int NumberOfItemsInSite { get; set; }

    public List<ListDetails> Lists { get; set; }
}
    