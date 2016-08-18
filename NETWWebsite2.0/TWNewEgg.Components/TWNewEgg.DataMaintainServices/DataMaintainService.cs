using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.DataMaintainServices.Interface;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DomainModels.DataMaintain;

namespace TWNewEgg.DataMaintainServices
{
    public class DataMaintainService : IDataMaintainService
    {
        private ICartRepoAdapter _cartRepoAdapter;
        private IRetgoodRepoAdapter _retgoodRepoAdapter;
        private IRefund2cRepoAdapter _refund2cRepoAdapter;
        private IDatamaintain_logRepoAdapter _Datamaintain_logRepoAdapter;

        public DataMaintainService(ICartRepoAdapter cartRepoAdapter, IRetgoodRepoAdapter retgoodRepoAdapter, IRefund2cRepoAdapter refund2cRepoAdapter , IDatamaintain_logRepoAdapter Datamaintain_logRepoAdapter)
        {
            this._cartRepoAdapter = cartRepoAdapter;
            this._retgoodRepoAdapter = retgoodRepoAdapter;
            this._refund2cRepoAdapter = refund2cRepoAdapter;
            this._Datamaintain_logRepoAdapter = Datamaintain_logRepoAdapter;
        }

        public CartDataMaintain_DM GetCartDataMaintain(DataMaintainSearchCondition_DM dataMaintainSearchCondition_DM)
        {
            CartDataMaintain_DM cartDataMaintain = new CartDataMaintain_DM();
            try
            {
                if (dataMaintainSearchCondition_DM != null)
                {
                    Cart cart = new Cart();
                    cart = _cartRepoAdapter.GetCart(dataMaintainSearchCondition_DM);
                    if (cart != null && cart.ID != "")
                    {
                        cartDataMaintain = ModelConverter.ConvertTo<CartDataMaintain_DM>(cart);
                    }

                    IQueryable<Process> querySearch = null;
                    Process objDbSearch = null;

                    querySearch = this._cartRepoAdapter.GetProcessByCartID(cartDataMaintain.ID);
                    objDbSearch = querySearch.FirstOrDefault();
                    int position = objDbSearch.OrderNote.IndexOf("<");
                    objDbSearch.OrderNote = objDbSearch.OrderNote.Substring(0, position);
                    if (objDbSearch != null)
                    {
                        cartDataMaintain.OrderNote = objDbSearch.OrderNote;   
                    }

                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return cartDataMaintain;
        }

        public CartDataMaintain_DM UpdateCart(CartDataMaintain_DM cartDataMaintain_DM)
        {
            CartDataMaintain_DM cartDataMaintain = new CartDataMaintain_DM();
            try
            {
                if (cartDataMaintain_DM == null)
                {
                    throw new NullReferenceException("CartDataMaintain_DM can't be null.");
                }

                Cart cart = new Cart();
                cart = _cartRepoAdapter.GetCart(cartDataMaintain_DM.ID);

                //子單配達時間
                IQueryable<Process> process = null;
                process = _cartRepoAdapter.GetProcessByCartID(cartDataMaintain_DM.ID);
                
                #region --多個LBS 更改--
                List<Process> processgroup = new List<Process>();
                processgroup = process.ToList();

                foreach (var processnote in processgroup)
                {
                    processnote.OrderNote = cartDataMaintain_DM.OrderNote + "</arrive>";
                }
                #endregion
               


                if (cart == null || cart.ID == "")
                {
                    throw new NullReferenceException("Cart can't find.");
                }

                cart.Receiver = cartDataMaintain_DM.Receiver;
                cart.Zipcode = cartDataMaintain_DM.Zipcode;
                cart.Location = cartDataMaintain_DM.Location;
                cart.ADDR = cartDataMaintain_DM.ADDR;
                cart.TelDay = cartDataMaintain_DM.TelDay;
                cart.TelNight = cartDataMaintain_DM.TelNight;
                cart.RecvMobile = cartDataMaintain_DM.RecvMobile;
                cart.RecvENGName = cartDataMaintain_DM.RecvENGName;
                cart.DelivENGADDR = cartDataMaintain_DM.DelivENGADDR;
                cart.Username = cartDataMaintain_DM.Username;
                cart.Phone = cartDataMaintain_DM.Phone;
                cart.Phone2 = cartDataMaintain_DM.Phone2;
                cart.Mobile = cartDataMaintain_DM.Mobile;
                cart.Email = cartDataMaintain_DM.Email;
                cart.CardPhone = cartDataMaintain_DM.CardPhone;
                cart.CardPhone2 = cartDataMaintain_DM.CardPhone2;
                cart.CardZipcode = cartDataMaintain_DM.CardZipcode;
                cart.CardLocation = cartDataMaintain_DM.CardLocation;
                cart.CardADDR = cartDataMaintain_DM.CardADDR;
                cart.CardMobile = cartDataMaintain_DM.CardMobile;
                cart.IinvoiceTitle = cartDataMaintain_DM.IinvoiceTitle;
                cart.ActCode = cartDataMaintain_DM.ActCode;
                cart.InvoZipcode = cartDataMaintain_DM.InvoZipcode;
                cart.InvoLocation = cartDataMaintain_DM.InvoLocation;
                cart.InvoADDR = cartDataMaintain_DM.InvoADDR;
                cart.InvoReceiver = cartDataMaintain_DM.InvoReceiver;
                cart.UsrLOC = cartDataMaintain_DM.UsrLOC;
                cart.UsrADDR = cartDataMaintain_DM.UsrADDR;
                cart.UserZipcode = cartDataMaintain_DM.UserZipcode;
                cart.Updated = cart.Updated + 1;
                cart.UpdateUser = cartDataMaintain_DM.UpdateUser;
                cart.UpdateDate = DateTime.UtcNow.AddHours(8);
                //datamaintain_log 修改紀錄
                Datamaintain_log Datamaintain_log = new Datamaintain_log();
                Datamaintain_log.Code = cartDataMaintain_DM.ID;
                Datamaintain_log.UpdateUser = cartDataMaintain_DM.UpdateUser;
                Datamaintain_log.UpdateDate = DateTime.Now;
                Datamaintain_log.UpdateNote = cartDataMaintain_DM.UpdateNote;
                Datamaintain_log = _Datamaintain_logRepoAdapter.Create(Datamaintain_log);
                // 更新欄位
                cart = _cartRepoAdapter.Update(cart);
                cartDataMaintain = ModelConverter.ConvertTo<CartDataMaintain_DM>(cart);

            }
            catch (Exception ex)
            {
                throw new NullReferenceException(ex.Message);
            }

            return cartDataMaintain;
        }

        public RetgoodDataMaintain_DM GetretgoodDataMaintain(DataMaintainSearchCondition_DM dataMaintainSearchCondition_DM)
        {
            RetgoodDataMaintain_DM retgoodDataMaintain = new RetgoodDataMaintain_DM();
            try
            {
                if (dataMaintainSearchCondition_DM != null)
                {
                    Retgood retgood = new Retgood();
                    retgood = _retgoodRepoAdapter.GetRetgood(dataMaintainSearchCondition_DM);
                    if (retgood != null && retgood.ID != 0)
                    {
                        retgoodDataMaintain = ModelConverter.ConvertTo<RetgoodDataMaintain_DM>(retgood);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return retgoodDataMaintain;
        }

        public RetgoodDataMaintain_DM Updateretgood(RetgoodDataMaintain_DM RetgoodDataMaintain_DM)
        {
            RetgoodDataMaintain_DM retgoodDataMaintain = new RetgoodDataMaintain_DM();
            try
            {
                if (RetgoodDataMaintain_DM == null)
                {
                    throw new NullReferenceException("RetgoodDataMaintain_DM can't be null.");
                }

                Retgood retgood = new Retgood();
                retgood = _retgoodRepoAdapter.GetRetgood(RetgoodDataMaintain_DM.Code);

                if (retgood == null || retgood.ID == 0)
                {
                    throw new NullReferenceException("retgood can't find.");
                }

                retgood.Price = RetgoodDataMaintain_DM.Price;
                retgood.Qty = RetgoodDataMaintain_DM.Qty ?? retgood.Qty;
                retgood.BankName = RetgoodDataMaintain_DM.BankName;
                retgood.BankBranch = RetgoodDataMaintain_DM.BankBranch;
                retgood.AccountNO = RetgoodDataMaintain_DM.AccountNO;
                retgood.AccountName = RetgoodDataMaintain_DM.AccountName;
                retgood.FrmName = RetgoodDataMaintain_DM.FrmName;
                retgood.FrmLocation = RetgoodDataMaintain_DM.FrmLocation;
                retgood.FrmZipcode = RetgoodDataMaintain_DM.FrmZipcode;
                retgood.FrmADDR = RetgoodDataMaintain_DM.FrmADDR;
                retgood.FrmPhone = RetgoodDataMaintain_DM.FrmPhone;
                retgood.FrmPhone2 = RetgoodDataMaintain_DM.FrmPhone2;
                retgood.FrmMobile = RetgoodDataMaintain_DM.FrmMobile;
                retgood.FrmEmail = RetgoodDataMaintain_DM.FrmEmail;
                retgood.FrmTime = RetgoodDataMaintain_DM.FrmTime;
                retgood.Updated = retgood.Updated + 1;
                retgood.UpdatedDate = DateTime.UtcNow.AddHours(8);
                retgood.UpdatedUser = RetgoodDataMaintain_DM.UpdatedUser;
                retgood.CauseNote = RetgoodDataMaintain_DM.CauseNote;
                retgood.Cause = RetgoodDataMaintain_DM.Cause;
                
                //退貨退款銀行帳號同步
                if (RetgoodDataMaintain_DM.ProcessID != null)
                {
                    refund2c refund2c = new refund2c();
                    //用子單號碼去找退款單
                    refund2c = _refund2cRepoAdapter.GetRefund2cbyProcessID(RetgoodDataMaintain_DM.ProcessID);
                    if (refund2c != null && refund2c.ID != 0)
                    {
                        var BankIdName = RetgoodDataMaintain_DM.BankName;
                        if (BankIdName != null )
                        {
                            string BankId = BankIdName.Substring(0, 3);
                            string BankName = BankIdName.Substring(3, BankIdName.Length - 3);

                        refund2c.BankID = BankId;
                        refund2c.BankName = BankName;
                        }
                        refund2c.SubBankName = RetgoodDataMaintain_DM.BankBranch;
                        refund2c.AccountNO = RetgoodDataMaintain_DM.AccountNO;
                        refund2c.AccountName = RetgoodDataMaintain_DM.AccountName;
                        refund2c = _refund2cRepoAdapter.Update(refund2c);
                    }
                }
                //datamaintain_log 修改紀錄
                Datamaintain_log Datamaintain_log = new Datamaintain_log();
                Datamaintain_log.Code = RetgoodDataMaintain_DM.Code;
                Datamaintain_log.UpdateUser = RetgoodDataMaintain_DM.UpdatedUser;
                Datamaintain_log.UpdateDate = DateTime.Now;
                Datamaintain_log.UpdateNote = RetgoodDataMaintain_DM.UpdateNote;
                Datamaintain_log = _Datamaintain_logRepoAdapter.Create(Datamaintain_log);

                retgood = _retgoodRepoAdapter.Update(retgood);
                retgoodDataMaintain = ModelConverter.ConvertTo<RetgoodDataMaintain_DM>(retgood);

            }
            catch (Exception ex)
            {
                throw new NullReferenceException(ex.Message);
            }

            return retgoodDataMaintain;
        }

        public refund2cDataMaintain_DM Getrefund2cDataMaintain(DataMaintainSearchCondition_DM dataMaintainSearchCondition_DM)
        {
            refund2cDataMaintain_DM refund2cDataMaintain = new refund2cDataMaintain_DM();
            try
            {
                if (dataMaintainSearchCondition_DM != null)
                {
                    refund2c refund2c = new refund2c();

                    refund2c = _refund2cRepoAdapter.GetRefund2c(dataMaintainSearchCondition_DM);

                    if (refund2c != null && refund2c.ID != 0)
                    {
                        refund2cDataMaintain = ModelConverter.ConvertTo<refund2cDataMaintain_DM>(refund2c);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new NullReferenceException(ex.Message);
            }

            return refund2cDataMaintain;
        }

        public refund2cDataMaintain_DM Updaterefund2c(refund2cDataMaintain_DM refund2cDataMaintain_DM)
        {
            refund2cDataMaintain_DM refund2cDataMaintain = new refund2cDataMaintain_DM();
            try
            {
                if (refund2cDataMaintain_DM != null)
                {
                    refund2c refund2c = new refund2c();
                    refund2c = _refund2cRepoAdapter.GetRefund2c(refund2cDataMaintain_DM.Code);
                    if (refund2c != null && refund2c.ID != 0)
                    {
                        refund2c.Amount = refund2cDataMaintain_DM.Amount;
                        refund2c.BankID = refund2cDataMaintain_DM.BankID;
                        refund2c.BankName = refund2cDataMaintain_DM.BankName;
                        refund2c.SubBankName = refund2cDataMaintain_DM.SubBankName;
                        refund2c.AccountNO = refund2cDataMaintain_DM.AccountNO;
                        refund2c.AccountName = refund2cDataMaintain_DM.AccountName;
                        refund2c.Updated = refund2c.Updated + 1;
                        refund2c.UpdateDate = DateTime.UtcNow.AddHours(8);
                        refund2c.UpdateUser = refund2cDataMaintain_DM.UpdateUser;
                        //datamaintain_log 修改紀錄
                        Datamaintain_log Datamaintain_log = new Datamaintain_log();
                        Datamaintain_log.Code = refund2cDataMaintain_DM.Code;
                        Datamaintain_log.UpdateUser = refund2cDataMaintain_DM.UpdateUser;
                        Datamaintain_log.UpdateDate = DateTime.Now;
                        Datamaintain_log.UpdateNote = refund2cDataMaintain_DM.UpdateNote;
                        Datamaintain_log = _Datamaintain_logRepoAdapter.Create(Datamaintain_log);

                        refund2c = _refund2cRepoAdapter.Update(refund2c);
                        refund2cDataMaintain = ModelConverter.ConvertTo<refund2cDataMaintain_DM>(refund2c);
                    }
                    //退貨退款銀行帳號同步
                    if (refund2cDataMaintain_DM.ProcessID != null)
                    {
                        Retgood retgood = new Retgood();
                        //用子單號碼去找退貨單
                        retgood = _retgoodRepoAdapter.GetRetgoodbyProcessID(refund2cDataMaintain_DM.ProcessID);
                        if (retgood != null && retgood.ID != 0)
                        {
                            var BankIdName = refund2cDataMaintain_DM.BankID + refund2cDataMaintain_DM.BankName;
                            retgood.BankName = BankIdName;//refund2cDataMaintain_DM.BankName;
                            retgood.BankBranch = refund2cDataMaintain_DM.SubBankName;
                            retgood.AccountNO = refund2cDataMaintain_DM.AccountNO;
                            retgood.AccountName = refund2cDataMaintain_DM.AccountName;
                            retgood = _retgoodRepoAdapter.Update(retgood);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new NullReferenceException(ex.Message);
            }

            return refund2cDataMaintain;
        }
    }
}
