using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using PaymentAI.Core.Entities;
using PaymentAI.Core.Interfaces.Repository;
using PaymentAI.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAI.Infrastructure.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        private readonly AppDbContext _context;
        public CustomerRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Customer?> GetCustomerByIdAsync(string customerId, string merchantId)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.MerchantId == merchantId);

        }
    }
}
