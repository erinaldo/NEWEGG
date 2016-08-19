using System;
using System.Collections.Generic;
using System.Linq;

namespace TWNewEgg.Models.ViewModels.Login
{
    public class Login
    {
        /// <summary>
        /// user account
        /// </summary>
        public string user { get; set; }
        /// <summary>
        /// user password
        /// </summary>
        public string pass { get; set; }
        /// <summary>
        /// google recaptcha
        /// </summary>
        public string gcap { get; set; }
        /// <summary>
        /// try time
        /// </summary>
        public string ratm { get; set; }
        /// <summary>
        /// retry time over 3 times
        /// </summary>
        public bool rytm { get; set; }
        /// <summary>
        /// remember me
        /// </summary>
        public string remb { get; set; }
        /// <summary>
        /// error message
        /// </summary>
        public string erro { get; set; }
        /// <summary>
        /// error message
        /// </summary>
        public string Iderro { get; set; }
        /// <summary>
        /// error message
        /// </summary>
        public string Pwderro { get; set; }
        /// <summary>
        /// login type, ex:facebook
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// login activity
        /// </summary>
        public string acty { get; set; }
        /// <summary>
        /// login from third party's uid, ex: facebook's uid
        /// </summary>
        public string ouid { get; set; }
    }
}
