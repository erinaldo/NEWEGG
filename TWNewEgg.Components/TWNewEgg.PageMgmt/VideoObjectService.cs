using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.PageMgmt.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels.PageMgmt;

namespace TWNewEgg.PageMgmt
{
    // Video Component直接指向VideoMgmt，Create、Update與Delete都會直接對Component操作，唯有Update、Create新的影片時會新增Video。
    public class VideoObjectService: IObjectService<VideoInfo>
    {
        private IRepository<VideoInfo> _video;
        private IRepository<ComponentInfo> _component;
        public VideoObjectService(IRepository<VideoInfo> video, IRepository<ComponentInfo> component)
        {
            this._video = video;
            this._component = component;
        }

        public List<VideoInfo> GetByComponents(List<ComponentInfo> components)
        {
            List<int> videoIDs = components.Where(x => x.ObjectType == "Video").Select(x => x.ObjectID).ToList();
            List<VideoInfo> videos = _video.GetAll().Where(x => videoIDs.Contains(x.VideoID)).ToList();
            return videos;
        }

        public void saveEditObject(DSComponentInfo component) 
        {
            VideoInfo video = component.Video;
            VideoInfo origin = _video.Get(x => x.VideoID == video.VideoID);

            // Update Object時，若是新增的Object則與相同VideoID的原始Object應為不同影片
            if (video != null && origin != null && video.ProviderVideoID != origin.ProviderVideoID) 
            {
                Create(video);
                component.ObjectID = video.VideoID;
            }
            origin.Title = video.Title;
            origin.Description = video.Description;
            _video.Update(origin);
        }

        public void saveNewObject(DSComponentInfo component)
        {
            VideoInfo video = component.Video;
            VideoInfo origin = _video.Get(x => x.VideoID == video.VideoID);

            //origin與video不同影片，代表video不是從既有影片選出的，是新創的
            if (video != null && (origin == null || origin.ProviderVideoID != video.ProviderVideoID)) {
                video.PublishedAt = DateTime.Now;
                Create(video);
                component.ObjectID = video.VideoID;
            }
        }

        public void saveDeleteObject(DSComponentInfo component)
        {
            // no need
        }

        private bool Create(VideoInfo video)
        {
            _video.Create(video);
            return true;
        }

    }
}
