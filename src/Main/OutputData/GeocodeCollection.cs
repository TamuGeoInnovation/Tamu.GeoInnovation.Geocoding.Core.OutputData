using System.Collections.Generic;
using System.Text;

namespace USC.GISResearchLab.Geocoding.Core.OutputData
{
    public class GeocodeCollection 
    {
        #region Properties
        public List<IGeocode> Geocodes { get; set; }

        #endregion

        public GeocodeCollection()
        {
            Geocodes = new List<IGeocode>();
        }
       

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool verbose)
        {
            StringBuilder ret = new StringBuilder();

            if (!verbose)

                for (int i = 0; i < Geocodes.Count; i++)
                {
                    if (i > 0)
                    {
                        ret.AppendLine();
                    }

                    ret.AppendFormat("[{0}]", Geocodes[i].ToString());
                    
                    if (i < Geocodes.Count -1)
                    {
                        ret.Append(",");
                    }
                }


            return ret.ToString();
        }

        public int GetValidGeocodeCount()
        {
            int ret = 0;


            foreach (IGeocode g in Geocodes)
            {
                if (g.Valid)
                {
                    ret++;
                }
            }


            return ret;
        }

        public List<IGeocode> GetValidGeocodes()
        {
            List<IGeocode> ret = new List<IGeocode>();
            foreach (IGeocode g in Geocodes)
            {
                if (g.Valid)
                {
                    ret.Add(g);
                }
            }
            return ret;
        }
    }
}
