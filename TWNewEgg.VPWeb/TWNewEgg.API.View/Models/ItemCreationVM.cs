using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.API.View
{
    public class ItemCreationVM : TWNewEgg.API.Models.ItemSketch
    {
        public ItemCreationVM()
        {
            this.ActionType = 2;
            //this.BoolItemPackage = false;
            //this.BoolIsChokingDanger = false;
            //this.BoolIs18 = false;
            //this.BoolIsShipDanger = false;
            //this.BoolShipType = false;
            this.AesPictureUrlCell = string.Empty;
        }

        public int ActionType { get; set; }
        public string AesItemQtyReg { get; set; }
        public string AesInventoryQtyReg { get; set; }

        /// <summary>
        /// 二維屬性資料
        /// </summary>
        /// <remarks>編輯時，將已儲存的值代入畫面使用</remarks>
        public string AesItemProperty { get; set; }

        /// <summary>
        /// 圖片路徑
        /// </summary>
        /// <remarks>編輯時，將已儲存的值代入畫面使用</remarks>
        public string AesPictureUrlCell { get; set; }

        /// <summary>
        /// 儲存事件的 Controller 路徑
        /// </summary>
        public string SaveActionUrl { get; set; }


        private bool m_BoolItemPackage;
        public bool BoolItemPackage
        {
            get
            {
                if (this.Item.ItemPackage != "1")
                {
                    // 預設為零售
                    m_BoolItemPackage = true;
                }
                else
                {
                    m_BoolItemPackage = false;
                }

                return m_BoolItemPackage;
            }
            set
            {
                m_BoolItemPackage = value;
            }
        }

        private bool m_BoolIsChokingDanger;
        public bool BoolIsChokingDanger
        {
            get
            {
                if (this.Product.IsChokingDanger == "Y")
                {
                    m_BoolIsChokingDanger = true;
                }
                else
                {
                    m_BoolIsChokingDanger = false;
                }

                return m_BoolIsChokingDanger;
            }
            set
            {
                m_BoolIsChokingDanger = value;
            }
        }

        private bool m_BoolIs18;
        public bool BoolIs18
        {
            get
            {
                if (this.Product.Is18 == "Y")
                {
                    m_BoolIs18 = true;
                }
                else
                {
                    m_BoolIs18 = false;
                }

                return m_BoolIs18;
            }
            set
            {
                m_BoolIs18 = value;
            }
        }

        private bool m_BoolIsShipDanger;
        public bool BoolIsShipDanger
        {
            get
            {
                if (this.Product.IsShipDanger == "Y")
                {
                    m_BoolIsShipDanger = true;
                }
                else
                {
                    m_BoolIsShipDanger = false;
                }

                return m_BoolIsShipDanger;
            }
            set
            {
                m_BoolIsShipDanger = value;
            }
        }

        private bool m_BoolShipType;
        public bool BoolShipType
        {
            get
            {
                if (this.Item.ShipType != "N")
                {
                    m_BoolShipType = true;
                }
                else
                {
                    m_BoolShipType = false;
                }

                return m_BoolShipType;
            }
            set
            {
                m_BoolShipType = value;
            }
        }

        private string m_Sdesc1;
        public string Sdesc1
        {
            get
            {
                if (this.Item.Sdesc != null && this.Item.Sdesc.Length > 0)
                {
                    string tempSdesc = this.Item.Sdesc;
                    int startPosition = 0;
                    int endPosition = 0;
                    startPosition = tempSdesc.IndexOf("<li>") + 4;
                    endPosition = tempSdesc.IndexOf("</li>");
                    if (startPosition >= 0 && endPosition >= 0 && endPosition - startPosition >= 0)
                    {
                        m_Sdesc1 = tempSdesc.Substring(startPosition, endPosition - startPosition);
                        tempSdesc = tempSdesc.Replace("<li>" + m_Sdesc1 + "</li>", "");
                    }
                    else
                    {
                        m_Sdesc1 = tempSdesc;
                    }
                }

                return m_Sdesc1;
            }
            set
            {
                m_Sdesc1 = value;
            }
        }

        private string m_Sdesc2;
        public string Sdesc2
        {
            get
            {
                if (this.Item.Sdesc != null && this.Item.Sdesc.Length > 0)
                {
                    string tempSdesc = this.Item.Sdesc;
                    for (int i = 0; i <= 1; i++)
                    {
                        string getSdesc2 = string.Empty;
                        if (tempSdesc.Length > 0)
                        {
                            int startPosition = 0;
                            int endPosition = 0;
                            startPosition = tempSdesc.IndexOf("<li>") + 4;
                            endPosition = tempSdesc.IndexOf("</li>");
                            if (endPosition - startPosition >= 0)
                            {
                                getSdesc2 = tempSdesc.Substring(startPosition, endPosition - startPosition);
                                tempSdesc = tempSdesc.Replace("<li>" + getSdesc2 + "</li>", "");
                            }
                        }

                        if (i == 1)
                        {
                            m_Sdesc2 = getSdesc2;
                        }
                    }
                }

                return m_Sdesc2;
            }
            set
            {
                m_Sdesc2 = value;
            }
        }

        private string m_Sdesc3;
        public string Sdesc3
        {
            get
            {
                if (this.Item.Sdesc != null && this.Item.Sdesc.Length > 0)
                {
                    string tempSdesc = this.Item.Sdesc;
                    for (int i = 0; i <= 2; i++)
                    {
                        string getSdesc3 = string.Empty;
                        if (tempSdesc.Length > 0)
                        {
                            int startPosition = 0;
                            int endPosition = 0;
                            startPosition = tempSdesc.IndexOf("<li>") + 4;
                            endPosition = tempSdesc.IndexOf("</li>");
                            if (endPosition - startPosition >= 0)
                            {
                                getSdesc3 = tempSdesc.Substring(startPosition, endPosition - startPosition);
                                tempSdesc = tempSdesc.Replace("<li>" + getSdesc3 + "</li>", "");
                            }
                        }

                        if (i == 2)
                        {
                            m_Sdesc3 = getSdesc3;
                        }
                    }
                }

                return m_Sdesc3;
            }
            set
            {
                m_Sdesc3 = value;
            }
        }
        #region 二維屬性(規格品待審編輯使用)

        public string color { get; set; }
        public int group_id { get; set; }
        public string size { get; set; }

        #endregion 二維屬性(規格品待審編輯使用)
        public enum GetActionType
        {
            TempAdd = 0,
            TempEdit = 1,
            SketchAdd = 2,
            SketchEdit = 3,
            SketchCopy = 4,
            TempCopy = 5
        }
    }
}
