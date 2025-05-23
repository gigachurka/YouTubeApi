using System.Collections.Generic;
using YouTubeApi.Models;

namespace YouTubeApi.ViewModels
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public List<VideoEntity> Videos { get; set; }
        public long TotalViews { get; set; }
        public long TotalLikes { get; set; }
        public long TotalComments { get; set; }
        public string ChannelTitle { get; set; }
        public double AvgViews { get; set; }
        public double AvgLikes { get; set; }
        public double EngagementRate { get; set; }
        public int VideoCount { get; set; }
        public DateTimeOffset? ChannelCreatedAt { get; set; }
        public string Badge { get; set; }
        public long SubscriberCount { get; set; }
    }
} 