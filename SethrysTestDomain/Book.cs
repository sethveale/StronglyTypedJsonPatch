using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SethrysTestDomain
{
    public class Book
    {
        [Key, Range(typeof(long), Constants.MinIsbnString, Constants.MaxIsbnString)]
        public long Isbn { get; set; }

        [Required, MaxLength(Constants.SmallTextLength)] public string Name;

        [Required, MaxLength(Constants.SmallTextLength)]
        public string Author { get; set; }

        public virtual ICollection<Page> Pages { get; set; }
    }
}