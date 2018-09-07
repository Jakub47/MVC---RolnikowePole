using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.SessionState;

namespace RolnikowePole.Infrastucture
{
    public class SessionManager : ISessionManager
    {
        private HttpSessionState session;
        AutoResetEvent _resetEvent = new AutoResetEvent(false);

        public SessionManager()
        {
            try
            {
                session = HttpContext.Current.Session;
            }
            catch(NullReferenceException)
            {
                _resetEvent.Set();
            }

            _resetEvent = null;
        }

        public T Get<T>(string key)
        {
            return (T)session[key];
        }

        public void Set<T>(string name, T value)
        {
            session[name] = value;
        }

        public void Abandon()
        {
            session.Abandon();
        }

        public T TryGet<T>(string key)
        {
            try
            {
                return (T)session[key];
            }
            catch (NullReferenceException)
            {
                return default(T);
            }
        }
    }
}