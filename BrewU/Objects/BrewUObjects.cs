using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BrewU.Objects
{
    /// <summary>
    /// Request used for authentication
    /// </summary>
    public class AuthenticationRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public override string ToString()
        {
            return "{ \"username\":\"" + Username + "\", \"password\": \"" + Password + "\" }";
        }
    }

    /// <summary>
    /// Request used for remote_beers
    /// </summary>
    public class BeerListRequest
    {
        public int Format { get; set; }
        public GroupType GroupID { get; set; }
        public bool HideMyBeers { get; set; }
        public bool IsFromGroup { get; set; }
        public string UserID { get; set; }
    }

    /// <summary>
    /// Request used for mget
    /// </summary>
    public class GetBeerRequest
    {
        public List<string> BeerIDs { get; set; }
    }

    /// <summary>
    /// Response from mget
    /// </summary>
    public class GetBeerResponse
    {
        public BeerModel BeerModel { get; set; }
    }

    /// <summary>
    /// List of Beer Objects
    /// </summary>
    public class BeerModel
    {
        public List<Beer> BeerObjects { get; set; }
    }

    public class Beer
    {
        public string Guid { get; set; }
        public string IsActive { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Format { get; set; }
        public string BreweryID { get; set; }
        public string Origin { get; set; }
        public string AlcoholByVolume { get; set; }
        public string Description { get; set; }
        public string StyleID { get; set; }
        public string LargeImage { get; set; }
        public string MedImage { get; set; }
        public string AddedOn { get; set; }
        public int Available { get; set; }
        public string UpdatedOn { get; set; }
        public string Type { get; set; }
        public string PourSizeID { get; set; }
        public string IsGlutenFree { get; set; }
        public string IsCan { get; set; }
        public string StyleName { get; set; }
        public string BreweryName { get; set; }
        public string HadOn { get; set; }
        public string InFridge { get; set; }
        public string PourSize { get; set; }
        public string LastHadOn { get; set; }
        public int IsBeerOfTheMonth { get; set; }
        public int IsFredsPick { get; set; }

    }

    public enum GroupType : int
    {
        BeerOfTheMonth = 1,
        WhatsNew = 3,
        WhatsHot = 4
    }
}
