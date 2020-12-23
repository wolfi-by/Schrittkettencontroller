using System;

namespace Schrittkettencontroller.Data
{

    public class Textelement
    {
        public string Text { set; get; }
        public int ID { set; get; }
        private TimeSpan _Interval;
        public TimeSpan GetInterval
        {
            get
            {
                return _Interval;
            }
        }
        public int Interval
        {
            get
            {
                return (int)_Interval.TotalSeconds;
            }
            set
            {
                _Interval = TimeSpan.FromSeconds(value);
            }
        }
        public string AdressToWrite { set; get; }
        public string ValueToWrite { set; get; }


    }
}