using System;

namespace TWNewEgg.Models.DomainModels.Lottery
{
    public class DrawResult
    {
        public Award Award { get; set; }
        public Game Game { get; set; }
        public string Message { get; set; }
        public string State { get; set; }

        public enum StateCode
        {
            Finished = 0,
            NotExists = 1,
            NotBegin = 2,
            IsOver = 3,
            NotQualified = 4,
            HasDrawedBefore = 5,
            Error = 6,
        }
    }
}
