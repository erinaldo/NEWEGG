using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TWNewEgg.ECWeb_Mobile.Auth;
using TWNewEgg.ECWeb.PrivilegeFilters.Api;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.ViewModels.Track;
using TWNewEgg.Models.DomainModels.Track;
using System.Web.Http.Cors;

namespace TWNewEgg.ECWeb_Mobile.Controllers.Api
{
#if DEBUG
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowNonSecures]
    //[AllowAnonymous]
#endif
    //[AllowNonSecures]
    //[AllowAnonymous]
    public class TrackController : ApiController
    {
        public Dictionary<string, List<TrackItem>> Get(string ac = "", int? cid = null)
        {
            ResponsePacket<Dictionary<string, List<TrackItem>>> results = new ResponsePacket<Dictionary<string, List<TrackItem>>>();
            if (!NEUser.IsAuthticated)
            {
                return results.results;
            }
            if(string.IsNullOrEmpty(ac))
            {
                results = Processor.Request<Dictionary<string, List<TrackItem>>, Dictionary<string, List<TrackItem>>>("TrackService", "GetTracksStatus", NEUser.ID, cid);
                if (string.IsNullOrEmpty(results.error))
                {
                    return results.results;
                }
                return new Dictionary<string, List<TrackItem>>();
            }
            switch (ac)
            {
                case "all":
                    results = Processor.Request<Dictionary<string, List<TrackItem>>, Dictionary<string, List<TrackItem>>>("TrackService", "GetTracksDetial", NEUser.ID);
                    break;
                default:
                    results = Processor.Request<Dictionary<string, List<TrackItem>>, Dictionary<string, List<TrackItem>>>("TrackService", "GetTracksStatus", NEUser.ID, cid);
                    break;
            }
            if (string.IsNullOrEmpty(results.error))
            {
                return results.results;
            }
            return new Dictionary<string, List<TrackItem>>();
        }

        public List<string> Post([FromBody]List<CartTrack> tracks)
        {
            if (!NEUser.IsAuthticated)
            {
                return new List<string>();
            }
            var results = Processor.Request<List<string>, List<string>>("TrackService", "AddToTracks", NEUser.ID, ConvertToDomainModel(tracks));

            if (string.IsNullOrEmpty(results.error))
            {
                return results.results;
            }

            return new List<string>();
        }

        public List<string> Put([FromBody]List<CartTrack> tracks)
        {
            if (!NEUser.IsAuthticated)
            {
                return new List<string>();
            }
            var results = Processor.Request<List<string>, List<string>>("TrackService", "UpdateToTracks", NEUser.ID, ConvertToDomainModel(tracks));

            if (string.IsNullOrEmpty(results.error))
            {
                return results.results;
            }

            return new List<string>();
        }

        public List<string> Delete([FromBody]List<CartTrack> tracks)
        {
            if (!NEUser.IsAuthticated)
            {
                return new List<string>();
            }
            var results = Processor.Request<List<string>, List<string>>("TrackService", "DeleteFromTracks", NEUser.ID, ConvertToDomainModel(tracks));

            if (string.IsNullOrEmpty(results.error))
            {
                return results.results;
            }

            return new List<string>();
        }

        private List<TrackDM> ConvertToDomainModel(List<CartTrack> tracks)
        {
            List<TrackDM> results = new List<TrackDM>();
            if (tracks == null)
            {
                return null;
            }
            for (int i = 0; i < tracks.Count; i++)
            {
                TrackDM newTrack = new TrackDM();
                newTrack.ItemID = tracks[i].iid;
                newTrack.Status = tracks[i].stu;
                newTrack.Qty = (tracks[i].qty < 1) ? 1 : tracks[i].qty;
                newTrack.CategoryID = tracks[i].cid;
                newTrack.CategoryType = tracks[i].cty;
                results.Add(newTrack);
            }
            return results;
        }
    }
}
