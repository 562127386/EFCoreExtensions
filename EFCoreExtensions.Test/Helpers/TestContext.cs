using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace EFCoreExtensions.Test.Helpers
{
    [ExcludeFromCodeCoverage]
    public class TestContext : DbContext
    {
        public TestContext(DbContextOptions<TestContext> options)
            : base(options)
        {
        }
        public virtual DbSet<PersonEntity> People { get; set; }
        public virtual DbSet<PhoneEntity> Phones { get; set; }
        public virtual DbSet<EmailEntity> Emails { get; set; }
    }
}
