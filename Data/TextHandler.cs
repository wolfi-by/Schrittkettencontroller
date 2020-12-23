using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using S7.Net;

namespace Schrittkettencontroller.Data
{
    public class TextHandler
    {
        public bool isEnabled
        {
            get
            {
                return timer != null ? timer.Enabled : false;
            }
        }

        public List<Textelement> elements { get; set; } = new List<Textelement>();
        private Timer timer;
        public Textelement current { set; get; } = new Textelement();





        private void TickHandler(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            var index = elements.IndexOf(current);
            current = index < elements.Count - 1 ? current = elements[index + 1] : null;
            if (current != null)
            {
                StartTimer();
            }
            else
            {
                current = new Textelement();
                timer.Dispose();
                timer = null;
            }
            NotifyDataChanged();
        }

        public void Start()
        {
            timer = new Timer();
            timer.Elapsed += TickHandler;
            timer.AutoReset = false;
            current = elements.FirstOrDefault();
            StartTimer();
        }

        public void Stop()
        {
            timer.Stop();
            timer.Dispose();
            timer = null;
            current = new Textelement();
            NotifyDataChanged();
        }

        private void StartTimer()
        {


            if (!string.IsNullOrEmpty(current.AdressToWrite))
            {
                var p = new S7.Net.Plc(CpuType.S7300, "192.168.178.210", 0, 2);
                try
                {
                    p.Open();

                    p.Write(current.AdressToWrite.ToUpper(), current.ValueToWrite);
                    Console.WriteLine($"Written data: {p.Read(current.AdressToWrite)}");
                }

                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    p.Close();
                }


            }
            timer.Interval = current.GetInterval.TotalMilliseconds;
            timer.Start();
        }

        public void AddItem(Textelement item)
        {
            item.ID = FindFreeNumber();

            if (item.GetInterval == TimeSpan.Zero) item.Interval = 1;
            elements.Add(item);
            NotifyDataChanged();
        }

        public void RemoveItem(Textelement item)
        {
            elements.Remove(item);
            NotifyDataChanged();
        }

        public event Action OnChange;

        private void NotifyDataChanged() => OnChange?.Invoke();

        private int FindFreeNumber()
        {
            if (elements.Count > 0)
            {
                var max = elements.Max(t => t.ID);
                return max + 1;
            }
            return 0;
        }

        public string GetText()
        {
            string Text = string.Empty;
            foreach (var item in elements)
            {
                Text += item.Text + "<br />";
            }
            return Text;
        }
    }
}