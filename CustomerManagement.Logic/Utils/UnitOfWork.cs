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
            session = SessionFactory.OpenSession();
            transaction = session.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void Dispose()
        {
            if (!isAlive)
                return;

            isAlive = false;

            try
            {
                if (isCommitted)
                    transaction.Commit();
            }
            finally
            {
                transaction.Dispose();
                session.Dispose();
            }
        }

        public void Commit()
        {
            if (!isAlive)
                return;

            isCommitted = true;
        }

        internal Maybe<T> Get<T>(long id) where T : class
        {
            return session.Get<T>(id);
        }

        internal void SaveOrUpdate<T>(T entity)
        {
            session.SaveOrUpdate(entity);
        }

        internal void Delete<T>(T entity)
        {
            session.Delete(entity);
        }

        internal IQueryable<T> Query<T>()
        {
            return session.Query<T>();
        }
    }
}