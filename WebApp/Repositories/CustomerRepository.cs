using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Entities;

namespace WebApp.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private MyDBContext _context;
        public CustomerRepository(MyDBContext context)
        {
            _context = context;
        }

        public IQueryable<Customer> GetAll()
        {
            return _context.Customers;
        }
        public Customer GetSingle(Guid id)
        {
            return _context.Customers.FirstOrDefault(x => x.Id == id);
        }

        public void Add(Customer item)
        {
            _context.Customers.Add(item);
        }

        public void Delete(Guid id)
        {
            Customer customer = GetSingle(id);
            _context.Customers.Remove(customer);
        }

        public void Update(Customer item)
        {
            _context.Customers.Update(item);
        }

        public bool Save()
        {
            return _context.SaveChanges() >= 0;
        }


    }
}
