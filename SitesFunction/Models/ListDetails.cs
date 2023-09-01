using System;

public class ListDetails
{
    public string SiteId { get; set; }
    public string ListId { get; set; }
    public string ListName { get; set; }
    public string ListUrl { get; set; }
    public string ListType { get; set; }
    public string ListTemplate { get; set; }
    public DateTime ListCreatedDate { get; set; }
    public DateTime ListLastItemUserModifiedDate { get; set; }
    public int ListItemCount { get; set; }
    public int ListMajorVersionCount { get; set; }
    public int ListMinorVersionCount { get; set; }
    public bool ListHasUniquePermissions { get; set; }
    public bool ListHasRetentionLabel { get; set; }
}