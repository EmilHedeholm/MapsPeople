using System.Collections.Generic;


namespace MapspeopleConsumer.JsonModel {
    public class RootObject {
        public string Id { get; set; }
        public string DatasetId { get; set; }
        public string BaseType { get; set; }
        public Geometry Geometry { get; set; }
        public Anchor Anchor { get; set; }
        public List<object> Aliases { get; set; }
        public int Status { get; set; }
        public BaseTypeProperties BaseTypeProperties { get; set; }
        public Properties Properties { get; set; }
        public string TilesUrl { get; set; }
        public List<TileStyle> TileStyles { get; set; }
        public string ParentId { get; set; }
        public string ExternalId { get; set; }
        public string DisplayTypeId { get; set; }
        public DisplaySetting DisplaySetting { get; set; }
        public List<object> Categories { get; set; }

    }
}
