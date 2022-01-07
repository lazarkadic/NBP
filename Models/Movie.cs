using System;

namespace NBP1.Models
{
    public class Movie{

        public Guid Id {get; init;}
        public String Title {get; set;}

        public String Description {get; set;}

        public String ImageUri {get; set;}

        public int DatePublish {get; set;}

        public int Rate {get; set;}

        public int RateCount {get; set;}


    }
}
