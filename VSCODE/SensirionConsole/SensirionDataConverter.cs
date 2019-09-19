using System;
using Newtonsoft.Json;


namespace SensirionConsole
{
    
public  class SensirionDataConverter
{
    
         class SensirionDataObject
        {
            public Scd30[] scd30 { get; set; }
            public Sps30_Mass_Conc[] sps30_mass_conc { get; set; }
            public Sps30_Number_Conc[] sps30_number_conc { get; set; }
            public Sgpc3[] sgpc3 { get; set; }
            public Sht31d[] sht31d { get; set; }
        }

         class Scd30
        {
            public string name { get; set; }
            public float value { get; set; }
        }

         class Sps30_Mass_Conc
        {
            public string name { get; set; }
            public float value { get; set; }
        }

         class Sps30_Number_Conc
        {
            public string name { get; set; }
            public float value { get; set; }
        }

         class Sgpc3
        {
            public string name { get; set; }
            public int value { get; set; }
        }

         class Sht31d
        {
            public string name { get; set; }
            public float value { get; set; }
        }

 

        public string convertSensirionData(string sensdata)
        {

            string planeDatastr ="{";
    
            var sdo =JsonConvert.DeserializeObject<SensirionDataObject>(sensdata);
            try{
                foreach(Scd30 sdc30 in sdo.scd30 )
                {
                planeDatastr+="\"sdc30"+sdc30.name+"\":"+sdc30.value.ToString()+",";
                }

                foreach(Sht31d sht31d in sdo.sht31d )
                {
                planeDatastr+="\"sht31d"+sht31d.name+"\":"+sht31d.value.ToString()+",";
                }
                
                foreach(Sps30_Mass_Conc sps30m  in sdo.sps30_mass_conc )
                {
                planeDatastr+="\"Sps30_Mass_Conc"+sps30m.name+"\":"+sps30m.value.ToString()+",";
                }

                foreach(Sps30_Number_Conc sps30n in sdo.sps30_number_conc )
                {
                planeDatastr+="\"Sps30_Number_Conc"+sps30n.name+"\":"+sps30n.value.ToString()+",";
                }

                planeDatastr = planeDatastr.TrimEnd(',');
            } catch (Exception ex)
            {

            }
            planeDatastr +="}";
            return planeDatastr;
        }   


}  //     public string Name { get; set; }
   // }
 
}