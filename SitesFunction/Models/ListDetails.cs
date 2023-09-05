using System;

namespace groveale
{
    public class ListDetails
    {
        public string SiteId { get; set; }
        public string ListId { get; set; }
        public string DriveId { get; set; }
        public string ListName { get; set; }
        public string ListUrl { get; set; }
        public string ListType { get; set; }
        public string ListTemplate { get; set; }
        public DateTimeOffset ListCreatedDate { get; set; }
        public DateTimeOffset ListLastItemModifiedDate { get; set; }
        public int ListItemCount { get; set; }
        public long ListSizeUsed { get; set; }
        // List items that are in the recycle bin 
        public long ListDeletedItemsSize { get; set; }
        public int ListMajorVersionCount { get; set; }
        public int ListMinorVersionCount { get; set; }
        public bool ListHasUniquePermissions { get; set; }
        public bool ListHasRetentionLabel { get; set; }
        public bool IsIndexed { get; set; }
    }
}