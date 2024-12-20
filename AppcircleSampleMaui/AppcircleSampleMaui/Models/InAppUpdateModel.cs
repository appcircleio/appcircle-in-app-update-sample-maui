namespace AppcircleSampleMaui.Models;

public class InAppUpdateModel
{
    public class UpdateResult
    {
        public string DownloadUrl { get; set; }
        public string Version { get; set; }
    }
    public class TokenResponse
    {
        public string access_token { get; set; }
    }
    public class AppVersion
    {
        public string Id { get; set; }
        public string ProfileId { get; set; }
        public string AppResourceReferenceId { get; set; }
        public string AppIconResourceReferenceId { get; set; }
        public string Name { get; set; }
        public string UniqueName { get; set; }
        public string SignedCertName { get; set; }
        public string Version { get; set; }
        public string VersionCode { get; set; }
        public int PublishType { get; set; }
        public DateTime? PublishDate { get; set; }
        public string PublishDateStr { get; set; }
        public int PlatformType { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public int DownloadCount { get; set; }
        public string Summary { get; set; }
        public string ReleaseNotes { get; set; }
        public DateTime? LatestNotificationDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string BuildId { get; set; }
        public string AppIconUrl { get; set; }
        public bool IsDownloadLimitExceeded { get; set; }
        public int OrganizationDownloadCount { get; set; }
        public int OrganizationDownloadLimit { get; set; }
    }
    public class AppVersionsResponse
    {
        public List<AppVersion> data { get; set; }
    }

    public enum PublishType
    {
        NotPublished = 0,
        Beta,
        Live,
    }
}