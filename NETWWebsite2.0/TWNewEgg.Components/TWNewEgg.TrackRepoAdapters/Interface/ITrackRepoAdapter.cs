using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.TrackRepoAdapters.Interface
{
    public interface ITrackRepoAdapter
    {
        IQueryable<Track> GetAll();
        List<Track> AddTracks(List<Track> addTracks);
        Track UpdateTracks(Track updateTrack, bool isUpdateDate);
        IQueryable<Track> ReadTracks(int accountID, int? status, DateTime? beforeDate, DateTime? afterDate);
        void DeleteTracks(List<Track> deleteTracks);
    }
}
