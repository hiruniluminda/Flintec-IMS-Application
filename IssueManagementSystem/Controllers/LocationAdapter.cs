using IssueManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IssueManagementSystem.Controllers
{
    public class LocationAdapter
    {
        //-------------------------------------------
        //KT-ASM KT - ASSEMBLY - SM
        //KT-ACC KT - ASSEMBLY - ACCESSORIES
        //KT-APT KT - ASSEMBLY - POTTED
        //KT-ARC KT - ASSEMBLY - ROCKER
        //KT-AWL KT - ASSEMBLY - WELDED
        //KT-HTR KT - HEAT TREATMENT
        //KT-MCS KT - MACHINE SHOP
        //KT-MD2 KT - ASSEMBLY - MEDICAL DEVICE 2
        //KT-GAG KT - GAGING
        //KT-MCS KT - MACHINE SHOP

        //--------------------------------------
        //KG-APB KG - ASSEMBLY - PB
        //KG-ASP KG - ASSEMBLY - SP
        //KT-ASP KT - ASSEMBLY - SP 
        //KG-AMD1 KG-ASSEMBLY-MEDICAL DEVICE

        //KG-GAG  KG - GAGING SECTION

        //KG-MCS KG - MACHINE SHOP

        //--------------RS1 RS2----------------------
        //KG-AFL KG - ASSEMBLY - FAST LANE
        //KG-FL5 KG - ASSEMBLY - FAST LANE 5
        //KG-ASV KG - ASSEMBLY - SAVANNAH

        line ln = new line();
        public LocationAdapter(int ims_Location) {
            try
            {
                using (issue_management_systemEntities1 db = new issue_management_systemEntities1())
                {
                    ln = db.lines.Where(x => x.line_id == ims_Location).SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                // Optionally rethrow or handle the exception in another way
                ln = new line(); // Initialize with a default empty line object
            }
        }

        public String get_NAV_Location()
        {
            try
            {
                if (ln != null)
                {
                    return ln.location; //before remodified and ex handling, this block maintain this line only
                }
                else
                {
                    return "Unknown Location"; // Default value if line is not found
                }
            }
            catch (Exception ex)
            {
                return "Unknown Location"; // Return a safe default
            }
        }

        public String get_LineName()
        {
            try
            {
                if (ln != null)
                {
                    return ln.line_name;//before remodified and ex handling, this block maintain this line only
                }
                else
                {
                    return "Unknown Line"; // Default value if line is not found
                }
            }
            catch (Exception ex)
            {
                return "Unknown Line"; // Return a safe default
            }
        }

        public int get_departmentID()
        {
            try
            {
                if (ln != null)
                {
                    return ln.department_id;//before remodified and ex handling, this block maintain this line only
                }
                else
                {
                    return -1; // Return a default or error value
                }
            }
            catch (Exception ex)
            {
                return -1; // Return a safe default in case of an error
            }
        }
    }
}