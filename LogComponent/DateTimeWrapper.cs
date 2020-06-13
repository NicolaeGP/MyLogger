using System;
using System.Collections.Generic;
using System.Text;

namespace LogComponent
{
    public class DateTimeWrapper : IDateTime
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}
