using System;
using System.Collections.Generic;
using System.Linq;
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
            int i = 0;
            try
            {
               
                foreach (IGeocode g in Geocodes)
                {
                    if (g.Valid && g != null)
                    {
                        ret.Add(g);
                    }
                    i++;
                }
            }
            catch (Exception e)
            {
                throw new Exception("BOO in getValidGeocodes " + e.InnerException + " and msg: " + e.Message + "and record is: " + Convert.ToString(i) + "and value1 is: " + Geocodes[i-1].ToString() + "and value2 is: " + Geocodes[i].ToString() + "and value2 is: " + Geocodes[i + 1].ToString());
            }
            return ret;
        }        
    }
}
