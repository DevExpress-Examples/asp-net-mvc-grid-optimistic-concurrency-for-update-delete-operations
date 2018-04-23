using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace GridViewOptimisticConcurrencyMvc.Models {
    public class Customer {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        [Timestamp]
        public Byte[] RowVersion { get; set; }
    }

    public class CustomerDbContext : DbContext {
        public CustomerDbContext()
            : base("CustomerDbContext") {
            Database.SetInitializer(new CustomerDbContextInitializer());
        }

        public DbSet<Customer> Customers { get; set; }
    }

    public class CustomerDbContextInitializer : DropCreateDatabaseIfModelChanges<CustomerDbContext> {
        protected override void Seed(CustomerDbContext context) {
            IList<Customer> defaultCustomers = new List<Customer>();

            defaultCustomers.Add(new Customer() { FirstName = "David", LastName = "Adler", Age = 25, Email = "David.Adler@somewhere.com" });
            defaultCustomers.Add(new Customer() { FirstName = "Michael", LastName = "Alcamo", Age = 38, Email = "Michael.Alcamo@somewhere.com" });
            defaultCustomers.Add(new Customer() { FirstName = "Amy", LastName = "Altmann", Age = 27, Email = "Amy.Altmann@somewhere.com" });

            foreach (Customer std in defaultCustomers)
                context.Customers.Add(std);

            base.Seed(context);
        }
    }
}