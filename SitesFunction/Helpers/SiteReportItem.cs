using System;

public class SiteReportItem
{
    public DateTime ReportRefreshDate { get; set; }
    public Guid SiteId { get; set; }
    public string SiteURL { get; set; }
    public string OwnerDisplayName { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? LastActivityDate { get; set; }
    public int FileCount { get; set; }
    public int ActiveFileCount { get; set; }
    public int PageViewCount { get; set; }
    public int VisitedPageCount { get; set; }
    public long StorageUsedBytes { get; set; }
    public long StorageAllocatedBytes { get; set; }
    public string RootWebTemplate { get; set; }
    public string OwnerPrincipalName { get; set; }
    public int ReportPeriod { get; set; }
}
