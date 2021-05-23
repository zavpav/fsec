namespace ProfanityList.WordList
{
    public class BasketEditResult
    {
        public enum EnumResult
        {
            Ok,
            Error
        }

        public BasketEditResult()
        {
            this.Word = "";
            this.Description = "";
        }

        public EnumResult Result { get; set; }

        public string Word { get; set; }

        public string Description { get; set; }
    }
}