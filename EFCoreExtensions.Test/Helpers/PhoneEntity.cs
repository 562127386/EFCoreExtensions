using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace EFCoreExtensions.Test.Helpers
{
    [ExcludeFromCodeCoverage]
    public class PhoneEntity
    {
        [Key]
        public int IdPhone { get; set; }
        public string Phone { get; set; }
        public int IdPerson { get; set; }
        [ForeignKey(nameof(IdPerson))]
        public PersonEntity Person { get; set; }
    }
}
