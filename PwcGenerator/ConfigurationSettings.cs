using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwcGenerator
{
    internal class ConfigurationSettings
    {
        /* Organizations we want to pull from */
        public static readonly string[] OrganizationIDs = new string[]
        {
            "WA52", /* Adams County Pet Rescue */
            "WA07", /* Benton Franklin Animal Services */
            "WA669", /* Binky Bunny Tales */
            "WA135", /*CASA (Camano Animal Shelter Association) */
            "WA96", /* Cascade Animal Protection Society */
            "WA101", /* Cat Tails Rescue */
            "WA650", /* Dog Gone Seattle */
            "WA538", /* ECPR (Emerald City Pet Rescue) */
            "WA134", /* EAS (Everett Animal Services) */
            "WA562", /* Forever Home Dog Rescue */
            "WA673", /* Kitty Korner Rescue (Pet Café -Edmonds) */
            "WA41", /*Kitsap Humane Society */
            "WA119", /* Meow Cat Rescue*/
            "WA606", /* Must Love Boxers */
            "WA132", /* NOAH (Northwest Organization for Animal Help)*/
            "WA167", /* Pasados Safe Haven*/
            "WA59", /* PAWS and PAWS Cat City*/
            "WA70", /* Purrfect Pals Cat Sanctuary*/
            "WA73", /* Puyallup Animal Rescue*/
            "WA252", /* RASKC (Regional Animal Services of King County)*/
            "WA622", /* Rompin Paws*/
            "WA630", /* Save-a-Mutt*/
            "WA40", /* SAFeR (Seattle Area Feline Rescue)*/
            "WA06", /* Seattle Humane */
            "WA400", /* Seattle Persian and Himalayan */
            "WA60", /* SVHS (Skagit Valley Humane) */
            "WA544", /* Smidget Rescue */
            "WA621", /* Useless Bay */
            "WA47", /* WAMAL (Washington Alaska Malamute Adoption League) */
            "WA175", /* Yakima Valley Pet Rescue */
        };

        /* Total results we will end up with */
        public static readonly int TotalResults = 250;

        /* Folder to download photos and output file locally to */
        public static readonly string WriteLocation = @"C:\Users\elena\Desktop\GoogleDriveFolder\";

        /* Number of images to download in parallel */
        public static readonly int NumberOfParallelDownloads = 8;
    }
}
