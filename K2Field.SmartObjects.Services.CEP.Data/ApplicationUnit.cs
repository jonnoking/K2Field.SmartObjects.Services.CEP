using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.Data
{
    public class ApplicationUnit : IDisposable
    {
        private CEPListenerContext _context = new CEPListenerContext();

        private IRepository<Model.EventListener> _eventlistener = null;
        private IRepository<Model.EventListenerLog> _eventlistenerlog = null;


        public IRepository<Model.EventListener> EventListeners
        {
            get
            {
                if (this._eventlistener == null)
                {
                    this._eventlistener = new GenericRepository<Model.EventListener>(this._context);
                }
                return this._eventlistener;
            }
        }

        public IRepository<Model.EventListenerLog> EventListenerLogs
        {
            get
            {
                if (this._eventlistenerlog == null)
                {
                    this._eventlistenerlog = new GenericRepository<Model.EventListenerLog>(this._context);
                }
                return this._eventlistenerlog;
            }
        }

        public int SaveChanges()
        {
            return this._context.SaveChanges();
        }

        public void Dispose()
        {
            if (this._context != null)
            {
                this._context.Dispose();
            }
        }
    }
}
