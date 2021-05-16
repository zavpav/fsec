namespace ProfanityList.WordList
{
    public class BasketEditResult
    {
        public enum EnumResult
        {
            Ok,
            Error
        }

        public EnumResult Result { get; set; }

        public string Word { get; set; }

        public string Description { get; set; }
    }
}