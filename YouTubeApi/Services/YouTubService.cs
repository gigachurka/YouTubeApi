//using Google.Apis.Services;
//using Google.Apis.YouTube.v3;
//using YouTubeApi.Data;
//using YouTubeApi.Models;
//using YouTubeApi.Services;

//public class YouTubService : InterfaceServ
//{
//    private readonly ApplicationDbContext _context;
//    private const string ApiKey = "AIzaSyCAc0C3r_XNElzvji9CnFhpzcGm7rhMCkg"; // перенеси в appsettings.json для безопасности

//    public YouTubService(ApplicationDbContext context)
//    {
//        _context = context;
//    }

//    public async Task<List<VideoEntity>> GetVideosFromChannelAsync(string channelId)
//    {
//        var youtubeService = new YouTubeService(new BaseClientService.Initializer
//        {
//            ApiKey = ApiKey,
//            ApplicationName = "MyYouTubeApp"
//        });

//        var searchRequest = youtubeService.Search.List("snippet");
//        searchRequest.ChannelId = channelId;
//        searchRequest.Order = SearchResource.ListRequest.OrderEnum.Date;
//        searchRequest.MaxResults = 10;

//        var searchResponse = await searchRequest.ExecuteAsync();

//        var videoList = new List<VideoEntity>();

//        foreach (var item in searchResponse.Items)
//        {
//            if (item.Id.Kind != "youtube#video") continue;

//            var video = new VideoEntity
//            {
//                Title = item.Snippet.Title,
//                Link = $"https://www.youtube.com/watch?v={item.Id.VideoId}",
//                Thumbnail = item.Snippet.Thumbnails.Medium.Url,
//                PublishedAt = item.Snippet.PublishedAtDateTimeOffset
//            };

//            var existingVideo = _context.Videos.FirstOrDefault(v => v.Link == video.Link);
//            if (existingVideo == null)
//            {
//                _context.Videos.Add(video);
//                videoList.Add(video);
//            }
//            else
//            {
//                // Обновляем данные
//                existingVideo.Title = video.Title;
//                existingVideo.Thumbnail = video.Thumbnail;
//                existingVideo.PublishedAt = video.PublishedAt;
//            }

//        }

//        await _context.SaveChangesAsync();
//        return videoList;
//    }
//}
