using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebCrawler.Database.Models
{
    public class User : IdentityUser<Guid>
    {
        public User() => Id = Guid.NewGuid();

        public string Name { get; set; }

        public string Picture { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
