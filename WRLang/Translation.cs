namespace WRLang {
    public class Translation {
        public int Id { get; init; }
        public string Text { get; set; } = "";
        public string Original { get; set; } = "";

        public override string? ToString() {
            return Id.ToString();
        }
    }
}
