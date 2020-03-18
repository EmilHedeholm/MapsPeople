using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapspeopleConsumer.JsonModel
{
    class RootObject
    {
        public string id { get; set; }
        public string datasetId { get; set; }
        public string baseType { get; set; }
        public Geometry geometry { get; set; }
        public Anchor anchor { get; set; }
        public List<object> aliases { get; set; }
        public int status { get; set; }
        public BaseTypeProperties baseTypeProperties { get; set; }
        public Properties properties { get; set; }
        public string tilesUrl { get; set; }
        public List<TileStyle> tileStyles { get; set; }
        public string parentId { get; set; }
        public string externalId { get; set; }
        public string displayTypeId { get; set; }
        public DisplaySetting displaySetting { get; set; }
        public List<object> categories { get; set; }

    }
}
