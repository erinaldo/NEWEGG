using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Cart;

namespace TWNewEgg.CartRepoAdapters
{

    public class CartTempRepoAdapter : ICartTempRepoAdapter
    {
        private IRepository<CartTemp> _cartTemp;
        private IRepository<CartItemTemp> _cartItemTemp;
        private IRepository<CartCouponTemp> _cartCouponTemp;

        public CartTempRepoAdapter(IRepository<CartTemp> cartTemp, IRepository<CartItemTemp> cartItemTemp, IRepository<CartCouponTemp> cartCouponTemp)
        {
            this._cartTemp = cartTemp;
            this._cartItemTemp = cartItemTemp;
            this._cartCouponTemp = cartCouponTemp;
        }

        public CartTemp CreateCartTemp(CartTemp cartTemp)
        {
            try
            {
                cartTemp.Status = (int)CartTempDM.StatusEnum.Initial;

                this._cartTemp.Create(cartTemp);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return cartTemp;
        }

        public CartTemp UpdateCartTemp(CartTemp cartTemp)
        {
            try
            {
                if (cartTemp.Status < 0)
                {
                    cartTemp.Status = (int)CartTempDM.StatusEnum.Initial;
                }

                this._cartTemp.Update(cartTemp);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return cartTemp;
        }

        public CartItemTemp CreateCartItemTemp(CartItemTemp cartItemTemp)
        {
            try
            {
                this._cartItemTemp.Create(cartItemTemp);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return cartItemTemp;
        }

        public CartItemTemp UpdateCartItemTemp(CartItemTemp cartItemTemp)
        {
            try
            {
                this._cartItemTemp.Update(cartItemTemp);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return cartItemTemp;
        }

        public CartCouponTemp CreateCartCouponTemp(CartCouponTemp cartCouponTemp)
        {
            try
            {
                this._cartCouponTemp.Create(cartCouponTemp);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return cartCouponTemp;
        }

        public CartCouponTemp UpdateCartCouponTemp(CartCouponTemp cartCouponTemp)
        {
            try
            {
                this._cartCouponTemp.Update(cartCouponTemp);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return cartCouponTemp;
        }

        public IQueryable<CartTemp> GetAllCartTemp()
        {
            return this._cartTemp.GetAll();
        }

        public IQueryable<CartTemp> GetCartTemp(string serialNumber)
        {
            IQueryable<CartTemp> getCartTemp = this._cartTemp
                .GetAll()
                .Where(x => x.SerialNumber == serialNumber);
            return getCartTemp;
        }

        public IQueryable<CartTemp> GetCartTemp(int accountId, int cartType)
        {
            IQueryable<CartTemp> getCartTemp = this._cartTemp
                .GetAll()
                .Where(x => x.AccountID == accountId && x.CartTypeID == cartType);
            return getCartTemp;
        }

        public IQueryable<CartTemp> GetCartTemp(List<int> cartTempIDs)
        {
            IQueryable<CartTemp> getCartTemp = this._cartTemp
                .GetAll()
                .Where(x => cartTempIDs.Contains(x.ID));
            return getCartTemp;
        }

        public IQueryable<CartItemTemp> GetCartItemTempList(int cartTempID)
        {
            IQueryable<CartItemTemp> getCartItemTempList = this._cartItemTemp
                .GetAll()
                .Where(x => x.CartTempID == cartTempID);
            return getCartItemTempList;
        }

        public IQueryable<CartCouponTemp> GetCartCouponTempList(int cartTempID)
        {
            IQueryable<CartCouponTemp> getCartCouponTempList = this._cartCouponTemp
                .GetAll()
                .Where(x => x.CartTempID == cartTempID);
            return getCartCouponTempList;
        }

        public void DeleteCartTemp(int cartTempID)
        {
            try
            {
                CartTemp getCartTemp = this._cartTemp
                .GetAll()
                .Where(x => x.ID == cartTempID).FirstOrDefault();
                if (getCartTemp != null)
                {
                    this._cartTemp.Delete(getCartTemp);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteCartTemp(List<int> cartTempIDs)
        {
            try
            {
                List<CartTemp> getCartTemps = this._cartTemp
                .GetAll()
                .Where(x => cartTempIDs.Contains(x.ID)).ToList();
                if (getCartTemps != null && getCartTemps.Count > 0)
                {
                    getCartTemps.ForEach(x =>
                    {
                        this._cartTemp.Delete(x);
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteCartItemTempByID(int cartItemTempID)
        {
            try
            {
                CartItemTemp getCartItemTemp = this._cartItemTemp
                .GetAll()
                .Where(x => x.ID == cartItemTempID).FirstOrDefault();
                if (getCartItemTemp != null)
                {
                    this._cartItemTemp.Delete(getCartItemTemp);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteCartItemTempByIDs(List<int> cartItemTempIDs)
        {
            try
            {
                List<CartItemTemp> getCartItemTemps = this._cartItemTemp
                .GetAll()
                .Where(x => cartItemTempIDs.Contains(x.ID)).ToList();
                if (getCartItemTemps != null && getCartItemTemps.Count > 0)
                {
                    getCartItemTemps.ForEach(x =>
                    {
                        this._cartItemTemp.Delete(x);
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteCartItemTempByCartTempID(int cartTempID)
        {
            try
            {
                List<CartItemTemp> getCartItemTemps = this._cartItemTemp
                .GetAll()
                .Where(x => x.CartTempID == cartTempID).ToList();
                getCartItemTemps.ForEach(x =>
                {
                    this._cartItemTemp.Delete(x);
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteCartItemTempByCartTempIDs(List<int> cartTempIDs)
        {
            try
            {
                List<CartItemTemp> getCartItemTemps = this._cartItemTemp
                .GetAll()
                .Where(x => cartTempIDs.Contains(x.CartTempID)).ToList();
                if (getCartItemTemps != null && getCartItemTemps.Count > 0)
                {
                    getCartItemTemps.ForEach(x =>
                    {
                        this._cartItemTemp.Delete(x);
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteCartCouponTempByID(int cartCouponTempID)
        {
            try
            {
                CartCouponTemp getCartCouponTemp = this._cartCouponTemp
                    .GetAll()
                    .Where(x => x.ID == cartCouponTempID).FirstOrDefault();
                if (getCartCouponTemp != null)
                {
                    this._cartCouponTemp.Delete(getCartCouponTemp);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteCartCouponTempByIDs(List<int> cartCouponTempIDs)
        {
            try
            {
                List<CartCouponTemp> getCartCouponTemps = this._cartCouponTemp
                    .GetAll()
                    .Where(x => cartCouponTempIDs.Contains(x.ID)).ToList();
                if (getCartCouponTemps != null && getCartCouponTemps.Count > 0)
                {
                    getCartCouponTemps.ForEach(x =>
                    {
                        this._cartCouponTemp.Delete(x);
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteCartCouponTempByCartTempID(int cartTempID)
        {
            try
            {
                List<CartCouponTemp> getCartCouponTemps = this._cartCouponTemp
                    .GetAll()
                    .Where(x => x.CartTempID == cartTempID).ToList();
                if (getCartCouponTemps != null && getCartCouponTemps.Count > 0)
                {
                    getCartCouponTemps.ForEach(x =>
                    {
                        this._cartCouponTemp.Delete(x);
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteCartCouponTempByCartTempIDs(List<int> cartTempIDs)
        {
            try
            {
                List<CartCouponTemp> getCartCouponTemps = this._cartCouponTemp
                    .GetAll()
                    .Where(x => cartTempIDs.Contains(x.CartTempID)).ToList();
                if (getCartCouponTemps != null && getCartCouponTemps.Count > 0)
                {
                    getCartCouponTemps.ForEach(x =>
                    {
                        this._cartCouponTemp.Delete(x);
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RemoveTimeoutCartTemps(int cartTempLimitedTimeOfMinute, int cartTempLimitedTimeOfMonth)
        {
            DateTime negativeMinutes = DateTime.UtcNow.AddHours(8).AddMinutes(cartTempLimitedTimeOfMinute);
            DateTime negativeMonths = DateTime.UtcNow.AddHours(8).AddMonths(cartTempLimitedTimeOfMonth);
            List<CartTemp> clearList = new List<CartTemp>();
            List<CartTemp> getCartTempsNoEnd = this._cartTemp.GetAll()
                .Where(x => x.Status != (int)CartTempDM.StatusEnum.SOCreated).ToList();

            List<CartTemp> getCartTempsClose = this._cartTemp.GetAll()
                .Where(x => x.Status == (int)CartTempDM.StatusEnum.SOCreated).ToList();
            try
            {
                getCartTempsNoEnd.ForEach(x =>
                {
                    if (x.CreateDate == null)
                    {
                        clearList.Add(x);
                    }
                    else
                    {
                        if (DateTime.Compare(x.CreateDate.Value, negativeMinutes) <= 0)
                        {
                            clearList.Add(x);
                        }
                    }
                });

                getCartTempsClose.ForEach(x =>
                {
                    if (x.CreateDate == null)
                    {
                        clearList.Add(x);
                    }
                    else
                    {
                        if (DateTime.Compare(x.CreateDate.Value, negativeMonths) <= 0)
                        {
                            clearList.Add(x);
                        }
                    }
                });

                if (clearList.Count > 0)
                {
                    List<int> removeCartTempIDList = clearList.Select(x => x.ID).ToList();
                    clearList.ForEach(x =>
                    {
                        this._cartTemp.Delete(x);
                    });

                    this.DeleteCartItemTempByCartTempIDs(removeCartTempIDList);
                    this.DeleteCartCouponTempByCartTempIDs(removeCartTempIDList);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
