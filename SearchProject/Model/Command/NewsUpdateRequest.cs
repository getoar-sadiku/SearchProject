using System;

namespace SearchProject.Model.Command
{
    public class NewsUpdateRequest
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string LinkTitle { get; set; }
        public string SubTitle { get; set; }
        public string ImgUrl { get; set; }
        public DateTime DateTime { get; set; }
        public string DataPerAfishim { get; set; }
        public string NewUrl { get; set; }
        public string NewUrlHost { get; set; }
        public string GroupCode { get; set; }
        public string PortalName { get; set; }
        public object fullArticle { get; set; }
        public int NCategory { get; set; }
        public int GlobalRank { get; set; }
        public int AlbanianRank { get; set; }
        public int GjirafaRank { get; set; }
        public int TotalResults { get; set; }
        public int Country { get; set; }
        public double termRank { get; set; }
        public object Image { get; set; }
        public bool isSaved { get; set; }
        public bool isPartnerPortals { get; set; }
        public bool isVideo { get; set; }
        public string urlHash { get; set; }
        public DateTime crawlTime { get; set; }
        public string selectedCategories { get; set; }
        public string Comments { get; set; }
    }
}
