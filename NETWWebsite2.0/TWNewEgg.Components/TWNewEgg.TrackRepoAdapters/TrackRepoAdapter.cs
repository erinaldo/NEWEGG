using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.TrackRepoAdapters.Interface;

namespace TWNewEgg.TrackRepoAdapters
{
    public class TrackRepoAdapter : ITrackRepoAdapter
    {
        private IRepository<Track> _trackDB;

        public TrackRepoAdapter(IRepository<Track> track)
        {
            _trackDB = track;
        }

        public IQueryable<Track> GetAll()
        {
            return this._trackDB.GetAll();
        }

        public List<Track> AddTracks(List<Track> addTracks)
        {
            List<Track> addResults = new List<Track>();
            if (addTracks == null)
            {
                return addResults;
            }
            for (int i = 0; i < addTracks.Count; i++)
            {
                addTracks[i].CreateDate = DateTime.UtcNow.AddHours(8);
                _trackDB.Create(addTracks[i]);
                addResults.Add(addTracks[i]);
            }
            return addResults;
        }

        public Track UpdateTracks(Track updateTrack, bool isUpdateDate)
        {
            //List<Track> updateResults = new List<Track>();
            if (updateTrack == null)
            {
                return null;
            }

            if (isUpdateDate)
            {
                updateTrack.CreateDate = DateTime.UtcNow.AddHours(8);
            }
            _trackDB.Update(updateTrack);
            return updateTrack;
        }

        public IQueryable<Track> ReadTracks(int accountID, int? status, DateTime? beforeDate, DateTime? afterDate)
        {
            IQueryable<Track> currentTrack = _trackDB.GetAll().Where(x => x.ACCID == accountID);
            if (status != null)
            {
                currentTrack = currentTrack.Where(x => x.Status == status.Value);
            }
            if (beforeDate != null)
            {
                currentTrack = currentTrack.Where(x => x.CreateDate < beforeDate);
            }
            if (afterDate != null)
            {
                currentTrack = currentTrack.Where(x => x.CreateDate >= afterDate);
            }
            return currentTrack;
        }

        public void DeleteTracks(List<Track> deleteTracks)
        {
            if (deleteTracks == null)
            {
                return ;
            }
            for (int i = 0; i < deleteTracks.Count; i++)
            {
                _trackDB.Delete(deleteTracks[i]);
            }
        }
    }
}
