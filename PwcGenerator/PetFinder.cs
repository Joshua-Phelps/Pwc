using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwcGenerator
{
    // PetFinder classes for JSON serialization/deserialization
    public class PetFinderToken
    {
        public string Token_Type { get; set; }
        public int Expires_In { get; set; }
        public string Access_Token { get; set; }
    }

    public class PetFinderAnimal
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public string Organization_ID { get; set; }
        public string Type { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public string Description { get; set; }
        public PetFinderPhoto[] Photos { get; set; }
    }

    public class PetFinderPhoto
    {
        public string Small { get; set; }
        public string Medium { get; set; }
        public string Large { get; set; }
        public string Full { get; set; }
        public string LocalPath { get; set; }
    }

    public class PetFinderAnimals
    {
        public PetFinderAnimal[] animals { get; set; }
        public Pagination pagination { get; set; }
    }

    public class Pagination
    {
        public Link _links { get; set; }
    }

    public class Link
    {
        public Next next { get; set; }
    }

    public class Next
    {
        public string href { get; set; }
    }

    public enum PetFinderAnimalType
    {
        Cat,
        Dog
    }
}
