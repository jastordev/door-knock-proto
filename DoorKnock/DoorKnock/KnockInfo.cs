using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace DoorKnock
{
    public class KnockInfo
    {
        public Guid KnockGuid { get; set; } // Knock Guid
        public String UserImg { get; set; } // PLACEHOLDER TYPE - CHANGE TO IMG LATER
        public String FromUser { get; set; }
        public String Message { get; set; }
        public Symbol Reply { get; set; }// PLACEHOLDER TYPE - CHANGE ACCORDINGLY
        public DateTime TimeReceived { get; set; }// PLACEHOLDER TYPE - CHANGE ACCORDINGLY

        public KnockInfo(String knockFromImg, String knockFrom, String knockMsg)
        {
            this.KnockGuid = Guid.NewGuid();
            this.UserImg = knockFromImg;
            this.FromUser = knockFrom;
            this.Message = knockMsg;            
            this.TimeReceived = DateTime.Now;
        }    

    }

    public class KnockList : ObservableCollection<KnockInfo>
    {
        private bool _empty;
        public bool Empty {
            get { return _empty; }
            set { _empty = value; OnPropertyChanged(new PropertyChangedEventArgs("Empty")); }
        }

        public KnockList() {
            this.Empty = true;
        }

        public void AddKnock(KnockInfo knock)
        {
            this.Add(knock);            
            if (this.Count > 0) this.Empty = false;
        }

        public void RemoveKnock(Guid knockGUID)
        {
            this.Remove( this.Single( s => s.KnockGuid == knockGUID));
            if (this.Count == 0) this.Empty = true;
        }

        public KnockInfo GetKnock(Guid knockGUID)
        {
            return this.Single(s => s.KnockGuid == knockGUID);
        }
    }

    public class KnockListModel
    {
        private KnockList incKnockList = new KnockList();
        private KnockList knockHistory = new KnockList();

        public KnockList IncKnockList { get { return this.incKnockList; } }
        public KnockList KnockHistory { get { return this.knockHistory; } }

        public KnockListModel()
        {
            
        }
    }
}
