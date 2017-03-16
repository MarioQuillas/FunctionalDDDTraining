using System;
using System.Data;
using System.Linq;
using CustomerManagement.Logic.Common;
using NHibernate;
using NHibernate.Linq;

namespace CustomerManagement.Logic.Utils
{
    public class UnitOfWork : IDisposable
    {
        private readonly ISession session;
        private readonly ITransaction transaction;
        private bool isAlive = true;
        private bool isCommitted;

        public UnitOfWork()
        {
            this.session = SessionFactory.OpenSession();
            this.transaction = session.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void Dispose()
        {
            if (!this.isAlive)
                return;

            this.isAlive = false;

            try
            {
                if (this.isCommitted)
                {
                    this.transaction.Commit();
                }
            }
            finally
            {
                this.transaction.Dispose();
                this.session.Dispose();
            }
        }

        public void Commit()
        {
            if (!this.isAlive)
                return;

            this.isCommitted = true;
        }

        internal Maybe<T> Get<T>(long id) where T : class
        {
            return this.session.Get<T>(id);
        }

        internal void SaveOrUpdate<T>(T entity)
        {
            this.session.SaveOrUpdate(entity);
        }

        internal void Delete<T>(T entity)
        {
            this.session.Delete(entity);
        }

        internal IQueryable<T> Query<T>()
        {
            return this.session.Query<T>();
        }
    }
}
