using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.PurgeQueueAdapters.Interface;
using TWNewEgg.PurgeService.Interface;
using TWNewEgg.StorageServices.Interface;
using System.Net;

namespace TWNewEgg.PurgeService
{
    public class PurgeService : IPurgeService
    {
        private IPurgeQueueAdapters _iPurgeQueueAdapters;
        private ICDNAdapter _cdnAdapter;
        public PurgeService(IPurgeQueueAdapters iPurgeQueueAdapters, ICDNAdapter cdnAdapter)
        {
            this._iPurgeQueueAdapters = iPurgeQueueAdapters;
            this._cdnAdapter = cdnAdapter;
        }


        public string AzurePurge(int purgeNumber = 50)
        {
            string result = string.Empty;
            List<TWNewEgg.Models.DBModels.TWSQLDB.PurgeQueue> _purgeQueue = new List<TWNewEgg.Models.DBModels.TWSQLDB.PurgeQueue>();
            TWNewEgg.Models.DBModels.TWSQLDB.PurgeRequest _purgeRequest = new TWNewEgg.Models.DBModels.TWSQLDB.PurgeRequest();
            bool hasData = true;
            try
            {
                _purgeQueue = this._iPurgeQueueAdapters.read().Where(p => p.isPurged == 0).Take(purgeNumber).ToList();
                _purgeRequest = this._iPurgeQueueAdapters.readPurgeRequest().OrderByDescending(x =>x.ID).Take(1).FirstOrDefault();
                //No Data will no purge
                if (_purgeQueue == null || _purgeQueue.Count == 0)
                {
                    hasData = false;
                }
                //has datas need to purge
                if (hasData == true)
                {
                    if (_purgeRequest == null || (_purgeRequest.Status == 1 && _purgeRequest.StatusCode == HttpStatusCode.OK.ToString()))
                    {
                        TWNewEgg.Models.DBModels.TWSQLDB.PurgeRequest newpurgeRequest = new TWNewEgg.Models.DBModels.TWSQLDB.PurgeRequest();
                        newpurgeRequest.TryTimes = 0;
                        newpurgeRequest.Status = 1;
                        newpurgeRequest.CDNEndPointName = "";
                        newpurgeRequest.CreateDate = DateTime.Now;
                        newpurgeRequest.UpdateDate = DateTime.Now;
                        this._iPurgeQueueAdapters.insertPurgeRequest(newpurgeRequest);

                        List<string> toPurgeList = new List<string>();
                        _purgeQueue.ForEach(p =>
                        {
                            toPurgeList.Add(p.URL);
                        });
                        //開始 purge
                        this._cdnAdapter.Purge(toPurgeList);
                    }   
                    else 
                    {
                            int ID=_purgeRequest.ID;
                            _purgeQueue = this._iPurgeQueueAdapters.read().Where(p => p.PurgeRequestID == ID).Take(purgeNumber).ToList();
                            List<string> toPurgeList = new List<string>();
                            _purgeQueue.ForEach(p =>
                            {
                                toPurgeList.Add(p.URL);
                            });
                            //開始 purge
                            this._cdnAdapter.Purge(toPurgeList);

                    }
                    
                    _purgeRequest = this._iPurgeQueueAdapters.readPurgeRequest().OrderByDescending(x => x.ID).Take(1).FirstOrDefault();
                    ////purge success and then update the purge status
                    if (_purgeRequest.StatusCode == HttpStatusCode.OK.ToString())
                    {
                        _purgeRequest.Status = 1;
                        this._iPurgeQueueAdapters.updatePurgeRequest(_purgeRequest);
                        this.purgeStstusChangeAndPurge(_purgeQueue, PurgeType.purgeSuccess, _purgeRequest.ID);
                    }
                    else
                    {
                        this.purgeStstusChangeAndPurge(_purgeQueue, PurgeType.purgeError, _purgeRequest.ID);
                    }
                    result = "All Success";
                }
                else
                {
                    result = "No data need to purge";
                }
            }
            catch (Exception ex)
            {
                //rollback purge status
                this.purgeStstusChangeAndPurge(_purgeQueue, PurgeType.purgeError, _purgeRequest.ID);
                result = ex.InnerException.ToString();
            }
            return result;
        }

        private void purgeStstusChangeAndPurge(List<TWNewEgg.Models.DBModels.TWSQLDB.PurgeQueue> model, PurgeType purgeStatus,int ID)
        {
            switch (purgeStatus)
            {
                case PurgeType.purgeSuccess:
                    {
                        model.ForEach(p =>
                        {
                            p.isPurged = (int)PurgeType.purgeSuccess;
                            p.PurgeRequestID = ID;
                        });
                        break;
                    }
                case PurgeType.purgeError:
                    {
                        model.ForEach(p =>
                        {
                            p.isPurged = (int)PurgeType.purgeError;
                            p.PurgeRequestID = ID;
                        });
                        break;
                    }
                default:
                    {
                        model.ForEach(p =>
                        {
                            p.isPurged = (int)PurgeType.purgeError;
                            p.PurgeRequestID = ID;
                        });
                        break;
                    }
            }
            this.updatePurgeTable(model);
        }
        private void updatePurgeTable(List<TWNewEgg.Models.DBModels.TWSQLDB.PurgeQueue> model)
        {
            this._iPurgeQueueAdapters.updateMany(model);
        }
    }
}
