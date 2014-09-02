using System;

namespace odict.ru
{
    public class Reply
    {
        public int ID { get; set; }

        public bool WantsToUse { get; set; }
        public bool WantsToContribute { get; set; }
        public bool WantsToBuy { get; set; }

        public string Email { get; set; }

        public DateTime DateTime { get; set; }
    }
}
