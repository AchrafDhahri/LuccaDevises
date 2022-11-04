using System;
using System.Collections.Generic;
using System.Linq;

namespace LuccaDevises
{
    class DeviseObject : IDisposable
    {
        public string DeviseInitiale { get; set; }
        public string DeviseCible { get; set; }
        public decimal Montant { get; set; }


        public int NbreTauxChange { get; set; }
        public List<TauxChange> ListeTauxChange = new List<TauxChange>();

        void ReleaseUnmanagedResources()
        {
            ListeTauxChange = new List<TauxChange>();
            DeviseInitiale = null;
            DeviseCible = null;
            Montant = 0;

        }

        protected virtual void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DeviseObject()
        {
            Dispose(false);
        }
    }
}
