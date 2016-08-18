using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Track;

namespace TWNewEgg.TrackServices.Interface
{
    public interface ITrackService
    {
        bool CleanOldAndUpdateTracks(int accountID, DateTime beforeDate);
        List<string> AddToTracks(int accountID, List<TrackDM> newTracks);
        List<string> UpdateToTracks(int accountID, List<TrackDM> updateTracks);
        List<string> DeleteFromTracks(int accountID, List<TrackDM> deleteTracks);
        Dictionary<string, List<TrackItem>> GetTracksStatus(int accountID, int? categoryID = null);
        Dictionary<string, List<TrackItem>> GetTracksDetial(int accountID);
    }
}
