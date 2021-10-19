namespace gematria.Services
{
    public class LanguageService
    {
        public string Get(string lang)
        {
            string results = null;

            switch (lang)
            {
                case "en":
                    results = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    break;
                case "he":
                    results = "אבגדהוזחטיכלמנסעפצקרשת";
                    break;
                case "el":
                    //lower
                    results = "αβγδεζηθικλμνξοπρστυφχψω"; 
                    //results = "ΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩ";
                    break;
                default:
                    break;
            }

            return results;
        }
    }
}
