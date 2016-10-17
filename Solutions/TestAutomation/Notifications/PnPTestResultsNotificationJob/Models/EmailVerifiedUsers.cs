using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnPTestResultsNotificationJob.Models
{
  public class EmailVerifiedUsers
    {
        public string Email { get; set; }
        public string UPN { get; set; }
        public bool isAdmin { get; set; }
        public bool isCoreMember { get; set; }
        

    }
}
