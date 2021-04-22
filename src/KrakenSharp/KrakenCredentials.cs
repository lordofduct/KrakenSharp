using System;
using System.Collections.Generic;
using System.Text;

namespace KrakenSharp
{
    public class KrakenCredentials : ICloneable
    {

        #region Properties

        public System.Security.SecureString ApiKey { get; protected set; }
        public System.Security.SecureString ApiPrivKey { get; protected set; }

        #endregion

        #region CONSTRUCTOR

        protected KrakenCredentials()
        {

        }

        public KrakenCredentials(string apiKey, string apiPriveKey)
        {
            this.ApiKey = apiKey.ToSecureString();
            this.ApiPrivKey = apiPriveKey.ToSecureString();
        }

        #endregion

        #region ICloneable Interface

        public virtual KrakenCredentials Clone()
        {
            return new KrakenCredentials()
            {
                ApiKey = this.ApiKey.Copy(),
                ApiPrivKey = this.ApiPrivKey.Copy(),
            };
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion

    }
}
