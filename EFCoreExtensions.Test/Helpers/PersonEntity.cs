using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace EFCoreExtensions.Test.Helpers
{
    [ExcludeFromCodeCoverage]
    public class PersonEntity
    {
        [Key]
        public int IdPerson { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public char Gender { get; set; }
        public int Age { get; set; }
        public DateTime Birthday { get; set; }
        [InverseProperty(nameof(PhoneEntity.Person))]
        public virtual IEnumerable<PhoneEntity> Phones { get; set; }
        [InverseProperty(nameof(EmailEntity.Person))]
        public virtual IEnumerable<EmailEntity> Emails { get; set; }
    }
}
