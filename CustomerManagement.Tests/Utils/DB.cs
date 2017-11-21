using System;
using CustomerManagement.Logic.Model;
using CustomerManagement.Logic.Utils;

namespace CustomerManagement.Tests.Utils
{
    public class DB : IDisposable
    {
        private readonly UnitOfWork _unitOfWork;

        public DB()
        {
            _unitOfWork = new UnitOfWork();
        }


        public void Dispose()
        {
            _unitOfWork.Dispose();
        }

        public Customer ShouldContainCustomer(string name)
        {
            var repository = new CustomerRepository(_unitOfWork);
            var customer = repository.GetByName(name);

            Assert.NotNull(customer);

            return customer;
        }

        public Customer ShouldContainCustomer(long id)
        {
            var repository = new CustomerRepository(_unitOfWork);
            var customer = repository.GetById(id);

            Assert.True(customer.HasValue);

            return customer.Value;
        }
    }
}