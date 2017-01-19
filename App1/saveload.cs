using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;

using SQLite;

namespace Kaart
{
    // Functie: deze instantie is een blueprint voor de database. Het bevat de trackstring, de timedate, en de naam.
    class TrackInfo {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string trackstring { get; set; }
        public DateTime timedate { get; set; }
        public string name { get; set; }

        // Lege constructor is vereist, geen idee waarom maar het is zo.
        public TrackInfo() {

        }
        // De constructor die wij gaan gebruiken. 
        public TrackInfo(String str, DateTime timedate , String name) {

            this.trackstring = str;
            this.timedate = timedate;
            this.name = name;

        }

    }

    // Functie: Bij constructor wordt de SQLite verbinding gemaakt, en bij save_track en load_track kunnen tracks worden gesaved en geladen.
    class saveload
    {

        SQLiteConnection database;

        public saveload()
        {
            // Constructor

            string docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            string pad = System.IO.Path.Combine(docsFolder, "tracks.db");

            bool bestaat = File.Exists(pad);
            database = new SQLiteConnection(pad);

            if (!bestaat)
            {
                // Hé eerste keer dat deze wordt ingezet! We maken de tabel tracks aan.

                database.CreateTable<TrackInfo>();


            }
        }

        // Sla een track op
        public void save_track(String trackstring,DateTime timedate, String name) {
            TrackInfo info = new Kaart.TrackInfo(trackstring, timedate, name);
            database.Insert(info);


        }
        // Geef een lijst van TrackInfo instanties en stuur die terug.
        public List<TrackInfo> load_track() {
            List<TrackInfo> returnlist = new List<TrackInfo>();
             
            TableQuery<TrackInfo> query = database.Table<TrackInfo>();

            foreach (TrackInfo informatie in query)
            {
                returnlist.Add(informatie);
            }

            return returnlist;

        }

    }
}