using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SethrysTestDomain
{
    public class Topic
    {
        [Key, MaxLength(Constants.LongTextLength)]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<Topic> Aliases { get; set; }

        [JsonIgnore]
        public virtual ICollection<Topic> SubTopics { get; set; }

        [JsonIgnore]
        public virtual ICollection<Page> Pages { get; set; }
    }
}