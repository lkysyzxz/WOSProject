using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOSServer.Cache
{
    public class EquipmentCache
    {
        private static EquipmentCache _instance;
        public static EquipmentCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EquipmentCache();
                }
                return _instance;
            }
        }

        private Dictionary<int, Equipment> equipments = new Dictionary<int, Equipment>();

        private int index = 1;
        private EquipmentCache()
        {
            this.equipments.Add(index, new Equipment(index++, 300, 0, 0, 0, 1));
            this.equipments.Add(index, new Equipment(index++, 500, 0, 0, 0, 2));
            this.equipments.Add(index, new Equipment(index++, 690, 0, 0, 0, 3));
            this.equipments.Add(index, new Equipment(index++, 500, 0, 0, 10, 0));
            this.equipments.Add(index, new Equipment(index++, 900, 0, 10, 0, 0));
            this.equipments.Add(index, new Equipment(index++, 1200, 10, 0, 10, 0));
            this.equipments.Add(index, new Equipment(index++, 1200, 0, 15, 0, 0));
            this.equipments.Add(index, new Equipment(index++, 1500, 0, 20, 0, 0));
            this.equipments.Add(index, new Equipment(index++, 1900, 10, 20, 0, 0));
            this.equipments.Add(index, new Equipment(index++, 2000, 20, 15, 0, 0));
            this.equipments.Add(index, new Equipment(index++, 2000, 0, 25, 10, 0));
            this.equipments.Add(index, new Equipment(index++, 2100, 0, 15, 20, 0));
        }

        public Equipment GetEquipmentByID(int id)
        {
            if (equipments.ContainsKey(id))
                return equipments[id];
            else
                return null;
        }


    }
}
