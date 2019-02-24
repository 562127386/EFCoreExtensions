using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace EFCoreExtensions.Test.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class DbContextHelper
    {
        private static TestContext Context;
        public static TestContext Initialize(this DbContextOptions<TestContext> options, int elements)
        {
            Context = new TestContext(options);
            var flag = true;
            for (int x = 1; x <= elements; x++)
            {
                AddPerson(x, flag);
                AddEmail(x);
                AddPhone(x);
                flag = !flag;
            }
            return Context;
        }
        public static void AddPerson(int id, bool flag)
        {
            int age = (20 + id) > 40 ? 20 : (20 + id);
            Context.People.Add(new PersonEntity
            {
                IdPerson = id,
                FirstName = $"Firstname {id}",
                LastName = $"Lastname {id}",
                Age = age,
                Gender = flag ? 'M' : 'F',
                Birthday = flag ? new DateTime(1988,10,20): new DateTime(1990, 10, 20)
            });
            Context.SaveChanges();
        }
        public static void AddPhone(int idPerson)
        {
            var idCurrent = Context.Phones.OrderByDescending(x => x.IdPhone).FirstOrDefault()?.IdPhone;
            Context.Phones.Add(new PhoneEntity
            {
                IdPhone = idCurrent == null ? 1 : (int)idCurrent + 1,
                IdPerson = idPerson,
                Phone = string.Format("0:D10", idPerson)
            });
            Context.SaveChanges();
        }
        public static void AddEmail(int idPerson)
        {
            var idCurrent = Context.Emails.OrderByDescending(x => x.IdEmail).FirstOrDefault()?.IdEmail;
            Context.Emails.Add(new EmailEntity
            {
                IdEmail = idCurrent == null ? 1 : (int)idCurrent + 1,
                IdPerson = idPerson,
                Email = $"email{idPerson}@email.com"
            });
            Context.SaveChanges();
        }
        public static bool Equal<T>(this T left, T rigth)
        {
            if ((left == null && rigth != null) || (left != null && rigth == null))
                return false;
            if (left == null && rigth == null)
                return true;
            var properties = typeof(T).GetProperties();
            var equalCount = 0;
            foreach (var property in properties)
            {
                var leftValue = left.GetType().GetProperty(property.Name).GetValue(left, null);
                var rigthValue = rigth.GetType().GetProperty(property.Name).GetValue(rigth, null);
                if (leftValue.Equals(rigthValue))
                {
                    equalCount++;
                }
            }
            return properties.Count() == equalCount;
        }
    }
}
