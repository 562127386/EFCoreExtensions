using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace EFCoreExtensions.Test.Helpers
{
    [ExcludeFromCodeCoverage]
    public class EmailEntity
    {
        [Key]
        public int IdEmail { get; set; }
        public string Email { get; set; }
        public int IdPerson { get; set; }
        [ForeignKey(nameof(IdPerson))]
        public PersonEntity Person { get; set; }
    }
}
