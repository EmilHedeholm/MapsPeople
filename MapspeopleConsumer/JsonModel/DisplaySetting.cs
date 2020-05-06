

namespace MapspeopleConsumer.JsonModel {
    public class DisplaySetting {
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public double? ZoomFrom { get; set; }
        public double? IconScale { get; set; }
        public IconSize IconSize { get; set; }
        public string LabelTemplate { get; set; }
    }
}
